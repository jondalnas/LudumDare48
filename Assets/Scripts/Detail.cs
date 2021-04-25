using UnityEngine;
using UnityEngine.Tilemaps;

public class Detail : MonoBehaviour {
	Tilemap tilemap;
	void Start() {
		tilemap = GetComponent<Tilemap>();
	}
	public void ChangeColor(Color color) {
		foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin) {
			tilemap.RemoveTileFlags(pos, TileFlags.LockColor);
			tilemap.SetColor(pos, color);
			tilemap.SetTileFlags(pos, TileFlags.LockColor);
		}
	}
}
