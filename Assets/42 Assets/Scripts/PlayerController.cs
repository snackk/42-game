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
    public LayerMask _whatIsInteractable;

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

    const float _groundedRadius = .2f;

    public Rigidbody2D getPlayerRB
    {
        get { return _playerRB; }
    }

    public Animator getPlayerAnim
    {
        get { return _playerAnim; }
    }

    private List<IInteractable> _interactables = new List<IInteractable>();
    private PlayerMovement _playerMovement;

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
        foreach (IInteractable i in _interactables)
        {
            i.Interact(this);
        }
        //_interactables = new List<IInteractable>();

        if (!_playerJump && !_isBlock)
        {
            // Read the jump input in Update so button presses aren't missed.
            _playerJump = _playerMovement.wannaJump;
        }
    }

    // FixedUpdate is more acurate than Update 
    void FixedUpdate () {

        if (!_isBlock) 
            movePlayer();
        else _playerAnim.SetFloat("speed", 0);

        checkGroundColision();
        checkForInteraction();
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
        if (_interactables.Count == 0)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_playerGroundCheck.position, _groundedRadius, _whatIsInteractable);
            for (int i = 0; i < colliders.Length; i++)
            {
                //MOVE THIS TO ELSEWHERE
                if (colliders[i].gameObject.CompareTag("Interactable"))
                {
                    Debug.Log("cona");
                    _interactables.Add(colliders[i].GetComponent<IInteractable>());
                }
            }
        }
    }

    private void movePlayer() {
        float move = _playerMovement.horizontalMove;

        if (!_canMoveFreely)
        {
            if (Mathf.Abs(move) == 0)
                move = _playerMovement.verticalMove;
            move = Mathf.Abs(move);
        } else handleJump();

        //Set velocity by the movement
        _playerRB.velocity = new Vector2(move * _maxSpeed, _playerRB.velocity.y);

        //Adds speed to animator, animator then chooses the animation
        _playerAnim.SetFloat("speed", Mathf.Abs(move));

        if (move > 0 && !_playerFaceRight)
            flipSide();
        else if (move < 0 && _playerFaceRight)
                flipSide(); 
    }

    private void handleJump() {
        if (_playerJump && _isPlayerGrounded)
        {
            _playerRB.velocity = new Vector2(0f, _playerJumpForce);
            _playerAnim.SetBool("jump", true);
            _isPlayerGrounded = false;
        }
        else
        {
            if (_playerJump && !_isPlayerGrounded)
            {
                if (_amountJump < 1 && _isDoubleJumpAble)
                {
                    _playerRB.velocity = new Vector2(0f, _playerJumpForce);
                    _amountJump++;
                }
            }
            else _playerAnim.SetBool("jump", false);
        }

        _playerJump = false;
    }

    public void flipSide()
    {
        _playerFaceRight = !_playerFaceRight;
        Vector3 scaleNeg = transform.localScale;
        scaleNeg.x *= -1;
        transform.localScale = scaleNeg;
    }
}
