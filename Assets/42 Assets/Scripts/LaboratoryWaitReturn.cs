using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaboratoryWaitReturn : MonoBehaviour {
    private Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}

    void FixedUpdate()
    {
        var currentBaseState = anim.GetCurrentAnimatorStateInfo(0);

        if (currentBaseState.IsName("End"))
        {
            SceneManager.LoadScene("OutdoorsMachine");
        }
    }
}
