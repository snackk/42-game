using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerDisplayHandler : MonoBehaviour {
	Renderer _connections1;
	Renderer _connections2;
	Transform _glow;
	Transform _glowBad;
	Renderer _glowRenderer;
	Renderer _glowRendererBad;
	Renderer _glowRendererSpacebar;

	public int Connection { 
		get { return _connection; } 
		set {
			_connections1.enabled = (value <= 1);
			_connections2.enabled = (value > 1);
			if (value > 1) {
				SetGlowOnPosition (_glowBad, B_E);
				print (_glowBad.localPosition.x);
				print (_glowBad.localPosition.y);
			} else {
				SetGlowOnPosition (_glowBad, B_D);
				print (_glowBad.localPosition.x);
				print (_glowBad.localPosition.y);
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

	private bool isPressedKeyGlowing { get { return _glowRenderer.enabled; } set { _glowRenderer.enabled = value; _glowRendererBad.enabled = value; } }
	private bool isPressedSpaceGlowing { get { return _glowRendererSpacebar.enabled; } set { _glowRendererSpacebar.enabled = value; _glowRendererBad.enabled = value; } }

	// Use this for initialization
	void Start () {
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
		} else if (Input.GetKey(KeyCode.Space)){
			isPressedSpaceGlowing = true;
		} else {
			isPressedKeyGlowing = false;
			isPressedSpaceGlowing = false;
		}
	}
}
