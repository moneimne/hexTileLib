using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexLibrary : MonoBehaviour {

	public GameObject hexTile;
	public int rows = 7;
	public int cols = 7;

	private Tile[,] gridMap = null; // the map in hex coordinates

	// Use this for initialization
	void Start () {
		Debug.Log ("Borf");
		GenerateGrid (cols, rows);
	}

	bool validInidices(int row, int col) {
		return ((row > -1 && row < rows) && (col > -1 && col < cols));
	}

	void testNeighbor(int x, int y) {
		Tile current = gridMap [x, y];
		current.Unhighlight ();
		foreach (Tile n in current.neighbors) {
			n.Unhighlight ();
		}
	}

	// creates a hexagonal grid with specified number of rows and columns (to be changed later)
	Quaternion scratchQuaternion = new Quaternion();
	void GenerateGrid(int rows, int cols) {

		gridMap = new Tile[cols, rows];

		if (hexTile == null) {
			Debug.Log ("You fool! You forgot to attach the hex generator!");
			return;
		}

		float length = 1.0f;
		float height = 2.0f * length;
		float width = Mathf.Sqrt (3.0f) * length;

		int minRow = rows / 2;
		int minCol = cols / 2;

		// Generate grid
		for (int col = minCol; col < minCol + cols; ++col) {
			for (int row = minRow; row < minRow + rows; ++row) {
				Vector3 centroid = new Vector3 (
					1.0f + 1.5f * col, 
					Random.Range(0.0f, 1.0f), 
					width * row + ((col & 1) == 0 ? width * 0.5f : 0.0f)
				);

				// Generate hex tile
				Tile nextHex = ((GameObject) Instantiate(hexTile, centroid, scratchQuaternion)).GetComponent<Tile>();
				nextHex.InitializeTile (Tile.Element.Blank, col, row);

				// Add to our board
				gridMap [col - minCol, row - minRow] = nextHex;
			}
		}

		// Indicies for neighboring cells in offset coordinates
		Vector2[,] directions = new Vector2[2, 6] {
			{ new Vector2(0,  -1), new Vector2(1, -1), new Vector2( 1, 0),
				new Vector2(0, 1), new Vector2(-1,  0), new Vector2( -1, -1) },
			{ new Vector2(0, -1), new Vector2(1,  0), new Vector2( 1, 1),
				new Vector2(0,  1), new Vector2(-1, 1), new Vector2( -1, 0) }
		};

		// Populate tile neighbors
		for (int col = 0; col < cols; ++col) {
			for (int row = 0; row < rows; ++row) {
				Tile current = gridMap [row, col];

				int parity = col & 1;
				for (int direction = 0; direction < 6; ++direction) {
					Vector2 offsets = directions [parity, direction];
					int x = col + (int)offsets.x;
					int y = row + (int)offsets.y;
					if (validInidices(x, y)) {
						current.neighbors.Add (gridMap[x, y]);
					}
				}
			}
		}
				
	}

	/*
	 * Input: Vector3 in worldspace
	 * 
	 * Output: Offset Grid index of a hex that could be here
	 *
	 */
	Vector2 WorldToOffset(Vector3 wcoords) {
		float w = 1.5f;
		float h = Mathf.Sqrt (3) * 0.5f;
		// transform coordinates to grid of w, h rects
		float x = Mathf.Floor (wcoords.x / w);
		float y = Mathf.Floor (wcoords.z / h);
		// normalized remainder
		float u = (wcoords.x - x * w) / w;
		float v = (wcoords.z - y * h) / h;

		if (u > 1.0f / 3.0f) {
			// definitely inside one hexagon
			return new Vector2 (x, (((int)y & 1) == 0 ? y : y - 1));
		} else {
			// two possible hexagons
			if (((int)y & 1) == 1) {
				if (v > (3.0f * u)) {
					return new Vector2(x - 1, (y + 1) / 2);
				} else {
					return new Vector2 (x, (y - 1) / 2);
				}
			} else {
				if (v < (1.0f - 3.0f * u)) {
					return new Vector2 (x - 1, (y - 2) / 2);
				} else {
					return new Vector2 (x, y / 2);
				}
			}
		}

	}

	Vector2 OffsetToAxial (Vector2 offset)
	{
		int x = (int)offset.x;
		int y = (int)offset.y;
		y = y - (x + y & 1) / 2;
		return new Vector2(x, y);
	}


	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.A)) {
			testNeighbor (2, 2);
		}
	}
}
