using UnityEngine;

public class Machine : EnemyController {
	private GameObject shotBullet;
	public GameObject bullet;
	public float bulletDistance = 1f;

	protected override void Attack() {
		anim.SetBool("Shoot", true);
	}

	protected override void AwayFromTarget() {
		anim.SetBool("Shoot", false);
		anim.SetBool("Gun", false);
	}

	protected override void CloseToTarget() {
		move = Vector3.zero;

		if (target == player.transform) {
			anim.SetBool("Gun", true);
			rb.velocity = Vector2.zero;
			anim.SetBool("Shoot", true);		
		}
	}

	protected override void HitTarget() { }

	protected override void Init() { }

	protected override void NoticePlayer() { }

	protected override void StopAttack() {
		anim.SetBool("Shoot", false);
	}

	protected override void TakenOver() {
		anim.SetBool("Gun", true);
	}

	protected override void UpdateEnemy() {
	}

	public void BulletRayCast(Transform bullet) {
		RaycastHit2D hit;
		if (hit = Physics2D.Raycast(bullet.position, bullet.right, Mathf.Infinity, ~((1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("UI"))))) {
			ShootBullet(hit);
		}
	}

	public void Shoot() {
		RaycastHit2D[] hits = new RaycastHit2D[1];
		if (col.Raycast(transform.up, hits, Mathf.Infinity, ~((1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("UI")))) != 0) {
			ShootBullet(hits[0]);
		}
	}

	private void ShootBullet(RaycastHit2D hit) {
		if (hit.transform.CompareTag("Player")) {
			hit.transform.GetComponent<PlayerController>().Kill(gameObject);

			Vector3 toPlayer = transform.position - hit.transform.position;
			shotBullet = Instantiate(bullet, toPlayer.normalized * bulletDistance, Quaternion.Euler(0, 0, Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg + 180));
			shotBullet.GetComponent<Bullet>().shooting = transform;
		} else if (hit.transform.CompareTag("Enemy")) {
			hit.transform.GetComponent<EnemyController>().Kill(transform.up, DeathStyle.SHOT);
		} else {//Hits wall

		}
	}

	protected override void PlayerAvoidedAttack() {
		StartCoroutine(shotBullet.GetComponent<Bullet>().Destroy());
	}

	protected override object[] GetAnimationData() {
		return new object[] { anim.GetBool("Run"), anim.GetBool("Gun"), anim.GetBool("Shoot") };
	}

	protected override void SetAnimationData(object[] data) {
		anim.SetBool("Run", (bool)data[0]);
		anim.SetBool("Gun", (bool)data[1]);
		anim.SetBool("Shoot", (bool)data[1]);
	}
}
