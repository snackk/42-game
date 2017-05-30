using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
    
    //Player state variables (Depends on the player side)
    private float _maxSpeed;
    private float _accel;
    private float _daccel;
    private float _life;
    public float playerJumpForce;
    public bool isDoubleJumpAble;
    public bool isMachineSide;


    private float _hActualSpeed = 0;
    private float _vActualSpeed = 0;

    //Changes from scene to scene
    public string _whatScene;
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

    //Sprited to be loaded on runtime
    public Sprite _theGoodPlayerSprite = null;
    public Sprite _theBadPlayerSprite = null;

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
        //RenderPlayerSprite();
    }

    private void LoadPlayerSide() {
        _playerAnim.SetBool("MachineSide", isMachineSide);
        if (isMachineSide)
        {
            _maxSpeed = 3f;
            _accel = 6.0f;
            _daccel = 6.0f;
            _life = 100;
            playerJumpForce = 3;
            isDoubleJumpAble = true;
        }
        else
        {
            _maxSpeed = 2.5f;
            _accel = 4.0f;
            _daccel = 3.5f;
            _life = 100;
            //playerJumpForce = 3;
            //isDoubleJumpAble = false;
        }
    }

    private void RenderPlayerSprite() {
        SpriteRenderer _playerSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        if (_playerSpriteRenderer.sprite != null)
            return;

        if (isMachineSide)
            _playerSpriteRenderer.sprite = _theBadPlayerSprite;
        else _playerSpriteRenderer.sprite = _theGoodPlayerSprite;
    }

    private void Update()
    {
        RenderPlayerSprite();

        checkGroundColision();
        //checkInteraction();

        if (!_playerJump && !_isBlock)
            _playerJump = CrossPlatformInputManager.GetButtonDown("Jump");
    }

    // FixedUpdate is more acurate than Update 
    void FixedUpdate ()
    {
        switch (_whatScene.ToLower())
        {
            case "initial":
                if (_isBlock || !_canMoveFreely)
                    handlePlayerNonFreely();
                else playerMoveFreely();
                if(_isBlock)
                    handlePlayerNonFreelyInteraction();
                break;

            default:
                playerMoveFreely();
                break;
        }
    }

    private void playerMoveFreely() {

        if (Input.GetKeyDown(KeyCode.E)) {
            var aux = checkInteraction();
            if (aux != null)
            {
                aux.Interact();
            }
        }

        if ((Input.GetKey(KeyCode.A)) && (Mathf.Abs(_hActualSpeed) < _maxSpeed))
            _hActualSpeed -= _accel * Time.deltaTime;
        else if ((Input.GetKey(KeyCode.D)) && (Mathf.Abs(_hActualSpeed) < _maxSpeed))
            _hActualSpeed += _accel * Time.deltaTime;
        /*else if ((Input.GetKey(KeyCode.S)) && (Mathf.Abs(_vActualSpeed) < _maxSpeed) && _interactable != null)
            _vActualSpeed -= _accel * Time.deltaTime;
        else if ((Input.GetKey(KeyCode.W)) && (Mathf.Abs(_vActualSpeed) < _maxSpeed) && _interactable != null)
            _vActualSpeed += _accel * Time.deltaTime;*/
        else
        {
            if (_hActualSpeed > _daccel * Time.deltaTime)
                _hActualSpeed = _hActualSpeed - _daccel * Time.deltaTime;
            else if (_hActualSpeed < -_daccel * Time.deltaTime)
                _hActualSpeed = _hActualSpeed + _daccel * Time.deltaTime;
            else _hActualSpeed = 0;

            if (_vActualSpeed > _daccel * Time.deltaTime)
                _vActualSpeed = _vActualSpeed - _daccel * Time.deltaTime;
            else if (_vActualSpeed < -_daccel * Time.deltaTime)
                _vActualSpeed = _vActualSpeed + _daccel * Time.deltaTime;
            else _vActualSpeed = 0;
        }

        if(_vActualSpeed != 0)
            _playerRB.velocity = new Vector2(_hActualSpeed, _vActualSpeed);
        else _playerRB.velocity = new Vector2(_hActualSpeed, _playerRB.velocity.y);

        _playerAnim.SetFloat("speed", Mathf.Abs(_hActualSpeed));
        _playerAnim.SetFloat("speedV", Mathf.Abs(_vActualSpeed));

        if (_hActualSpeed > 0 && !_playerFaceRight)
            flipSide();
        else if (_hActualSpeed < 0 && _playerFaceRight)
            flipSide();

        handleJump(_hActualSpeed != 0);
    }

    private void handleJump(bool isMoving)
    {
        if (_playerJump)
        {
            if (_amountJump == 0)
            {
                _playerRB.velocity = new Vector2(0f, playerJumpForce);
                if(isMoving)
                    _playerAnim.SetBool("jump", true);
                _isPlayerGrounded = false;

                _amountJump++;
            }
            else
            {
                if (_amountJump == 1 && isDoubleJumpAble)
                {
                    _playerRB.velocity = new Vector2(0f, playerJumpForce);
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

    private IInteractable checkInteraction()
    {
        IInteractable toRet = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_playerGroundCheck.position, 0.5f, _whatIsInteractable);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.CompareTag("Interactable"))
            {
                toRet = colliders[i].GetComponent<IInteractable>();
                break;
            }
        }
        return toRet;
        
    }


    //          I N I T I A L  S C E N E  O N L Y

    private void handlePlayerNonFreely()
    {

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
                Input.GetKey(KeyCode.E) ||
                Input.GetKey(KeyCode.Space)) && (Mathf.Abs(_hActualSpeed) < _maxSpeed))
                _hActualSpeed += _accel * Time.deltaTime;
            else
            {
                if (_hActualSpeed > _daccel * Time.deltaTime)
                    _hActualSpeed = _hActualSpeed - _daccel * Time.deltaTime;
                else if (_hActualSpeed < -_daccel * Time.deltaTime)
                    _hActualSpeed = _hActualSpeed + _daccel * Time.deltaTime;
                else _hActualSpeed = 0;
            }

            _playerRB.velocity = new Vector2(_hActualSpeed, _playerRB.velocity.y);
            _playerAnim.SetFloat("speed", Mathf.Abs(_hActualSpeed));

            if (_hActualSpeed > 0 && !_playerFaceRight)
                flipSide();
            else if (_hActualSpeed < 0 && _playerFaceRight)
                flipSide();
        }
    }

    private void handlePlayerNonFreelyInteraction()
    {
        if (_playerAnim.GetFloat("speed") == 0)
        {
            if (Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.E))
            {
                var aux = checkInteraction();
                if (aux != null) {
                    aux.Interact();
                }
            }
        }
    }

    //          E N D !

}

