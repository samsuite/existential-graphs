using UnityEngine;
using System.Collections;
using Vectrosity;
using System.Collections.Generic;

public class test2d : MonoBehaviour {
	public VectorLine myLine;
	public Vector2[] line_array = new Vector2[101];


	// Use this for initialization
	void Start () {

		List<Vector2> line_points = new List<Vector2>(line_array);
		myLine = new VectorLine("circleline", line_points, 6.0f);
		myLine.MakeCircle (new Vector2 (100,100), 10);
		myLine.Draw ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
