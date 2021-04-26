using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public abstract class EnemyController : MonoBehaviour, IReplayable {
	protected GameObject player;

	private ParticleSystem blood;
	private ParticleSystem insideBlood;
	private Transform bloodTransform;
	private bool dead;
	public Sprite shotSprite;
	public Sprite decapSprite;
	private SpriteRenderer sr;
	protected Vector2 move;
	private Vector2 playerVel;
	public Transform[] patrolPoints;
	protected int targetCount;
	private int patrolDir = -1;

	public enum DeathStyle {
		DECAP,
		SHOT,
		EXPLOSION
	}

	protected Rigidbody2D rb;
	public float speed;
	public float patrolSpeed;
	private bool playerNoticed;
	protected Animator anim;
	protected Transform target;
	protected bool hasTarget;
	public float minDist = 0.5f;
	public float closeDist = 3f;

	protected Collider2D col;

	protected bool beingControlled;
	protected bool firstFrame;

	private Detail detail;

	// Start is called before the first frame update
	void Start() {
		bloodTransform = transform.Find("Blood");
		blood = bloodTransform.GetComponent<ParticleSystem>();
		insideBlood = transform.Find("Inside Blood").GetComponent<ParticleSystem>();
		blood.Stop();
		insideBlood.Stop();
		sr = GetComponentInChildren<SpriteRenderer>();

		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		player = GameObject.FindWithTag("Player");

		col = GetComponent<Collider2D>();

		detail = GameObject.Find("Detail").GetComponent<Detail>();

		hasTarget = patrolPoints.Length != 0;
		if (hasTarget) target = patrolPoints[0];

		Init();
	}


	private void FixedUpdate() {
		if (beingControlled) {
			rb.velocity = playerVel;
		} else if (hasTarget) {
			if (!target.CompareTag("Player")) {
				rb.velocity = move * patrolSpeed;
			} else {
				rb.velocity = move * speed;
			}
		}
	}

	// Update is called once per frame
	void Update() {
		if (Replay.IN_REPLAY) return;

		if (dead) return;

		UpdateEnemy();

		if (beingControlled) {
			//Look at mouse
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			Vector3 toMouse = mouseWorldPos - transform.position;

			Quaternion rot = Quaternion.LookRotation(toMouse, Vector3.back);
			rot.x = 0;
			rot.y = 0;
			transform.rotation = rot;

			//Player movement
			playerVel = (Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up) * speed;
			if (rb.velocity.sqrMagnitude < 0.001) {
				anim.SetBool("Run", false);
			} else {
				anim.SetBool("Run", true);
			}

			//Attack
			if (Input.GetButtonDown("Attack")) {
				Attack();
			} else if (Input.GetButtonUp("Attack")) {
				StopAttack();
			}

			//Exit body
			if (Input.GetButtonDown("Enemy Control") && !firstFrame) {
				KillInside(DeathStyle.EXPLOSION);
			}

			firstFrame = false;

			return;
		}

		if (hasTarget) {
			move = target.position - transform.position;
			float dist = move.magnitude;
			Quaternion rot = Quaternion.LookRotation(move, Vector3.back);
			rot.x = 0;
			rot.y = 0;
			transform.rotation = rot;
			if (dist < closeDist) {
				if (target.CompareTag("Player")) {
					CloseToTarget();
					move = Vector3.zero;
				} else {
					move /= dist;
					if (dist < minDist) {
						if (targetCount >= patrolPoints.Length -1 || targetCount <= 0) {
							patrolDir = -patrolDir;
						}
						targetCount += patrolDir;
						
						target = patrolPoints[targetCount];
					}
				}
			} else {
				move /= dist;
				AwayFromTarget();
				
				anim.SetBool("Run", true);
			}
		} else {
			move = Vector3.zero;
			anim.SetBool("Run", false);
		}

		if (playerNoticed) {
			NoticePlayer();
		} else {
			Vector3 toPlayer = player.transform.position - transform.position;
			RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer, toPlayer.magnitude, LayerMask.GetMask("Level"));
			if (!hit) {
				playerNoticed = true;

				GameObject.Find("-GAME LOOP-").SendMessage("EnemySeen");
			}
		}
	}

	protected abstract void Init();

	protected abstract void UpdateEnemy();

	protected abstract void NoticePlayer();

	protected abstract void HitTarget();

	protected abstract void AwayFromTarget();

	protected abstract void CloseToTarget();

	protected abstract void Attack();

	protected abstract void StopAttack();

	protected abstract void TakenOver();

	protected abstract void PlayerAvoidedAttack();

	IEnumerator StopBlood(float time) {
		yield return new WaitForSeconds(time);

		blood.Pause();
	}

	IEnumerator StopInsideBlood(float time) {
		yield return new WaitForSeconds(time);

		insideBlood.Pause();
	}

	public void Kill(Vector2 dir, DeathStyle style) {
		if (dead) return;

		transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90);

		Kill(style);
	}

	private void Kill(DeathStyle style) {
		if (dead) return;

		if (playerNoticed)
			GameObject.Find("-GAME LOOP-").SendMessage("EnemyDead");
		anim.enabled = false;

		foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>()) {
			renderer.enabled = false;
		}

		sr.enabled = true;

		switch (style) {
			case DeathStyle.DECAP:
				sr.sprite = decapSprite;
				break;

			case DeathStyle.SHOT:
				sr.sprite = shotSprite;
				break;

			case DeathStyle.EXPLOSION:
				sr.enabled = false;
				break;
		}

		sr.transform.Rotate(0, 0, 180);

		blood.Play();
		dead = true;
		StartCoroutine(StopBlood(3f));
		rb.simulated = false;
		foreach (Collider2D col in GetComponentsInChildren<Collider2D>()) {
			col.enabled = false;
		}

		detail.ColorFromPos(Color.HSVToRGB(UnityEngine.Random.Range(0.0f, 1.0f), 1.0f, 1.0f), 40, transform.position);

		if (!beingControlled) return;
		
		beingControlled = false;
		player.GetComponent<PlayerController>().LoseControl();
	}

	public void KillInside(DeathStyle style) {
		if (dead) return;

		insideBlood.Play();
		StartCoroutine(StopInsideBlood(3f));
		Kill(style);
		blood.Stop();
	}

	internal void TakeOver() {
		beingControlled = true;
		firstFrame = true;

		gameObject.tag = "Player";
		gameObject.layer = LayerMask.NameToLayer("Player");
	}

	public void ReplayData(int frame, object[] data) {
		transform.position = (Vector3)data[0];
		transform.rotation = (Quaternion)data[1];
		transform.localScale = (Vector3)data[2];
	}

	public object[] CollectData() {
		return new object[] { transform.position, transform.rotation, transform.localScale };
	}
}
