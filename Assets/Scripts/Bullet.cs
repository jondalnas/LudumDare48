using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float speed = 50.0f;
	public Pistol shooting;

	void FixedUpdate() {
		transform.position += transform.right * speed * Time.fixedDeltaTime;
	}

	public IEnumerator Destroy() {
		yield return new WaitForSeconds(0);

		shooting.BulletRayCast(transform);
		Destroy(gameObject);
	}
}
