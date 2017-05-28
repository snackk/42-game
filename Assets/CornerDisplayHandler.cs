using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerDisplayHandler : MonoBehaviour {
	Renderer _connections1;
	Renderer _connections2;
	Renderer _connectionsBroken;
	Renderer _self;

	Transform _glow;
	Transform _glowBad;
	Renderer _glowRenderer;
	Renderer _glowRendererBad;
	Renderer _glowRendererSpacebar;

	bool showBad = true;

	public void disable(){
		_connections1.enabled = _connections2.enabled = _self.enabled = false;
	}
	public int Connection { 
		get { return _connection; } 
		set {
			_connections1.enabled = (value <= 1);
			_connections2.enabled = (value == 2);
			_connectionsBroken.enabled = (value > 2);
			showBad = !(value > 2);
			if (value == 2) {
				SetGlowOnPosition (_glowBad, B_E);
			} else if (value < 2){
				SetGlowOnPosition (_glowBad, B_D);
			}
			_connection = value;
		}
	}

	private int _connection;

	private Vector3 A = new Vector3(-0.181f, 0.131f, -3.1f);
	private Vector3 S = new Vector3(-0.063f, 0.131f, -3.1f);
	private Vector3 W = new Vector3(-0.063f, 0.247f, -3.1f);
	private Vector3 D = new Vector3(0.062f, 0.131f, -3.1f);
	private Vector3 E = new Vector3(0.291f, 0.247f, -3.1f);
	private Vector3 SPACE = new Vector3(0.361f, 0.131f, -3.1f);

	private Vector3 B_E = new Vector3(0.291f, -0.124f, -3.1f);
	private Vector3 B_D = new Vector3(0.062f, -0.245f, -3.1f);

	private bool isPressedKeyGlowing { get { return _glowRenderer.enabled; } set { _glowRenderer.enabled = value; _glowRendererBad.enabled = showBad?value:false; } }
	private bool isPressedSpaceGlowing { get { return _glowRendererSpacebar.enabled; } set { _glowRendererSpacebar.enabled = value; _glowRendererBad.enabled = showBad?value:false; } }

	// Use this for initialization
	void Start () {
		_self = gameObject.GetComponent<Renderer> ();
		foreach (Transform child in transform) {
			switch (child.name.ToLower()) {
			case "connections1":
				_connections1 = child.GetComponent<Renderer> ();
				break;
			case "connections2":
				_connections2 = child.GetComponent<Renderer> ();
				break;
			case "glow":
				_glow = child;
				_glowRenderer = _glow.GetComponent<Renderer> ();
				break;
			case "glowbad":
				_glowBad = child;
				_glowRendererBad = _glowBad.GetComponent<Renderer> ();
				break;
			case "glowspace":
				SetGlowOnPosition (child.transform, SPACE);
				_glowRendererSpacebar = child.GetComponent<Renderer> ();
				break;
			case "cornerdisplaybroken":
				_connectionsBroken = child.GetComponent<Renderer> ();
				print (_connectionsBroken);
				break;
			}
		}
		Connection = 1;
		isPressedKeyGlowing = false;
	}


	void SetGlowOnPosition(Transform glow, Vector3 coordinate){
		glow.localPosition = coordinate;
	}
	
	// Update is called once per frame
	void Update () {
		if (_self.enabled || _connectionsBroken.enabled) {
			print (_connectionsBroken.enabled);
			if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) {
				SetGlowOnPosition (_glow, A);
				isPressedKeyGlowing = true;
			} else if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) {
				SetGlowOnPosition (_glow, S);
				isPressedKeyGlowing = true;
			} else if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {
				SetGlowOnPosition (_glow, D);
				isPressedKeyGlowing = true;
			} else if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) {
				SetGlowOnPosition (_glow, W);
				isPressedKeyGlowing = true;
			} else if (Input.GetKey (KeyCode.E)) {
				SetGlowOnPosition (_glow, E);
				isPressedKeyGlowing = true;
			} else if (Input.GetKey (KeyCode.Space)) {
				isPressedSpaceGlowing = true;
			} else {
				isPressedKeyGlowing = false;
				isPressedSpaceGlowing = false;
			}
		}
	}
}
