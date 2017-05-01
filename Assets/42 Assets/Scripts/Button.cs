using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Button : MonoBehaviour {
	private GameObject screen;
	private Renderer screenRenderer;

	private int timesCalled = 0;
	private float timer = 0.0f;

	private object lockIsInteractable = new object();
	private bool isInteractable = true;

	private object lockScreenRenderer = new object();
	private bool reEnableScreenRenderer = false;
		
	// Use this for initialization
	void Start () {
		screen = this.gameObject.transform.GetChild (0).gameObject;
		screenRenderer = screen.GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		/*if (Input.anyKey) {
			Interact ();
		}*/

		lock (lockScreenRenderer) {
			if (reEnableScreenRenderer) {
				reEnableScreenRenderer = false;
				screenRenderer.enabled = true;
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		Debug.Log ("Object entered");
	}

	void OnTriggerExit2D (Collider2D other)
	{
		Debug.Log ("Object Left");
	}

	void Interact()
	{			
		lock (lockIsInteractable) {
			if (isInteractable) {
				isInteractable = false;
				timer = 0;
				if (timesCalled++ >= 7) {
					//TODO: change backgrounds and desk for a while. Stop screenrender permanently.
				} else {
					Debug.Log (timesCalled);
					screenRenderer.enabled = false;

					Thread oThread = new Thread (new ThreadStart (() => { 
						Thread.Sleep (5000);
						lock (lockIsInteractable) {
							isInteractable = true;
						}
						lock (lockScreenRenderer) {
							reEnableScreenRenderer = true;
						}
					}));

					// Start the thread
					oThread.Start ();
				}
			}
		}
	}
}
