using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBoom : MonoBehaviour, IInteractable {

    public string toShut;
    public string toTurnOn;

    public int Interact()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        GameObject.Find("Layer BG-near/New/" + toShut).GetComponent<Renderer>().enabled = false;
        GameObject.Find("Layer BG-near/Destroyed/" + toTurnOn).GetComponent<Renderer>().enabled = true;
        //GameObject.Find(toTurnOn).GetComponent<Renderer>().enabled = true;
        return 0;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
