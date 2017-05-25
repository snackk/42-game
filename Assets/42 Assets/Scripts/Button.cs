using System.Threading;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Button : MonoBehaviour, IInteractable
{

    [SerializeField]
    private int _timesBeforeLightsOff = 2;

    private GameObject screen;
    private Renderer screenRenderer, officeON, deskOfficeON, deskOfficeOFF, officeOFF;

    private int timesCalled = 0;
    private float timer = 0.0f;

    private object lockIsInteractable = new object();
    private bool isInteractable = false;

    private object lockScreenRenderer = new object();
    private bool reEnableScreenRenderer = false;
    private bool lightsState = true;

    //DIOGOS
    private PlayerController _player = null;
    private PlayerMovement _playerMovement;

    // Use this for initialization
    void Start()
    {
        screen = this.gameObject.transform.GetChild(0).gameObject;
        screenRenderer = screen.GetComponent<Renderer>();
        officeON = GameObject.Find("Office_ON").GetComponent<Renderer>();
        deskOfficeON = GameObject.Find("Office_ON/Office_ON_Desk").GetComponent<Renderer>();
        deskOfficeOFF = GameObject.Find("Office_OFF").GetComponent<Renderer>();
        officeOFF = GameObject.Find("Office_OFF/Office_off_desk").GetComponent<Renderer>();

        _playerMovement = PlayerMovement.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (_player != null)
        {
            if (_playerMovement.wannaJump || _playerMovement.verticalMove != 0 || _playerMovement.horizontalMove != 0)
            {
                Interact(_player);
            }
            else
            {
                if (timesCalled > 0)
                {
                    var ready = timesCalled >= _timesBeforeLightsOff;
                    _player._isBlock = !ready;
                    _player._canMoveFreely = ready;
                }
            }

            lock (lockScreenRenderer)
            {
                if (reEnableScreenRenderer)
                {
                    reEnableScreenRenderer = false;
                    screenRenderer.enabled = true;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        lock (lockIsInteractable)
            isInteractable = timesCalled < _timesBeforeLightsOff;

        Debug.Log("Object entered");
        if (timesCalled == 0)
        {
            screenRenderer.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Object Left");
        if (!lightsState)
        {
            lightsState = true;

            SwitchLights(true);
            /*Thread oThread = new Thread (new ThreadStart (() => { 
				Thread.Sleep (1500);
				SwitchLights(true);
			}));
			oThread.Start ();*/
        }
        isInteractable = false;
    }

    void SwitchLights(bool state)
    {
        deskOfficeON.enabled = state;
        officeON.enabled = state;
        officeOFF.enabled = !state;
        deskOfficeOFF.enabled = !state;
    }

    public int Interact(PlayerController player)
    {
        this._player = player;
        _player._isBlock = true;

        lock (lockIsInteractable)
        {
            if (isInteractable)
            {
                isInteractable = false;
                timer = 0;
                if (++timesCalled >= _timesBeforeLightsOff)
                {

                    screenRenderer.enabled = false;
                    SwitchLights(false);
                    lightsState = false;

                    //TODO: Change camera to follow the player.
                    //_player._isBlock = true;
                    //_player._canMoveFreely = false;
                    //TODO

                }
                else
                {
                    Debug.Log(timesCalled);
                    screenRenderer.enabled = false;

                    Thread oThread = new Thread(new ThreadStart(() => {
                        Thread.Sleep(5000);
                        lock (lockIsInteractable)
                        {
                            isInteractable = true;
                        }
                        lock (lockScreenRenderer)
                        {
                            reEnableScreenRenderer = true;
                        }
                    }));

                    // Start the thread
                    oThread.Start();
                }
            }
        }
        return 0;
    }
}