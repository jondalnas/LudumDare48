using UnityEngine;

public class Pistol : EnemyController {
	public float pistolCooldown = 0.25f;
	private float timer;

	override protected void Init() { }

	override protected void NoticePlayer() {
		hasTarget = true;
		target = player;
	}

	override protected void HitTarget() { }

	override protected void AwayFromTarget() {
		anim.SetBool("Gun", false);
	}

	override protected void CloseToTarget() {
		Vector3 move = target.transform.position - transform.position;
		Quaternion rot = Quaternion.LookRotation(move, Vector3.back);
		rot.x = 0;
		rot.y = 0;
		transform.rotation = rot;
		if (target == player) {
			anim.SetBool("Gun", true);
			rb.velocity = Vector2.zero;
			timer += Time.deltaTime;
			if (pistolCooldown < timer) {
				anim.SetTrigger("Shoot");
				timer = 0;
			}
		}
	}

	override protected void Attack() {
		anim.SetTrigger("Shoot");
	}

	public void Shoot() {
		RaycastHit2D[] hits = new RaycastHit2D[1];
		if (col.Raycast(transform.up, hits, Mathf.Infinity, ~((1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("UI")))) != 0) {
			if (hits[0].transform.CompareTag("Player")) {
				hits[0].transform.GetComponent<PlayerController>().Kill();
			} else if (hits[0].transform.CompareTag("Enemy")) {
				hits[0].transform.GetComponent<EnemyController>().Kill(transform.up);
			} else {//Hits wall

			}
		}
	}

	protected override void TakenOver() {
		anim.SetBool("Gun", true);
	}

	protected override void StopAttack() { }
	protected override void UpdateEnemy() { }
}
