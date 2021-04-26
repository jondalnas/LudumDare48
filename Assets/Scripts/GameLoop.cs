using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoop : MonoBehaviour {
	private int levelIndex = 1;

	private int awokenEnemies;
	private bool hasBeatenStage;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void Update() {
		if (Input.anyKeyDown && hasBeatenStage) ForceNextLevel();
	}

	public void NextLevel() {
		if (awokenEnemies > 0) return;

		hasBeatenStage = true;

		Replay.StartReplay();
	}

	private void ForceNextLevel() {
		SceneManager.LoadScene(levelIndex);
		levelIndex++;
		hasBeatenStage = false;
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		Replay.ResetReplay();
	}

	public void EnemyDead() {
		awokenEnemies--;
	}

	public void EnemySeen() {
		awokenEnemies++;
	}
}
