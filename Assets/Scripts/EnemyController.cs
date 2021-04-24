using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public GameObject player;
	public enum EnemyType {
		Brawler,
		Pistol,
		Machine,
		Big
	}
	[Header(" Sprites" )]
	public Sprite brawler;
	public Sprite pistol;
	public Sprite machine;
	public Sprite big;

	private ParticleSystem blood;
	private ParticleSystem insideBlood;
	private Transform bloodTransform;
	private bool dead;

	public EnemyType type;
	// Start is called before the first frame update
	void Start() {
		bloodTransform = transform.Find("Blood");
		blood = bloodTransform.GetComponent<ParticleSystem>();
		insideBlood = transform.Find("Inside Blood").GetComponent<ParticleSystem>();
		blood.Stop();
		insideBlood.Stop();

		switch (type) {
			case EnemyType.Brawler:
				transform.GetComponentInChildren<SpriteRenderer>().sprite = brawler;
			break;

			case EnemyType.Pistol:
				transform.GetComponentInChildren<SpriteRenderer>().sprite = pistol;
			break;

			case EnemyType.Machine:
			transform.GetComponentInChildren<SpriteRenderer>().sprite = machine;
			break;

			case EnemyType.Big:
			transform.GetComponentInChildren<SpriteRenderer>().sprite = big;
			break;
		}
	}

	// Update is called once per frame
	void Update() {
		if (dead) return;

		Vector3 toPlayer = player.transform.position - transform.position;

		Quaternion rot = Quaternion.LookRotation(toPlayer, Vector3.back);
		rot.x = 0;
		rot.y = 0;
		transform.rotation = rot;
	}

	IEnumerator StopBlood(float time) {
		yield return new WaitForSeconds(time);

		blood.Pause();
	}

	IEnumerator StopInsideBlood(float time) {
		yield return new WaitForSeconds(time);

		insideBlood.Pause();
	}

	public void Kill(Vector2 dir) {
		if (dead) return;

		bloodTransform.rotation = Quaternion.Euler(Mathf.Atan2(dir.y, dir.x), 90, 0);

		blood.Play();
		dead = true;
		StartCoroutine(StopBlood(0.05f));
	}

	public void Kill() {
		if (dead) return;

		blood.Play();
		dead = true;
		StartCoroutine(StopBlood(0.05f));
	}

	public void KillInside() {
		if (dead) return;

		insideBlood.Play();
		dead = true;
		StartCoroutine(StopInsideBlood(0.1f));
		transform.Find("Sprite").gameObject.SetActive(false);
	}
}
