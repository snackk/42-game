using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SewerSceneChanger : MonoBehaviour, IInteractable {

	// Use this for initialization
	void Start () {
		
	}

	public int Interact(){
		var cornerDisplay = GameObject.Find ("CornerDisplay");
		if (cornerDisplay.GetComponent<CornerDisplayHandler> ().TurnScreenRed ()) {
			SceneManager.LoadScene ("Sewers");
		}
		return 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
