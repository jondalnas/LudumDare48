using UnityEngine;

public class PlayerController : MonoBehaviour {
	private Rigidbody2D rb;
	public int speed = 5;

	private Transform scytheLocation;
	private Transform scythe;
	public float scytheCooldown = 2f;
	private float scytheCooldownTimer;

	private Animator anim;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		scytheLocation = transform.Find("Sprite").Find("Player Arm").Find("Scythe location");
		scythe = transform.Find("Sprite").Find("Scythe");

		anim = GetComponent<Animator>();
	}

	void Update() {
		//Look at mouse
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector3 toMouse = mouseWorldPos - transform.position;

		Quaternion rot = Quaternion.LookRotation(toMouse, Vector3.back);
		rot.x = 0;
		rot.y = 0;
		transform.rotation = rot;

		//Player movement
		rb.velocity = (Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up) * speed;

		//Swing scythe
		scytheCooldownTimer += Time.deltaTime;
		if (Input.GetButtonDown("Attack") && scytheCooldownTimer > scytheCooldown) {
			anim.SetTrigger("Attack");

			scytheCooldownTimer = 0;
		}

		//Handle scythe location
		scythe.position = scytheLocation.position;
	}
}
