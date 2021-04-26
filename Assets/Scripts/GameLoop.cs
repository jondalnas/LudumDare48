using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoop : MonoBehaviour {
	private int levelIndex = 1;

	private int awokenEnemies;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public void EnemyDead() {
		awokenEnemies--;
	}

	public void EnemySeen() {
		awokenEnemies++;
	}

	public void NextLevel() {
		if (awokenEnemies > 0) return;

		SceneManager.LoadScene(levelIndex);
		levelIndex++;
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		Replay.ResetReplay();
	}
}
