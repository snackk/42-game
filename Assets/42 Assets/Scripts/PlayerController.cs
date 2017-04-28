using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float _maxSpeed;
    public float _life;

    private bool _playerFaceRight = true;

    private Rigidbody2D _playerRB;
    private Animator _playerAnim;

	// Use this for initialization
	void Start () {
        _playerRB = GetComponent<Rigidbody2D>();
        _playerAnim = GetComponent<Animator>();
    }
	
	// FixedUpdate is more acurate than Update 
	void FixedUpdate () {
        float move = Input.GetAxis("Horizontal");

        //Set velocity by the movement
        _playerRB.velocity = new Vector2(move*_maxSpeed, _playerRB.velocity.y);
        
        //Adds speed to animator, animator then chooses the animation
        _playerAnim.SetFloat("speed", Mathf.Abs(move));

        if (move > 0 && !_playerFaceRight)
            flipSide();
        else {
            if (move < 0 && _playerFaceRight)
            {
                flipSide();
            }
        }
	}

    private void flipSide() {
        _playerFaceRight = !_playerFaceRight;
        Vector3 scaleNeg = transform.localScale;
        scaleNeg.x *= -1 ;
        transform.localScale = scaleNeg;    
    }
}
