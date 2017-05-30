using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcRequiresItem : MonoBehaviour {

	private bool hasItem = false;

	public void caughtItem(){
		gameObject.GetComponent<Collider2D> ().enabled = false;
		GameObject.Find("BlockNPC/ChatBubble").GetComponent<Renderer>().enabled = false;
		hasItem = true;
	}

	public bool checkHasItem(){
		return hasItem;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
