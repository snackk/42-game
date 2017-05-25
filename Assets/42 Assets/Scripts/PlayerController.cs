﻿using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    //Player state variables
    private float _maxSpeed = 2;
    private float _life = 100;
    private float _playerJumpForce = 2;
    private bool _isDoubleJumpAble = false;

    public bool _canMoveFreely = false;
    public bool _isBlock = false;

    private bool _playerFaceRight = true;
    private bool _playerJump = false;

    private bool _isPlayerGrounded = true;
    private int _amountJump = 0;

    //Layers to check collisions
    private const float _groundedRadius = .015f;

    public LayerMask _whatIsGround;
    public LayerMask _whatIsInteractable;

    private Transform _playerCeilingCheck;
    private Transform _playerGroundCheck;

    //Player Animations
    private Rigidbody2D _playerRB;
    private Animator _playerAnim;

    //Interactions
    private List<IInteractable> _interactables = new List<IInteractable>();
    private PlayerMovement _playerMovement;


    public Rigidbody2D getPlayerRB
    {
        get { return _playerRB; }
    }

    public Animator getPlayerAnim
    {
        get { return _playerAnim; }
    }

    // Use this for initialization
    void Start () {
        _playerCeilingCheck = transform.Find("CeilingCheck"); 
        _playerGroundCheck = transform.Find("GroundCheck");

        _playerRB = GetComponent<Rigidbody2D>();
        _playerAnim = GetComponent<Animator>();

        _playerMovement = PlayerMovement.getInstance();
    }

    private void Update()
    {
     
    }

    // FixedUpdate is more acurate than Update 
    void FixedUpdate ()
    {
        checkGroundColision();
        checkForInteraction();
        
        movePlayer();
    }

    private void movePlayer()
    {
        float hMove = _playerMovement.horizontalMove;
        float vMove = _playerMovement.verticalMove;
        if (!_playerJump)
            _playerJump = _playerMovement.wannaJump;

        if (_isBlock)
        {
            if (_playerFaceRight)
                flipSide();
            _playerAnim.SetFloat("speed", 0);
        }

        if (_interactables.Count > 0)
        {
            if (hMove != 0 || vMove != 0 || _playerJump)
            {
                int result = 0;
                foreach (IInteractable i in _interactables)
                {
                    result = i.Interact();
                }
                if (result == 1)
                    _interactables = new List<IInteractable>();
            }
        }
        else
        {
            if (!_canMoveFreely)
            {
                if (Mathf.Abs(hMove) == 0)
                    hMove = _playerMovement.verticalMove;
                hMove = Mathf.Abs(hMove);
            }
            else handleJump();

            _playerRB.velocity = new Vector2(hMove * _maxSpeed, _playerRB.velocity.y);
            _playerAnim.SetFloat("speed", Mathf.Abs(hMove));

            if (hMove > 0 && !_playerFaceRight)
                flipSide();
            else if (hMove < 0 && _playerFaceRight)
                flipSide();
        }
    }

    private void handleJump()
    {
        if (_playerJump)
        {
            if (_amountJump == 0)
            {
                _playerRB.velocity = new Vector2(0f, _playerJumpForce);
                _playerAnim.SetBool("jump", true);
                _isPlayerGrounded = false;

                _amountJump++;
            }
            else
            {
                if (_amountJump == 1 && _isDoubleJumpAble)
                {
                    _playerRB.velocity = new Vector2(0f, _playerJumpForce);

                    _amountJump++;
                }
            }
        } else _playerAnim.SetBool("jump", false);

        _playerJump = false;
    }

    private void flipSide()
    {
        _playerFaceRight = !_playerFaceRight;
        Vector3 scaleNeg = transform.localScale;
        scaleNeg.x *= -1;
        transform.localScale = scaleNeg;
    }

    private void checkGroundColision()
    {
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_playerGroundCheck.position, _groundedRadius, _whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                _isPlayerGrounded = true;
                _amountJump = 0;

            }
        }
    }

    private void checkForInteraction()
    {
        if (_interactables.Count == 0 && _isBlock)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_playerGroundCheck.position, _groundedRadius, _whatIsInteractable);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.CompareTag("Interactable"))
                {
                    _interactables.Add(colliders[i].GetComponent<IInteractable>());
                }
            }
        }
    }
}