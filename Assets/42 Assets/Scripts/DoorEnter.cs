﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorEnter : MonoBehaviour, IInteractable {

	// Use this for initialization
	void Start () {
		
	}

	public int Interact(){
		SceneManager.LoadScene ("Laboratory");
		return 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
