using UnityEngine;

public class DoorReplay : MonoBehaviour, IReplayable {
	public object[] CollectData() {
		return new object[] { transform.rotation };
	}

	public void ReplayData(int frame, object[] data) {
		transform.rotation = (Quaternion)data[0];
	}
}
