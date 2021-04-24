using UnityEngine;

public class Brawler : EnemyController {
	override protected void NoticePlayer() {
		hasTarget = true;
		target = player;
	}

	override protected void HitTarget() {
		if (target == player) {
			player.GetComponent<PlayerController>().kill();
			rb.velocity = Vector2.zero;
		}
	}

	override protected void AwayFromTarget() {
		anim.SetBool("Punch", false);
	}
	override protected void CloseToTarget() {
		anim.SetBool("Punch", true);
		Vector3 move = target.transform.position - transform.position;
		float dist = move.magnitude;
		if (dist < minDist) {
			HitTarget();
		} else {
			rb.velocity = move / dist * speed;
			Quaternion rot = Quaternion.LookRotation(move, Vector3.back);
			rot.x = 0;
			rot.y = 0;
			transform.rotation = rot;
			anim.SetBool("Run", true);
		}
	}
}
