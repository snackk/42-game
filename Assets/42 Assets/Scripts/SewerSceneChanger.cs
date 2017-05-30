using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewerSceneChanger : MonoBehaviour, IInteractable {

	// Use this for initialization
	void Start () {
		
	}

	public int Interact(){
		var cornerDisplay = GameObject.Find ("CornerDisplay");
		cornerDisplay.GetComponent<CornerDisplayHandler>().TurnScreenRed();
		return 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
