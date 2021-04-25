using UnityEngine;

public class Scythe : MonoBehaviour {
	public float spinSpeed = 360 * 4;
	public float speed = 10;
	public float returnSpeed = 30;
	[HideInInspector]
	public Vector3 dir;
	private Transform player;

	private Collider2D col;
	private ContactFilter2D hitContactFilter;
	private bool returning;

	public float maxDistFromPlayer = 10;

	private void Start() {
		col = GetComponent<Collider2D>();

		hitContactFilter = new ContactFilter2D {
			useLayerMask = true,
			layerMask = LayerMask.GetMask("Enemy") | LayerMask.GetMask("Level") | LayerMask.GetMask("Default")
		};

		player = GameObject.Find("Player").transform;
	}

	void FixedUpdate() {
		if (dir == Vector3.zero) Debug.LogError("Direction has not been initialized or it has been initialized to zero");

		//Move and rotate
		transform.position += dir * speed * Time.fixedDeltaTime;
		transform.Rotate(0, 0, spinSpeed * Time.fixedDeltaTime);

		//Collision
		Collider2D[] cols = new Collider2D[1];
		if (col.OverlapCollider(hitContactFilter, cols) != 0) {
			if (cols[0].CompareTag("Enemy")) {
				cols[0].GetComponent<EnemyController>().Kill();
			} else {
				Return();
			}
		}

		//Return if too far away
		Vector3 toPlayer = player.position - transform.position;
		float dist = toPlayer.magnitude;
		if (dist > maxDistFromPlayer) {
			Return();
		}

		//Return
		if (returning) {
			dir = toPlayer / dist;

			if (dist < 0.1f) {
				player.GetComponent<PlayerController>().ScytheReturned();
				Destroy(gameObject);
			}
		}
	}

	private void Return() {
		returning = true;
		speed = returnSpeed;
		spinSpeed = 0;
	}
}
