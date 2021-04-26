using UnityEngine;

public class Pistol : EnemyController {
	public float pistolCooldown = 0.25f;
	private float timer;

	override protected void Init() { }

	override protected void NoticePlayer() {
		hasTarget = true;
		target = player.transform;
	}

	override protected void HitTarget() { }

	override protected void AwayFromTarget() {
		anim.SetBool("Gun", false);
	}

	override protected void CloseToTarget() {
		if (target == player.transform) {
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
		if (col.Raycast(transform.up, hits) != 0) {
			if (hits[0].transform.CompareTag("Player")) {
				hits[0].transform.GetComponent<PlayerController>().Kill();
			} else if (hits[0].transform.CompareTag("Enemy")) {
				hits[0].transform.GetComponent<EnemyController>().Kill(transform.up, DeathStyle.SHOT);
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
