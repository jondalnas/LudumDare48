using UnityEngine;

public class Pistol : EnemyController {
	public float pistolCooldown = 0.25f;
	private float timer;
	override protected void NoticePlayer() {
		hasTarget = true;
		target = player;

		Debug.Log("Hello");
	}

	override protected void HitTarget() {
		
	}

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
}
