using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (LineRenderer))]
public class linerenderer_circle : MonoBehaviour {

    public float radius = 1;
    public int num_points = 100;
    public Vector2 center = Vector2.zero;

    LineRenderer lines;
	public LineRenderer fill;

	void Start () {
        lines = GetComponent<LineRenderer>();

		redraw_circle();
	}

    void Update () {
        /*
        if (Random.value < .5f)
            radius -= Random.value / 10f;
        else
            radius += Random.value / 10f;
        radius = Mathf.Clamp(radius, 0f, 7.5f);
        */

        redraw_circle();
    }

    void redraw_circle () {
        lines.numPositions = num_points+2;
        fill.numPositions = num_points+2;

        fill.startWidth = radius + 0.1f;
        fill.endWidth = radius + 0.1f;


        for (int i = 0; i < num_points+2; i++){
            float theta = ((float)i/(float)num_points)*Mathf.PI*2;
            lines.SetPosition(i, transform.position + new Vector3(Mathf.Cos(theta)*radius,Mathf.Sin(theta)*radius,0f));
            fill.SetPosition(i, transform.position + new Vector3(Mathf.Cos(theta)*(radius/2f - 0.1f),Mathf.Sin(theta)*(radius/2f - 0.1f),0f));
        }
    }
}
