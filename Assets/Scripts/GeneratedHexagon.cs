using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedHexagon : MonoBehaviour {

	public Material meshMat;
	private static Mesh mesh;

	public void Generate () {
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		//Vector3 p = gameObject.GetComponent<Transform> ().position;

		if (mesh != null) {
			gameObject.GetComponent<MeshFilter> ().mesh = mesh;
			gameObject.GetComponent<MeshRenderer> ().material = meshMat;
			return;
		} else 
			mesh = GetComponent<MeshFilter>().mesh;


		mesh.Clear();

		List<Vector3> vertices = new List<Vector3> ();
		List<Vector3> normals = new List<Vector3> ();
		List<int> triangles = new List<int> ();


		//hexagon top
		Vector3 up = new Vector3(0, 1, 0);
		vertices.Add(new Vector3(0, 0, 0));
		normals.Add (new Vector3(up.x, up.y, up.z));

		for (int i = 0; i < 6; i++) {
			vertices.Add(new Vector3(Mathf.Sin(Mathf.PI / 3.0f * (i + 0.5f)), 0.0f, Mathf.Cos(Mathf.PI / 3.0f * (i + 0.5f))));
			normals.Add(new Vector3(up.x, up.y, up.z));
			int l = i + 1;
			int r = i + 2;
			r = r > 6 ? 1 : r;
			triangles.Add (0);
			triangles.Add (l);
			triangles.Add (r);
		}

		int sideIdx = 7;

		// hexagon sides
		for (int i = 0; i < 6; i++) {
			vertices.Add(new Vector3(Mathf.Sin(Mathf.PI / 3.0f * (i + 0.5f)), 0.0f, Mathf.Cos(Mathf.PI / 3.0f * (i + 0.5f))));
			vertices.Add(new Vector3(Mathf.Sin(Mathf.PI / 3.0f * (i + 0.5f)), -6.0f, Mathf.Cos(Mathf.PI / 3.0f * (i + 0.5f))));
			vertices.Add(new Vector3(Mathf.Sin(Mathf.PI / 3.0f * (i + 1.5f)), 0.0f, Mathf.Cos(Mathf.PI / 3.0f * (i + 1.5f))));
			vertices.Add(new Vector3(Mathf.Sin(Mathf.PI / 3.0f * (i + 1.5f)), -6.0f, Mathf.Cos(Mathf.PI / 3.0f * (i + 1.5f))));
			Vector3 norm = new Vector3 (Mathf.Sin (Mathf.PI / 3.0f * (i + 1.0f)), 0.0f, Mathf.Cos (Mathf.PI / 3.0f * (i + 1.0f)));
			for (int j = 0; j < 4; j++) {
				normals.Add(new Vector3(norm.x, norm.y, norm.z));
			}
			triangles.Add (sideIdx + 1);
			triangles.Add (sideIdx + 2);
			triangles.Add (sideIdx);
			triangles.Add (sideIdx + 3);
			triangles.Add (sideIdx + 2);
			triangles.Add (sideIdx + 1);
			sideIdx += 4;
		}

		mesh.SetVertices (vertices);
		mesh.SetNormals (normals);
		mesh.SetTriangles (triangles, 0);

		gameObject.GetComponent<MeshRenderer> ().material = meshMat;
	}

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
