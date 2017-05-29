using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
    
    //Player state variables (Depends on the player side)
    private float _maxSpeed;
    private float _accel;
    private float _daccel;
    private float _life;
    private float _playerJumpForce;
    private bool _isDoubleJumpAble;

    private float _actualSpeed = 0;

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
            _maxSpeed = 2.5f;
            _accel = 6.0f;
            _daccel = 6.0f;
            _life = 100;
            _playerJumpForce = 3;
            _isDoubleJumpAble = true;
        }
        else
        {
            _maxSpeed = 2.5f;
            _accel = 4.0f;
            _daccel = 3.5f;
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

        checkGroundColision();
        checkInteraction();

        if (!_playerJump && !_isBlock)
            _playerJump = CrossPlatformInputManager.GetButtonDown("Jump");
    }

    // FixedUpdate is more acurate than Update 
    void FixedUpdate ()
    {
        handlePlayer();
    }

    private void handlePlayer() {
        if (_isBlock || !_canMoveFreely)
            handlePlayerNonFreely();
        else playerMoveFreely();

        handlePlayerInteraction();
    }

    private void handlePlayerNonFreely() {

        if (_isBlock)
        {
            if (_playerFaceRight)
                flipSide();
            _playerAnim.SetFloat("speed", 0);
        }

        if (!_canMoveFreely && !_isBlock)
        {
            if ((Input.GetKey(KeyCode.A) || 
                Input.GetKey(KeyCode.W) || 
                Input.GetKey(KeyCode.S) || 
                Input.GetKey(KeyCode.D) || 
                Input.GetKey(KeyCode.Space)) && (Mathf.Abs(_actualSpeed) < _maxSpeed))
                    _actualSpeed += _accel * Time.deltaTime;
                else
                {
                    if (_actualSpeed > _daccel * Time.deltaTime)
                        _actualSpeed = _actualSpeed - _daccel * Time.deltaTime;
                    else if (_actualSpeed < -_daccel * Time.deltaTime)
                        _actualSpeed = _actualSpeed + _daccel * Time.deltaTime;
                    else _actualSpeed = 0;
                }

                _playerRB.velocity = new Vector2(_actualSpeed, _playerRB.velocity.y);
                _playerAnim.SetFloat("speed", Mathf.Abs(_actualSpeed));

                if (_actualSpeed > 0 && !_playerFaceRight)
                    flipSide();
                else if (_actualSpeed < 0 && _playerFaceRight)
                    flipSide();
        }
    }

    private void playerMoveFreely() {

        if ((Input.GetKey(KeyCode.A)) && (Mathf.Abs(_actualSpeed) < _maxSpeed))
            _actualSpeed -= _accel * Time.deltaTime;
        else if ((Input.GetKey(KeyCode.D)) && (Mathf.Abs(_actualSpeed) < _maxSpeed))
            _actualSpeed += _accel * Time.deltaTime;
        else {
            if (_actualSpeed > _daccel * Time.deltaTime)
                _actualSpeed = _actualSpeed - _daccel * Time.deltaTime;
            else if (_actualSpeed < -_daccel * Time.deltaTime)
                _actualSpeed = _actualSpeed + _daccel * Time.deltaTime;
            else _actualSpeed = 0;
        }

        _playerRB.velocity = new Vector2(_actualSpeed, _playerRB.velocity.y);
        _playerAnim.SetFloat("speed", Mathf.Abs(_actualSpeed));

        if (_actualSpeed > 0 && !_playerFaceRight)
            flipSide();
        else if (_actualSpeed < 0 && _playerFaceRight)
            flipSide();

        handleJump(_actualSpeed != 0);
    }

    private void handlePlayerInteraction() {
        if (_interactable != null && _playerAnim.GetFloat("speed") == 0)
        {
            if (Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.E))
            {      
                int result = 0;
                result = _interactable.Interact();

                if (result == 1)
                    _interactable = null;
            }
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

    private void checkInteraction()
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