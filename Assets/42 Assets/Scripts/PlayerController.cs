using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    //Player state variables
    public float _maxSpeed;
    public float _life;
    public float _playerJumpForce;
    public bool _isDoubleJumpAble = false;
    public bool _canMoveFreely = false;
    public bool _isBlock = false;

    [SerializeField]
    private bool _playerFaceRight = true;
    [SerializeField]
    private bool _playerJump = false;

    private bool _isPlayerGrounded = true;
   
    private int _amountJump = 0;

    //Layers to check collisions
    [SerializeField]
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
        List<IInteractable> toDelete = new List<IInteractable>();
        foreach (IInteractable i in _interactables)
        {
            int result = i.Interact(this);
            if (result == 1)
                toDelete.Add(i);
        }

        foreach (IInteractable i in toDelete) { //If interaction returned 1, then i wont need it anymore
            _interactables.Remove(i);
        }        
    }

    // FixedUpdate is more acurate than Update 
    void FixedUpdate ()
    {
        checkGroundColision();
        checkForInteraction();

        if (!_isBlock)
            movePlayer();
        else {
            if (_playerFaceRight)
                flipSide();
            _playerAnim.SetFloat("speed", 0);
        }
    }

    private void movePlayer()
    {
        float move = _playerMovement.horizontalMove;

        if (!_canMoveFreely)
        {
            if (Mathf.Abs(move) == 0)
                move = _playerMovement.verticalMove;
            move = Mathf.Abs(move);
        } else handleJump();

        _playerRB.velocity = new Vector2(move * _maxSpeed, _playerRB.velocity.y);
        _playerAnim.SetFloat("speed", Mathf.Abs(move));

        if (move > 0 && !_playerFaceRight)
            flipSide();
        else if (move < 0 && _playerFaceRight)
                flipSide(); 
    }

    private void handleJump()
    {
        if (!_playerJump)
        {
            // Read the jump input in Update so button presses aren't missed.
            _playerJump = _playerMovement.wannaJump;
        }

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
        if (_interactables.Count == 0)
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