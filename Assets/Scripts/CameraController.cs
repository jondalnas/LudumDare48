using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameObject player;
	public float cameraMouseFollow = 0.1f;
	public float cameraTargetFollow = 0.1f;
	public Material blackAndWhiteMat;
	private float blend;

	void Start() {
		blackAndWhiteMat = new Material(Shader.Find("Hidden/Black and White"));
	}

	void Update() {
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector3 playerToMouse = mouseWorldPos - player.transform.position;

		transform.position += (player.transform.position.x * Vector3.right + player.transform.position.y * Vector3.up + playerToMouse * cameraMouseFollow) * cameraTargetFollow * Time.deltaTime;
		transform.position /= 1 + cameraTargetFollow * Time.deltaTime;
		transform.position = transform.position.x * Vector3.right + transform.position.y * Vector3.up + 10 * Vector3.back;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		blend = (blend * 19.0f + Time.timeScale) * 0.05f;

		if (Time.timeScale == 1) {
			Graphics.Blit(source, destination);
			return;
		}

		blackAndWhiteMat.SetFloat("_blend", blend);
		Graphics.Blit(source, destination, blackAndWhiteMat);
	}
}
