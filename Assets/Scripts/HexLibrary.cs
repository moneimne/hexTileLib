using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexLibrary : MonoBehaviour {

	public GameObject hex;
	private GameObject[,] gridMap = null; // the map in hex coordinates
	private Dictionary<KeyValuePair<int, int>, GameObject> allHexes = null;
	public GameObject debugMark;
	public Camera debugCamera;

	// Use this for initialization
	void Start () {
		Debug.Log ("Borf");
		gridMap = new GameObject[7, 7];

		allHexes = new Dictionary<KeyValuePair<int, int>, GameObject> ();
		GenerateGrid (7, 7, -3, -3);

		if (debugMark == null) {
			debugMark = (GameObject) Instantiate(hex, new Vector3(0, 0, 0), new Quaternion());
		}
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
					Random.Range(-1.0f, -0.1f), 
					width * row + ((col & 1) == 0 ? width * 0.5f : 0.0f)
				);
				GameObject nextHex = (GameObject) Instantiate(hex, centroid, new Quaternion());
				//Vector2 axialIndex = OffsetToAxial(new Vector2(row, col));
				//Debug.Log (axialIndex.x + ", " + axialIndex.y); 
				allHexes.Add (new KeyValuePair<int, int> (col, row), nextHex);
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

		Debug.Log("(x, y) = (" + x + ", " + y + "); (u, v) = (" + u + "," + v + ")"); 

		if (u > 1.0f / 3.0f) {
			// definitely inside one hexagon
			Vector2 vec = new Vector2 (x, (((int)y & 1) == 0 ? y / 2 : (y - 1) / 2));
			Debug.Log("Interior " + vec.x + ", " + vec.y); 
			return vec;

		} else {
			// two possible hexagons
			if (((int)y & 1) == 0) {
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
	 * Input: player mouse position (Input.mousePosition) and camera reference
	 *
	 * Output: a hex if it's under the cursor, null otherwise
	 *
	 */
	private GameObject MouseSelectHex(Vector3 mouseCoords, Camera cam) {
		// convert mouse screen space into world space
		Ray r = cam.ScreenPointToRay(mouseCoords);
		// raycast to planes from top downward
		Intersection i = PlaneRayCast(r, new Vector3(0, 1, 0), new Vector3(0, 0, 0));
		if (!i.hit) {
			return null;
		}
		// convert to offset
		Vector2 hex = WorldToOffset(i.point);
		debugMark.transform.position = i.point;
		KeyValuePair<int, int> k = new KeyValuePair<int, int> ((int)hex.x, (int)hex.y);
		// convert to axial
		//hex = OffsetToAxial(hex);
		// see if hex exists here
		//GameObject g = gridMap[(int)hex.x, (int)hex.y];

		if (allHexes.ContainsKey(k)) {
			return allHexes[k];
		}
		return null;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0)) {
			GameObject g = MouseSelectHex (Input.mousePosition, debugCamera);
			if (g != null)
				debugMark.transform.position = g.transform.position;
		}
	}
}
