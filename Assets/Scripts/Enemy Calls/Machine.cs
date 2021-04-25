using UnityEngine;

public class Machine : EnemyController {
	protected override void Attack() {
		anim.SetBool("Shoot", true);
	}

	protected override void AwayFromTarget() {
		anim.SetBool("Shoot", false);
		anim.SetBool("Gun", false);
	}

	protected override void CloseToTarget() {
		Vector3 move = target.transform.position - transform.position;
		Quaternion rot = Quaternion.LookRotation(move, Vector3.back);
		rot.x = 0;
		rot.y = 0;
		transform.rotation = rot;
		if (target == player) {
			anim.SetBool("Gun", true);
			rb.velocity = Vector2.zero;
			anim.SetBool("Shoot", true);		
		}
	}

	protected override void HitTarget() {
	}

	protected override void Init() {
	}

	protected override void NoticePlayer() {
		hasTarget = true;
		target = player;
	}

	protected override void StopAttack() {
		anim.SetBool("Shoot", false);
	}

	protected override void TakenOver() {
		anim.SetBool("Gun", true);
	}

	protected override void UpdateEnemy() {
	}

	public void Shoot() {
		RaycastHit2D[] hits = new RaycastHit2D[1];
		if (col.Raycast(transform.up, hits) != 0) {
			if (hits[0].transform.CompareTag("Player")) {
				hits[0].transform.GetComponent<PlayerController>().Kill();
			} else if (hits[0].transform.CompareTag("Enemy")) {
				hits[0].transform.GetComponent<EnemyController>().Kill(transform.up, DeathStyle.SHOT);
			} else {//Hits wall

			}
		}
	}
}
