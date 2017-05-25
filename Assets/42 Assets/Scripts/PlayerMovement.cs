using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour {

    private static PlayerMovement _instance;

    public float horizontalMove { get
        {
        return Input.GetAxis("Horizontal");
        }
    }

    public float verticalMove
    {
        get
        {
            return Input.GetAxis("Vertical");
        }
    }

    public bool wannaJump
    {
        get
        {
            return CrossPlatformInputManager.GetButtonDown("Jump");
        }
    }

    protected PlayerMovement() {

    }

    public static PlayerMovement getInstance() {
        if (_instance == null)
            _instance = new PlayerMovement();
        return _instance;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
