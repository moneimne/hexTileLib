using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is a wrapper around a tile which
// stores data used by the path-finding algorithm.
// The tile it refers to should not be changed
public class SearchTileNode {

	static int inf = 10000;

	Tile tile;

	int distance;
	Tile previousTile;

	public void Initialize (Tile referencedTile) {
		tile = referencedTile;
		Reset ();
	}

	void Reset () {
		distance = inf;
		previousTile = null;
	}
}
