using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaboratoryWaitReturn : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Thread oThread = new Thread(new ThreadStart(() => {
			Thread.Sleep(5000);

			SceneManager.LoadScene ("Laboratory");
		}));

		// Start the thread
		oThread.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
