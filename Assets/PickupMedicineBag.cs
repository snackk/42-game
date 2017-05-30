using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMedicineBag : MonoBehaviour, IInteractable {

	public int Interact(){
		gameObject.GetComponent<Renderer> ().enabled = false;
		GameObject.Find("BlockNPC").GetComponent<NpcRequiresItem>().caughtItem();
		return 0;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
