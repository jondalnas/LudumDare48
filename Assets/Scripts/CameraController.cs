using System;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public Transform target;
	public float cameraMouseFollow = 0.1f;
	public float cameraTargetFollow = 0.1f;

	void FixedUpdate() {
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector3 targetToMouse = mouseWorldPos - target.position;

		transform.position += (target.position.x * Vector3.right + target.position.y * Vector3.up + 10 * Vector3.back + targetToMouse * cameraMouseFollow) * cameraTargetFollow;
		transform.position /= 1 + cameraTargetFollow;
	}

	internal void SetTarget(Transform target) {
		this.target = target;
	}
}
