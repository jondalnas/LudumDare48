using UnityEngine;

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


	public EnemyType type;
	// Start is called before the first frame update
	void Start() {
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
		Vector3 toPlayer = player.transform.position - transform.position;

		Quaternion rot = Quaternion.LookRotation(toPlayer, Vector3.back);
		rot.x = 0;
		rot.y = 0;
		transform.rotation = rot;
	}
}
