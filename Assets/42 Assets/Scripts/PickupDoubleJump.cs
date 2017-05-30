using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDoubleJump : MonoBehaviour, IInteractable {

	public int Interact() {
		GameObject.Find("Player").GetComponent<PlayerController>().isDoubleJumpAble = true;
		gameObject.GetComponent<Renderer> ().enabled = false;

		return 0;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
