using UnityEngine;
using System.Collections;
using Vectrosity;
using System.Collections.Generic;

public class mousetest : MonoBehaviour {
	public Vector2 mouse_position;
	public float w_pix;
	public float h_pix;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		mouse_position = (Input.mousePosition);
		w_pix = Screen.width;
		h_pix = Screen.height;
	}
}
