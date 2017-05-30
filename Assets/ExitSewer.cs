using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitSewer : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (GameObject.Find ("BlockNPC").GetComponent<NpcRequiresItem> ().checkHasItem ()) {
			SceneManager.LoadScene ("");
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
