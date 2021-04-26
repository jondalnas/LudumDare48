using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoop : MonoBehaviour {
	private static int levelIndex = 0;

	private static int awokenEnemies;
	private static bool hasBeatenStage;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void Update() {
		if (hasBeatenStage && Input.anyKeyDown) ForceNextLevel();
	}

	public static void NextLevel() {
		if (levelIndex == 0) {
			levelIndex++;
			SceneManager.LoadScene(levelIndex);
			return;
		}

		if (awokenEnemies > 0) return;

		hasBeatenStage = true;

		Replay.StartReplay();
	}

	private void ForceNextLevel() {
		SceneManager.LoadScene(++levelIndex);
		hasBeatenStage = false;
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		Replay.ResetReplay();
	}

	public static void CloseGame() {
		Application.Quit();
	}

	public static void EnemyDead() {
		awokenEnemies--;
	}

	public static void EnemySeen() {
		awokenEnemies++;
	}

	internal static void RestartLevel() {
		SceneManager.LoadScene(levelIndex);
		Replay.ResetReplay();
		hasBeatenStage = false;
	}
}
