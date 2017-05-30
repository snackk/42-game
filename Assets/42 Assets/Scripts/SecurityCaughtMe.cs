using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecurityCaughtMe : MonoBehaviour {

    public Transform _player = null;
    private bool entered = false;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if(entered)
            transform.position = Vector3.Lerp(transform.position, _player.position, Time.deltaTime);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        entered = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        SceneManager.LoadScene("Laboratory");
    }
}
