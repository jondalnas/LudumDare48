using UnityEngine;

public class PlayerController : MonoBehaviour {
	private Rigidbody2D rb;
	public int speed = 5;

	private Transform scytheLocation;
	private Transform scythe;
	public static readonly float scytheCooldown = 0.5f;
	private float scytheCooldownTimer = scytheCooldown;
	private Quaternion lockedRot;
	private BoxCollider2D scytheCollider;
	private ContactFilter2D enemyContactFilter;

	private CircleCollider2D mouseCollider;
	private Transform mouseColliderTransform;

	private Animator anim;

	private bool controllingEnemy;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		scytheLocation = transform.Find("Sprite").Find("Player Arm").Find("Scythe location");
		scythe = transform.Find("Sprite").Find("Scythe");
		scytheCollider = scythe.GetComponent<BoxCollider2D>();

		mouseColliderTransform = transform.Find("Mouse collider");
		mouseCollider = mouseColliderTransform.GetComponent<CircleCollider2D>();

		anim = GetComponent<Animator>();

		enemyContactFilter = new ContactFilter2D {
			useLayerMask = true,
			layerMask = LayerMask.GetMask("Enemy")
		};
	}

	void Update() {
		if (controllingEnemy) return;

		//Look at mouse
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseColliderTransform.position = mouseWorldPos;

		if (scytheCooldownTimer < scytheCooldown * 0.7f) {
			lockedRot = transform.rotation;
		} else if (scytheCooldownTimer < scytheCooldown) { //When cooldown is 70% done
			Vector3 toMouse = mouseWorldPos - transform.position;

			Quaternion rot = Quaternion.LookRotation(toMouse, Vector3.back);
			rot.x = 0;
			rot.y = 0;

			rot.eulerAngles = Mathf.LerpAngle(lockedRot.eulerAngles.z, rot.eulerAngles.z, (scytheCooldownTimer / scytheCooldown - 0.7f) / (1 - 0.7f)) * Vector3.forward;
			transform.rotation = rot;
		} else { //When cooldown is done
			Vector3 toMouse = mouseWorldPos - transform.position;

			Quaternion rot = Quaternion.LookRotation(toMouse, Vector3.back);
			rot.x = 0;
			rot.y = 0;
			transform.rotation = rot;
		}

		//Player movement
		rb.velocity = (Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up) * speed;

		//Swing scythe
		scytheCooldownTimer += Time.deltaTime;
		if (scytheCooldownTimer > scytheCooldown) {
			if (Input.GetButtonDown("Attack")) {
				anim.SetTrigger("Attack");

				scytheCooldownTimer = 0;
			}
		} else { //Is swinging
			Collider2D[] cols = new Collider2D[1];
			if (scytheCollider.OverlapCollider(enemyContactFilter, cols) != 0) {
				if (cols[0].gameObject.CompareTag("Enemy")) {
					cols[0].gameObject.GetComponent<EnemyController>().Kill();
				}
			}
		}

		//Taking over enemies
		if (Input.GetButtonDown("Enemy Control")) {
			Collider2D[] cols = new Collider2D[4];
			int colNum;
			if ((colNum = mouseCollider.OverlapCollider(enemyContactFilter, cols)) != 0) {
				float dist = float.PositiveInfinity;
				int index = -1;
				for (int i = 0; i < colNum; i++) {
					float currDist = (cols[i].transform.position - transform.position).sqrMagnitude;
					if (currDist < dist) {
						dist = currDist;
						index = i;
					}
				}

				controllingEnemy = true;
				Camera.main.GetComponent<CameraController>().SetTarget(cols[index].transform);
				cols[index].GetComponent<EnemyController>().TakeOver();
			}
		}

		//Teleport
		if (Input.GetButtonDown("Teleport")) {
			Collider2D[] cols = new Collider2D[4];
			int colNum;
			if ((colNum = mouseCollider.OverlapCollider(enemyContactFilter, cols)) != 0) {
				float dist = float.PositiveInfinity;
				int index = -1;
				for (int i = 0; i < colNum; i++) {
					float currDist = (cols[i].transform.position - transform.position).sqrMagnitude;
					if (currDist < dist) {
						dist = currDist;
						index = i;
					}
				}

				transform.position = cols[index].transform.position;
				cols[index].GetComponent<EnemyController>().KillInside();
			}
		}

		//Handle scythe location
		scythe.position = scytheLocation.position;
	}

	public void LoseControl() {
		controllingEnemy = false;
		Camera.main.GetComponent<CameraController>().SetTarget(transform);
	}

	public void Kill() {
		Debug.Log("Dead");
	}
}
