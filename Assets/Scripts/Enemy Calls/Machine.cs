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
		if (target == player.transform) {
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
		target = player.transform;
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
