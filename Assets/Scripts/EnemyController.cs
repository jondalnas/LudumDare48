using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public abstract class EnemyController : MonoBehaviour {
	protected GameObject player;

	private ParticleSystem blood;
	private ParticleSystem insideBlood;
	private Transform bloodTransform;
	private bool dead;

	protected Rigidbody2D rb;
	public float speed;
	private bool playerNoticed;
	protected Animator anim;
	protected GameObject target;
	protected bool hasTarget;
	public float minDist = 0.5f;
	public float closeDist = 3f;

	protected Collider2D col;

	protected bool beingControlled;

	// Start is called before the first frame update
	void Start() {
		bloodTransform = transform.Find("Blood");
		blood = bloodTransform.GetComponent<ParticleSystem>();
		insideBlood = transform.Find("Inside Blood").GetComponent<ParticleSystem>();
		blood.Stop();
		insideBlood.Stop();

		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		player = GameObject.FindWithTag("Player");

		col = GetComponent<Collider2D>();

		Init();
	}

	// Update is called once per frame
	void Update() {
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
			rb.velocity = (Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up) * speed;

			//Attack
			if (Input.GetButtonDown("Attack")) {
				Attack();
			} else if (Input.GetButtonUp("Attack")) {
				StopAttack();
			}

			return;
		}

		if (hasTarget) {
			Vector3 move = target.transform.position - transform.position;
			float dist = move.magnitude;
			if (dist < closeDist) {
				CloseToTarget();
			} else {
				AwayFromTarget();
				rb.velocity = move / dist * speed;
				Quaternion rot = Quaternion.LookRotation(move, Vector3.back);
				rot.x = 0;
				rot.y = 0;
				transform.rotation = rot;
				anim.SetBool("Run", true);
			}
		} else {
			anim.SetBool("Run", false);
		}

		if (playerNoticed) {
			NoticePlayer();
		} else {
			Vector3 toPlayer = player.transform.position - transform.position;
			RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer, toPlayer.magnitude, LayerMask.GetMask("Level"));
			if (!hit) {
				playerNoticed = true;
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

	IEnumerator StopBlood(float time) {
		yield return new WaitForSeconds(time);

		blood.Pause();
	}

	IEnumerator StopInsideBlood(float time) {
		yield return new WaitForSeconds(time);

		insideBlood.Pause();
	}

	public void Kill(Vector2 dir) {
		if (dead) return;

		bloodTransform.rotation = Quaternion.Euler(Mathf.Atan2(dir.y, dir.x), 90, 0);

		blood.Play();
		dead = true;
		StartCoroutine(StopBlood(0.05f));

		if (!beingControlled) return;

		beingControlled = false;
		player.GetComponent<PlayerController>().LoseControl();
	}

	public void Kill() {
		if (dead) return;

		blood.Play();
		dead = true;
		StartCoroutine(StopBlood(0.05f));

		if (!beingControlled) return;

		beingControlled = false;
		player.GetComponent<PlayerController>().LoseControl();
	}

	public void KillInside() {
		if (dead) return;

		insideBlood.Play();
		dead = true;
		StartCoroutine(StopInsideBlood(0.1f));
		transform.Find("Sprite").gameObject.SetActive(false);

		if (!beingControlled) return;

		beingControlled = false;
		player.GetComponent<PlayerController>().LoseControl();
	}

	internal void TakeOver() {
		beingControlled = true;

		gameObject.tag = "Player";
		gameObject.layer = LayerMask.NameToLayer("Player");
	}
}
