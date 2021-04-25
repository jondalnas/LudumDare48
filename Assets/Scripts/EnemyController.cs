using UnityEngine;
using System.Collections;
using UnityEngine.Events;

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
	}

	// Update is called once per frame
	void Update() {
		if (dead) return;

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

	protected abstract void NoticePlayer();

	protected abstract void HitTarget();

	protected abstract void AwayFromTarget();

	protected abstract void CloseToTarget();

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
	}

	public void Kill() {
		if (dead) return;

		blood.Play();
		dead = true;
		StartCoroutine(StopBlood(0.05f));
	}

	public void KillInside() {
		if (dead) return;

		insideBlood.Play();
		dead = true;
		StartCoroutine(StopInsideBlood(0.1f));
		transform.Find("Sprite").gameObject.SetActive(false);
	}
}
