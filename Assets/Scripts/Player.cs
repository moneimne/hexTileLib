using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public Camera camera;

	GameObject board;
	HexLibrary hexLibrary;

	Tile selected;
	List<Tile> path = null;
	Tile goal;

	// The board with offset coordinates
	private Dictionary<KeyValuePair<int, int>, SearchTileNode> searchNodes;


	private void SelectTile() {
		Tile g = hexLibrary.MouseSelectHex (Input.mousePosition, camera);
		if (g != null) {
			if (selected) {
				selected.Unhighlight ();
			}
			selected = g;
			selected.Highlight();
		}
	}

	private void SelectPath() {
		Tile g = hexLibrary.MouseSelectHex (Input.mousePosition, camera);
		if (g != null) {
			if (path != null) {
				hexLibrary.UnhighlightPath (path);
			}
			path = hexLibrary.TestPath(g, hexLibrary.GetTile(0, 0));
		}
	}

	// Use this for initialization
	void Start () {}

	public void Initialize(GameObject gameBoard) {
		board = gameBoard;
		hexLibrary = board.GetComponent<HexLibrary> ();
	}

	// Update is called once per frame
	void Update () {
		// Select a tile
		if (Input.GetMouseButton (0)) {
			SelectPath ();
		}
	}
}
