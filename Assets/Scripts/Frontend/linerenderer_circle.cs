using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (LineRenderer))]
public class linerenderer_circle : MonoBehaviour {

    System.Type circle_type = typeof(linerenderer_circle);

    public float radius = 1;
    public int num_points = 100;
    //public Vector2 center = Vector2.zero;
    public float line_width = 0.05f;

    LineRenderer lines;
    public Transform quad;

    [HideInInspector]
    public bool highlighted = false;
    [HideInInspector]
    public bool intersecting = false;

    public Color normal_color = Color.black;
    public Color highlighted_color = Color.blue;


	void Start () {
        lines = GetComponent<LineRenderer>();
		redraw_circle();
	}

    void Update () {
        redraw_circle();

        if (highlighted){
            set_color(highlighted_color);
        }
        else if (intersecting){
            set_color(Color.red);
        }
        else {
            set_color(normal_color);
        }

        Object[] circles = Object.FindObjectsOfType(circle_type);

        intersecting = false;
        for( int i = 0; i < circles.Length; i++ ){
            linerenderer_circle cir  = circles[i] as linerenderer_circle;

            if (this != cir){
                if (intersects_with(cir)){
                    intersecting = true;
                }
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

    // does this other circle intersect with me (doesn't count if circles are concentric)
    bool intersects_with (linerenderer_circle other){

        float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(other.transform.position.x, other.transform.position.y));
        float threshold = 0.05f;

        if (dist > (radius + other.radius) + threshold) {
            // no overlap
            return false;
        }
        else if (dist <= (radius - other.radius) - threshold) 
        {
            // completely inside
            return false;
        }
        else if (dist <= (other.radius - radius) - threshold) 
        {
            // completely inside
            return false;
        }
        
        // overlap
        return true;
    }
}
