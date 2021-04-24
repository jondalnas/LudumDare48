using UnityEngine;

public class PlayerController : MonoBehaviour {
	private Rigidbody2D rb;
	public int speed = 5;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
	}

	void Update() {
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector3 toMouse = mouseWorldPos - transform.position;

		Quaternion rot = Quaternion.LookRotation(toMouse, Vector3.back);
		rot.x = 0;
		rot.y = 0;
		transform.rotation = rot;

		rb.velocity = (Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up) * speed;
	}

	public void kill() {
		Debug.Log("Dead");
	}
}
