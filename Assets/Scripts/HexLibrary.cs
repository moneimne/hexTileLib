using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexLibrary : MonoBehaviour {

	public GameObject hexTile;
	public Camera debugCamera;

	public int rows = 7;
	public int cols = 7;

	// The currently selected tile
	Tile selected = null;

	int minRow;
	int minCol;

	// The board with offset coordinates
	private Dictionary<KeyValuePair<int, int>, Tile> gridMap = null; 

	// Use this for initialization
	void Start () {
		Debug.Log ("Borf");
	}

	public void Initialize() {
		minRow = rows / -2;
		minCol = cols / -2;

		GenerateGrid (cols, rows);
	}

	void TestNeighbor(int col, int row) {
		KeyValuePair<int, int> k = new KeyValuePair<int, int> (col, row);

		if (!gridMap.ContainsKey(k)) {
			return;
		}

		Tile current = gridMap [k];
		current.Unhighlight ();
		for (int direction = 0; direction < 6; ++direction) {
			Tile neighbor = current.neighbors [direction];
			if (neighbor) {
				neighbor.Unhighlight ();
			}

		}
	}

	// creates a hexagonal grid with specified number of rows and columns (to be changed later)
	Quaternion scratchQuaternion = new Quaternion();
	void GenerateGrid(int rows, int cols) {

		gridMap = new Dictionary<KeyValuePair<int, int>, Tile> ();

		if (hexTile == null) {
			Debug.Log ("You fool! You forgot to attach the hex generator!");
			return;
		}

		float length = 1.0f;
		float height = 2.0f * length;
		float width = Mathf.Sqrt (3.0f) * length;

		// Generate grid
		for (int col = minCol; col < minCol + cols; ++col) {
			for (int row = minRow; row < minRow + rows; ++row) {
				Vector3 centroid = new Vector3 (
					                   1.0f + 1.5f * col, 
					                   Random.Range (-1.0f, -0.1f), 
					                   width * row + ((col & 1) == 0 ? width * 0.5f : 0.0f)
				                   );

				// Generate hex tile
				Tile nextHex = ((GameObject)Instantiate (hexTile, centroid, scratchQuaternion)).GetComponent<Tile> ();
				nextHex.Initialize (Tile.Element.Blank, col, row);

				gridMap.Add (new KeyValuePair<int, int> (col, row), nextHex);
			}
		}

		// Indicies for neighboring cells in offset coordinates
		Vector2[,] directions = new Vector2[2, 6] { { new Vector2 (0, -1), new Vector2 (1, -1), new Vector2 (1, 0),
				new Vector2 (0, 1), new Vector2 (-1, 0), new Vector2 (-1, -1)
			}, { new Vector2 (0, -1), new Vector2 (1, 0), new Vector2 (1, 1),
				new Vector2 (0, 1), new Vector2 (-1, 1), new Vector2 (-1, 0)
			}
		};

		// Populate tile neighbors
		for (int col = minCol; col < minCol + cols; ++col) {
			for (int row = minRow; row < minRow + rows; ++row) {
				KeyValuePair<int, int> k = new KeyValuePair<int, int> (col, row);
				Tile current = gridMap [k];

				int parity = (col+1) & 1;
				for (int direction = 0; direction < 6; ++direction) {
					Vector2 offsets = directions [parity, direction];
					int x = col + (int)offsets.x;
					int y = row + (int)offsets.y;
					k = new KeyValuePair<int, int> (x, y);
					current.neighbors [direction] = null;
					if (gridMap.ContainsKey(k)) {
						current.neighbors [direction] = gridMap[k];
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
		if (((int)x & 1) == 1)
			y++;

		if (u > 1.0f / 3.0f) {
			// definitely inside one hexagon
			Vector2 vec = new Vector2 (x, (((int)y & 1) == 0 ? y / 2 : (y - 1) / 2));
			return vec;

		} else {
			// two possible hexagons

			if (((int)y & 1) == 1) {
				if (v > (3.0f * u)) {
					// in a previous col
					return new Vector2(x - 1, ((int)x & 1) == 0 ? (y + 1) / 2 : (y - 1) / 2);
				} else {
					return new Vector2 (x, (y - 1) / 2);
				}
			} else {
				if (v < (1.0f - 3.0f * u)) {
					// in a previous col
					return new Vector2 (x - 1, ((int)x & 1) == 0 ? y / 2 : (y - 2) / 2);
				} else {
					return new Vector2 (x, y / 2);
				}
			}
		}

	}

	/*
	 * Input: grid index in offset coordinates
	 * 
	 * Output: grid index in axial coordinates
	 * 
	 */
	Vector2 OffsetToAxial (Vector2 offset)
	{
		int x = (int)offset.x;
		int y = (int)offset.y;
		y = y - (x + y & 1) / 2;
		return new Vector2(x, y);
	}

	private struct Intersection {
		public float distance;
		public bool hit;
		public Vector3 point;

		public Intersection(float d, bool h, Vector3 p) {
			distance = d;
			hit = h;
			point = p;
		}
	}

	/*
	 * Input: ray origin, ray direction, plane normal, point on plane 
	 * 
	 * Output: point of intersection if there is one
	 */
	Intersection PlaneRayCast(Ray ray, Vector3 normal, Vector3 p0) {
		if (Vector3.Dot (ray.origin, normal) < 0.0001) {
			return new Intersection(-1, false, new Vector3()); // parallel
		}
		float t = Vector3.Dot(p0 - ray.origin, normal) / Vector3.Dot(ray.direction, normal);
		Vector3 p = ray.origin + t * ray.direction;
		return new Intersection (t, true, p);
	}

	/*
	 * Input: player mouse position (Input.mousePosition) and player's camera reference
	 *
	 * Output: a hex if it's under the cursor, null otherwise
	 *
	 */
	public Tile MouseSelectHex(Vector3 mouseCoords, Camera cam) {
		// convert mouse screen space into world space
		Ray r = cam.ScreenPointToRay(mouseCoords);
		// raycast to planes from top downward
		Intersection i = PlaneRayCast(r, new Vector3(0, 1, 0), new Vector3(0, 0, 0));
		if (!i.hit) {
			return null;
		}
		// convert to offset
		Vector2 hex = WorldToOffset(i.point);
		KeyValuePair<int, int> k = new KeyValuePair<int, int> ((int)hex.x, (int)hex.y);
		// convert to axial
		//hex = OffsetToAxial(hex);
		// see if hex exists here
		//GameObject g = gridMap[(int)hex.x, (int)hex.y];

		if (gridMap.ContainsKey(k)) {
			return gridMap[k];
		}
		return null;
	}

	/*
	 * Path finding: given a source tile and a target tile
	 * generate a path between them
	 */
	public void astar(Tile source, Tile destination) {
		List<Tile> path = new List<Tile> ();

		Stack<Tile> stack = new Stack<Tile> ();

		Tile current = source;
		while (current != destination) {

			// Push all neighboring nodes
			for (int i = 0; i < 6; ++i) {
				Tile neighbor = current.neighbors [i];

				// Check acessibility
				if (neighbor) {
					stack.Push (neighbor);
				}
				current = stack.Pop ();
			}
		}
	}

	public Dictionary<KeyValuePair<int, int>, SearchTileNode> generateSearchNodes() {
		Dictionary<KeyValuePair<int, int>, SearchTileNode> searchNodes = new Dictionary<KeyValuePair<int, int>, SearchTileNode> ();

		// Populate tile neighbors
		for (int col = minCol; col < minCol + cols; ++col) {
			for (int row = minRow; row < minRow + rows; ++row) {
				KeyValuePair<int, int> k = new KeyValuePair<int, int> (col, row);
				Tile tile = gridMap [k];

				SearchTileNode node = new SearchTileNode ();
				node.Initialize (tile);

				searchNodes.Add (new KeyValuePair<int, int> (col, row), node);
			}
		}
		return searchNodes;
	}

	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown (KeyCode.A)) {
			TestNeighbor (2, 2);
		}
	}
}
