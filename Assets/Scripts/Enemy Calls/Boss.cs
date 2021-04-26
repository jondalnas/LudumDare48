using UnityEngine;

public class Boss : EnemyController {
	private int hitCount = 0;
	public float playerKnockback;
	public float knockbackTime;
	private float timer;
	private Vector2 chargeDir;
	public float chargeSpeed;
	public float invTime = 1;
	private float invTimer;
	private bool charging;
	public float knockedoutTime = 1;
	public float knockedoutTimer;

	private void OnCollisionEnter2D(Collision2D collision) {
		charging = false;
		if (collision.transform.CompareTag("Player")) {
			collision.transform.GetComponent<PlayerController>().Kill(gameObject);
		}
		knockedoutTimer = 0;
		
	}
	protected override void Attack() {

	}

	protected override void AwayFromTarget() {

	}

	protected override void CloseToTarget() {
		if (!charging) {
			charging = true;
			chargeDir = (player.transform.position - transform.position).normalized;
		} else {
			move = chargeDir * chargeSpeed;
		}

		if (knockedoutTimer < knockedoutTime) {
			move = Vector2.zero;
		}
	}

	protected override object[] GetAnimationData() {
		return new object[] { anim.GetBool("Run") };
	}

	protected override void HitTarget() {
	}

	protected override void Init() {
		knockedoutTimer = knockedoutTime;
	}

	protected override void NoticePlayer() {
	}

	protected override void PlayerAvoidedAttack() {

	}

	protected override void SetAnimationData(object[] data) {
		anim.SetBool("Run", (bool)data[0]);
	}

	protected override void StopAttack() {
	}

	protected override void TakenOver() {
	}

	protected override void UpdateEnemy() {
		knockedoutTimer += Time.deltaTime;
		invTimer += Time.deltaTime;
		timer += Time.deltaTime;
		if (knockbackTime < timer) {
			player.GetComponent<PlayerController>().knockedBack = false;
		}


	}

	public override void Kill(Vector2 dir, DeathStyle style) {
		if (invTimer < invTime) {
			return;
		}

		invTimer = 0;

		if (hitCount == 5) {
			player.GetComponent<PlayerController>().knockedBack = false;
			Kill(DeathStyle.DECAP);
			return;
		}
		hitCount++;
		timer = 0;
		player.GetComponent<PlayerController>().knockedBack = true;
		player.GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * playerKnockback;
	}

	protected override void TargetDead() { }
}
