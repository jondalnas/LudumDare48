using System;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public Transform target;
	public float cameraMouseFollow = 0.01f;
	public float cameraTargetFollow = 5f;
	public Material blackAndWhiteMat;
	private float blend;

	void Start() {
		blackAndWhiteMat = new Material(Shader.Find("Hidden/Black and White"));
	}

	void FixedUpdate() {
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector3 targetToMouse = mouseWorldPos - target.position;

		transform.position += (target.transform.position.x * Vector3.right + target.transform.position.y * Vector3.up + targetToMouse * cameraMouseFollow) * cameraTargetFollow * Time.deltaTime;
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

	internal void SetTarget(Transform target) {
		this.target = target;
	}
}
