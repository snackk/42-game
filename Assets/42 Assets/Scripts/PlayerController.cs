using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
    
    //Player state variables (Depends on the player side)
    private float _maxSpeed;
    private float _life;
    private float _playerJumpForce;
    private bool _isDoubleJumpAble;

    //Changes from scene to scene
    public bool _canMoveFreely = false;
    public bool _isBlock = false;   //To interact with player YOU MUST BLOCK HIM!
    
    //DO NOT TOUCH THESE
    private bool _playerFaceRight = true;
    private bool _playerJump = false;

    private bool _isPlayerGrounded = true;
    private int _amountJump = 0;

    //Layers to check collisions
    private const float _groundedRadius = .05f;

    public LayerMask _whatIsGround;
    public LayerMask _whatIsInteractable;

    private Transform _playerCeilingCheck;
    private Transform _playerGroundCheck;

    //Player Animations
    private Rigidbody2D _playerRB;
    private Animator _playerAnim;

    //Interactions
    private IInteractable _interactable = null;

    //Sprited to be loaded on runtime
    public Sprite _theGoodPlayerSprite;
    public Sprite _theBadPlayerSprite;

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

        LoadPlayerSide();
        RenderPlayerSprite();
    }

    private void LoadPlayerSide() {
        if (_playerAnim.GetBool("MachineSide"))
        {
            _maxSpeed = 2;
            _life = 100;
            _playerJumpForce = 3;
            _isDoubleJumpAble = true;
        }
        else
        {
            _maxSpeed = 1.5f;
            _life = 100;
            _playerJumpForce = 3;
            _isDoubleJumpAble = false;
        }
    }

    private void RenderPlayerSprite() {
        SpriteRenderer _playerSpriteRenderer = GetComponent<SpriteRenderer>();

        //_playerTheGoodSprites = Resources.LoadAll<Sprite>(@"TheGoodPlayer/");
        //_playerTheBadSprites = Resources.LoadAll<Sprite>(@"TheGoodPlayer/");
        if (_playerSpriteRenderer.sprite != null)
            return;

        if (_playerAnim.GetBool("MachineSide"))
            _playerSpriteRenderer.sprite = _theBadPlayerSprite;
        else _playerSpriteRenderer.sprite = _theGoodPlayerSprite;
    }

    private void Update()
    {
        RenderPlayerSprite();
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
        float hMove = Input.GetAxis("Horizontal");
        float vMove = Input.GetAxis("Vertical");
        if (!_playerJump && !_isBlock)
            _playerJump = CrossPlatformInputManager.GetButtonDown("Jump");

        if (_isBlock)
        {
            if (_playerFaceRight)
                flipSide();
            _playerAnim.SetFloat("speed", 0);
        }

        if (_interactable != null  && _playerAnim.GetFloat("speed") == 0)
        {
            if (Input.GetKey(KeyCode.E)) {
                int result = 0;
                result = _interactable.Interact();

                if (result == 1)
                    _interactable = null;
            }
        }
        else
        {
            if (!_canMoveFreely)
            {
                if (Mathf.Abs(hMove) == 0)
                    hMove = vMove;
                hMove = Mathf.Abs(hMove);
            }
            else handleJump(hMove != 0);

            _playerRB.velocity = new Vector2(hMove * _maxSpeed, _playerRB.velocity.y);
            _playerAnim.SetFloat("speed", Mathf.Abs(hMove));

            if (hMove > 0 && !_playerFaceRight)
                flipSide();
            else if (hMove < 0 && _playerFaceRight)
                flipSide();
        }
    }

    private void handleJump(bool isMoving)
    {
        if (_playerJump)
        {
            if (_amountJump == 0)
            {
                _playerRB.velocity = new Vector2(0f, _playerJumpForce);
                if(isMoving)
                    _playerAnim.SetBool("jump", true);
                _isPlayerGrounded = false;

                _amountJump++;
            }
            else
            {
                if (_amountJump == 1 && _isDoubleJumpAble)
                {
                    _playerRB.velocity = new Vector2(0f, _playerJumpForce);
                        if (isMoving)
                            _playerAnim.SetBool("jump", true);
                        _isPlayerGrounded = false;

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
        if (_interactable == null && _isBlock)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_playerGroundCheck.position, _groundedRadius, _whatIsInteractable);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.CompareTag("Interactable"))
                {
                    _interactable = colliders[i].GetComponent<IInteractable>();
                    break;
                }
            }
        }
    }
}