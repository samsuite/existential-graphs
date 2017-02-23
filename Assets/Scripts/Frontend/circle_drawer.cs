using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (LineRenderer))]
public class circle_drawer : MonoBehaviour {

    public float radius = 1;
    public int num_points = 100;
    //public Vector2 center = Vector2.zero;
    public float line_width = 0.05f;

    LineRenderer lines;
    public Transform quad;

    [HideInInspector]
    public bool intersecting = false;

    public Color normal_color = Color.black;
    public Color highlighted_color = Color.blue;
    public Color error_color = Color.red;
    public Color highlighted_error_color = Color.magenta;


	void Start () {
        lines = GetComponent<LineRenderer>();
		redraw_circle();
	}

    void Update () {

        // technically we don't need to redraw this every frame, but that's actually a pretty negligible optimization (i tried it) and it introduces some annoying bugs.
        // so whatever. no need to prematurely optimize
        redraw_circle();

        if (intersecting){

            if (node_manager.currently_selected_circle == this){
                set_color(highlighted_error_color);
            }
            else {
                set_color(error_color);
            }

        }
        else if (node_manager.currently_selected_circle == this){
            set_color(highlighted_color);
        }
        else {
            set_color(normal_color);
        }

        // check if we're intersecting with a cut
        intersecting = false;
        for( int i = 0; i < node_manager.all_cuts.Count; i++ )
        {
            circle_drawer cir  = node_manager.all_cuts[i];

            if (this != cir){
                if (node_manager.intersect(this, cir)){
                    intersecting = true;
                }
            }
        }

        // check if we're intersecting with a variable
        for( int i = 0; i < node_manager.all_vars.Count; i++ )
        {
            variable_drawer var  = node_manager.all_vars[i];

            if (node_manager.intersect(this, var)){
                intersecting = true;
            }
        }

    }

    void redraw_circle () {
        lines.startWidth = line_width;
        lines.endWidth = line_width;

        lines.numPositions = num_points+2;

        for (int i = 0; i < num_points+2; i++){
            float theta = ((float)i/(float)num_points)*Mathf.PI*2;
            lines.SetPosition(i, transform.position + new Vector3(Mathf.Cos(theta)*radius,Mathf.Sin(theta)*radius,0f));
        }

        quad.transform.localScale = Vector3.one*radius*2f;
    }

    public void set_color (Color col){
        lines.startColor = col;
        lines.endColor = col;
    }
}
