using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public enum Element {Blank, Fire, Water, Grass}

	public Tile[] neighbors;
	public Color highlightColor = Color.cyan;
	public Element type = Element.Blank;

	int row = 0;
	int col = 0;
	Renderer renderer;

	// Use this for initialization
	void Start () {
		renderer = gameObject.GetComponent<Renderer> ();
		Highlight ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InitializeTile(Element element, int rowIndex, int colIndex) {
		row = rowIndex;
		col = colIndex;
		type = element;
		neighbors = new Tile[6];
	}

	public void Highlight() {
		renderer.material.color = highlightColor;
	}

	public void Unhighlight() {
		renderer.material.color = Color.white;
	}
}
