using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public Camera camera;

	GameObject board;
	HexLibrary hexLibrary;

	Tile selected;
	List<Tile> path;
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

	// Use this for initialization
	void Start () {}

	public void Initialize(GameObject gameBoard) {
		board = gameBoard;
		hexLibrary = board.GetComponent<HexLibrary> ();
		searchNodes = hexLibrary.generateSearchNodes ();
	}

	// Update is called once per frame
	void Update () {
		// Select a tile
		if (Input.GetMouseButton (0)) {
			SelectTile ();
		}
	}
}
