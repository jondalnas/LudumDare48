using UnityEngine;

public class Pistol : EnemyController {
	public float pistolCooldown = 0.25f;
	private float timer;
	public GameObject bullet;
	public float bulletDistance = 1f;
	private GameObject shotBullet;

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

	protected override void PlayerAvoidedAttack() {
		StartCoroutine(shotBullet.GetComponent<Bullet>().Destroy());
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
			shotBullet.GetComponent<Bullet>().shooting = this;
		} else if (hit.transform.CompareTag("Enemy")) {
			hit.transform.GetComponent<EnemyController>().Kill(transform.up);
		} else {//Hits wall

		}
	}

	protected override void TakenOver() {
		anim.SetBool("Gun", true);
	}

	protected override void StopAttack() { }
	protected override void UpdateEnemy() { }
}
