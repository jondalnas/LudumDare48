using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Replay : MonoBehaviour {
	public static List<IReplayable> replayObjects = new List<IReplayable>();
	public static List<object[]> replay = new List<object[]>();
	private static IReplayable[] replayObjectsArray;
	private static object[][] replayArray;
	private static int totalFrames;

	public static bool IN_REPLAY;
	private static int replayIndex;

	private bool loop = true;

	void Start() {
		ResetReplay();
	}

	void FixedUpdate() {
		if (!IN_REPLAY) {
			foreach (IReplayable ro in replayObjects) {
				replay.Add(ro.CollectData());
			}

			totalFrames++;
		} else {
			for (int i = 0; i < replayObjects.Count; i++) {
				int index = i + replayObjects.Count * replayIndex;

				replayObjectsArray[i].ReplayData(replayIndex, replayArray[index]);
			}

			replayIndex++;

			if (replayIndex >= totalFrames) {
				replayIndex = 0;

				if (!loop) {
					IN_REPLAY = false;

					foreach (IReplayable ro in replayObjects) {
						ro.ReplayEnded();
					}
				} else {
					foreach (IReplayable ro in replayObjects) {
						ro.ReplayReset();
					}
				}
			}
		}
	}

	public static void StartReplay() {
		foreach (IReplayable ro in replayObjects) {
			ro.ReplayReset();
		}

		IN_REPLAY = true;

		replayArray = replay.ToArray();
		replayObjectsArray = replayObjects.ToArray();
	}

	public static void ResetReplay() {
		replayObjects.Clear();
		replay.Clear();

		replayObjects.Add((IReplayable)GameObject.Find("Player").GetComponent<PlayerController>());
		foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy")) {
			replayObjects.Add(e.GetComponent<EnemyController>());
		}

		foreach (DoorReplay d in (DoorReplay[])GameObject.FindObjectsOfType(typeof(DoorReplay))) {
			replayObjects.Add(d);
		}

		IN_REPLAY = false;
		replayIndex = 0;
		totalFrames = 0;
	}
}
