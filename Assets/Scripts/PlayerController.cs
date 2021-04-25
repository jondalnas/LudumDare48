using UnityEngine;

public class PlayerController : MonoBehaviour {
	private Rigidbody2D rb;
	public int speed = 5;
	public Vector2 inputVelocity;

	private Transform scytheLocation;
	private Transform scythe;
	public static readonly float scytheCooldown = 0.5f;
	private float scytheCooldownTimer = scytheCooldown;
	private Quaternion lockedRot;
	private BoxCollider2D scytheCollider;
	private ContactFilter2D enemyContactFilter;

	private CircleCollider2D mouseCollider;
	private Transform mouseColliderTransform;
	public float teleportCooldown = 4f;
	private float teleportTimer;

	private Animator anim;

	private float timeScaleTarget = 1;

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
		Time.timeScale += (timeScaleTarget - Time.timeScale) / 10f;

		if (Time.timeScale < 0.05) Time.timeScale = 0;
		if (Time.timeScale > 0.95) Time.timeScale = 1;

		if (controllingEnemy) return;

		inputVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		//Look at mouse
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseColliderTransform.position = mouseWorldPos;

		if (Time.timeScale < 0.1) goto timestop;

		//Look at mouse
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

		timestop:

		//Teleport
		mouseColliderTransform.position = mouseWorldPos;

		teleportTimer -= Time.deltaTime;
		if (teleportTimer < 0 && Input.GetButtonDown("Teleport")) {
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

				teleportTimer = teleportCooldown;

				transform.position = cols[index].transform.position;
				cols[index].GetComponent<EnemyController>().KillInside();

				if (timeScaleTarget != 1) timeScaleTarget = 1;
			}
		}
	}

	void FixedUpdate() {
		//Player movement
		if (!controllingEnemy) rb.velocity = inputVelocity * speed * Time.fixedDeltaTime;
		else rb.velocity = Vector3.zero;

		//Handle scythe location
		scythe.position = scytheLocation.position;
	}

	public void LoseControl() {
		controllingEnemy = false;
		Camera.main.GetComponent<CameraController>().SetTarget(transform);
	}

	public void Kill() {
		if (teleportTimer < 0) {
			//Final chance

			if (timeScaleTarget == 0) return;

			timeScaleTarget = 0;
			Time.timeScale = 0.1f;
		} else {
			//Kill player
		}
	}
}
