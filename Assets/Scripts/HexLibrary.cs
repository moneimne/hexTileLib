using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexLibrary : MonoBehaviour {

	public GameObject hex;
	private GameObject[,] gridMap = null; // the map in hex coordinates

	// Use this for initialization
	void Start () {
		Debug.Log ("Borf");
		gridMap = new GameObject[7, 7];
		GenerateGrid (7, 7, -3, -3);

	}

	// creates a hexagonal grid with specified number of rows and columns (to be changed later)
	void GenerateGrid(int rows, int cols, int minRow, int minCol) {
		if (hex == null)
			return;

		float length = 1.0f;
		float height = 2.0f * length;
		float width = Mathf.Sqrt (3.0f) * length;


		for (int col = minCol; col < minCol + cols; col++) {
			for (int row = minRow; row < minRow + rows; row++) {
				Vector3 centroid = new Vector3 (
					1.0f + 1.5f * col, 
					Random.Range(0.0f, 1.0f), 
					width * row + ((col & 1) == 0 ? width * 0.5f : 0.0f)
				);
				GameObject nextHex = (GameObject) Instantiate(hex, centroid, new Quaternion());
				Vector2 axialIndex = OffsetToAxial(new Vector2(row, col));
				Debug.Log (axialIndex.x + ", " + axialIndex.y); 
				gridMap [(int) axialIndex.x - minRow, (int) axialIndex.y - minCol] = nextHex;
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
		
	}
}
