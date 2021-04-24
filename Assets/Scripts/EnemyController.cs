using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyController : MonoBehaviour {
	public GameObject player;
	protected Rigidbody2D rb;
	public float speed;
	private bool playerNoticed;
	protected Animator anim;
	protected GameObject target;
	protected bool hasTarget;
	public float minDist = 0.5f;
	public float closeDist = 1.2f;

	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update() {
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
}
