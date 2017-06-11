using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject board;
	public GameObject player1;
	public GameObject player2;

	// Use this for initialization
	void Start () {
		board.GetComponent<HexLibrary> ().Initialize ();
		player1.GetComponent<Player> ().Initialize (board);
		//player2.GetComponent<Player> ().Initialize (board);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
