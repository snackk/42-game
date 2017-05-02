using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
    
    public float _maxSpeed;
    public float _life;
    public float _playerJumpForce;
    public bool _isDoubleJumpAble;
    public bool _canMoveFreely;

    [SerializeField]
    public bool _isBlock = false;

    public LayerMask _whatIsGround;

    [SerializeField]
    private bool _playerFaceRight = true;
    [SerializeField]
    private bool _playerJump = false;
    [SerializeField]
    private bool _isPlayerGrounded = false;
    [SerializeField]
    [Range(0, 1)] private int _amountJump = 0;

    private Transform _playerCeilingCheck;
    private Transform _playerGroundCheck;

    private Rigidbody2D _playerRB;
    private Animator _playerAnim;

    const float _groundedRadius = 1.3f;

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
        _playerCeilingCheck = transform.Find("GroundCheck");
        _playerGroundCheck = transform.Find("CeilingCheck");

        _playerRB = GetComponent<Rigidbody2D>();
        _playerAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_playerJump)
        {
            // Read the jump input in Update so button presses aren't missed.
            _playerJump = CrossPlatformInputManager.GetButtonDown("Jump");
        } 
    }

    // FixedUpdate is more acurate than Update 
    void FixedUpdate () {
        checkGroundColision();
        if (!_isBlock) {
            movePlayer();
        } else _playerAnim.SetFloat("speed", 0);
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

    private void movePlayer() {
        float move = Input.GetAxis("Horizontal");

        if (!_canMoveFreely)
        {
            if(Mathf.Abs(move) == 0)
                move = Input.GetAxis("Vertical");
            move = Mathf.Abs(move);
        }

        //Set velocity by the movement
        _playerRB.velocity = new Vector2(move * _maxSpeed, _playerRB.velocity.y);

        //Adds speed to animator, animator then chooses the animation
        _playerAnim.SetFloat("speed", Mathf.Abs(move));

        if (move > 0 && !_playerFaceRight)
            flipSide();
        else if (move < 0 && _playerFaceRight)
                flipSide();

        if(_canMoveFreely)
            handleJump();
    }

    private void handleJump() {
        if (_playerJump && _isPlayerGrounded)
        {
            _playerRB.AddForce(new Vector2(0f, _playerJumpForce));
            _playerAnim.SetBool("jump", true);
            _isPlayerGrounded = false;
        }
        else
        {
            if (_playerJump && !_isPlayerGrounded)
            {
                if (_amountJump < 1 && _isDoubleJumpAble)
                {
                    _playerRB.AddForce(new Vector2(0f, _playerJumpForce));
                    _amountJump++;
                }
            }
            else _playerAnim.SetBool("jump", false);
        }

        _playerJump = false;
    }

    private void flipSide()
    {
        _playerFaceRight = !_playerFaceRight;
        Vector3 scaleNeg = transform.localScale;
        scaleNeg.x *= -1;
        transform.localScale = scaleNeg;
    }
}
