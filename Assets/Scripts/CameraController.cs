using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameObject player;
	public float cameraMouseFollow = 0.1f;
	public float cameraTargetFollow = 0.1f;
	
	void Start() {

	}

	void FixedUpdate() {
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector3 playerToMouse = mouseWorldPos - player.transform.position;

		transform.position += (player.transform.position.x * Vector3.right + player.transform.position.y * Vector3.up + 10 * Vector3.back + playerToMouse * cameraMouseFollow) * cameraTargetFollow;
		transform.position /= 1 + cameraTargetFollow;
	}
}
