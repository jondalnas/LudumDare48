
public interface IReplayable {
	public void ReplayData(int frame, object[] data);
	public object[] CollectData();
}
