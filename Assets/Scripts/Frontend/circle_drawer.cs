using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (LineRenderer))]
public class circle_drawer : MonoBehaviour {

    public struct touch_data {
        public int index;                   // the index of this touch
        public Vector3 start_pos;           // the starting position of this touch
        public Vector3 center_start_pos;    // the position of the center of the circle when this touch starts
    }

    public float radius = 1;
    public int num_points = 100;
    //public Vector2 center = Vector2.zero;
    public float line_width = 0.05f;

    bool clicking = false;

    LineRenderer lines;
    public Transform quad;

    [HideInInspector]
    public bool intersecting = false;

    public Color normal_color = Color.black;
    public Color highlighted_color = Color.blue;
    public Color error_color = Color.red;
    public Color highlighted_error_color = Color.magenta;

    List<touch_data> current_touches = new List<touch_data>();

    Vector3 mouse_start_pos = new Vector3();
    Vector3 center_start_pos = new Vector3();

    float dist_between_points;
    float initial_dist1;
    float initial_dist2;
    float initial_radius;

    public Transform pos_1;
    public Transform pos_2;

	void Start () {
        lines = GetComponent<LineRenderer>();
		redraw_circle();

        dist_between_points = Vector2.Distance(pos_1.position, pos_2.position);
        initial_dist1 = Vector2.Distance(pos_1.position, transform.position);
        initial_dist2 = Vector2.Distance(pos_2.position, transform.position);
        initial_radius = radius;
	}

    void Update () {



        ///////////// just for testing

        Vector3 touchpos1 = pos_1.position;
        Vector3 touchpos2 = pos_2.position;

        float current_dist_between_points = Vector2.Distance(touchpos1, touchpos2);
        float dist_ratio = current_dist_between_points/dist_between_points;

        float current_dist1 = Vector2.Distance(touchpos1, transform.position);
        float current_dist2 = Vector2.Distance(touchpos2, transform.position);

        float desired_dist1 = current_dist1*dist_ratio;
        float desired_dist2 = current_dist2*dist_ratio;

        // radius = initial_radius*dist_ratio;




        // ok now we need to get the point based on the intersection of the circles

        // (x - pos_1.x)^2 + (y - pos_1.y)^2 = desired_dist1^2
        // (x - pos_2.x)^2 + (y - pos_2.y)^2 = desired_dist2^2

        // x^2 - 2*pos_1.x*x + pos_1.x^2 + y^2 - 2*pos_1.y*y + pos_1.y^2 = desired_dist1^2
        // x^2 - 2*pos_2.x*x + pos_2.x^2 + y^2 - 2*pos_2.y*y + pos_2.y^2 = desired_dist2^2


        // 2*pos_2.x*x - 2*pos_1.x*x + pos_1.x^2 - pos_2.x^2 + 2*pos_2.y*y - 2*pos_1.y*y + pos_1.y^2 - pos_2.y^2 = desired_dist1^2 - desired_dist2^2
        // 2*pos_2.x*x - 2*pos_1.x*x + 2*pos_2.y*y - 2*pos_1.y*y = desired_dist1^2 - desired_dist2^2 + pos_2.y^2 - pos_1.y^2 + pos_2.x^2 - pos_1.x^2


        // (2*pos_2.x - 2*pos_1.x)*x + (2*pos_2.y - 2*pos_1.y)*y = (desired_dist1^2 - desired_dist2^2 + pos_2.y^2 - pos_1.y^2 + pos_2.x^2 - pos_1.x^2)

        float x_coefficient = (2*touchpos2.x - 2*touchpos1.x);
        float y_coefficient = (2*touchpos2.y - 2*touchpos1.y);
        float rad_out = Mathf.Pow(desired_dist1, 2) - Mathf.Pow(desired_dist2, 2) + Mathf.Pow(touchpos2.y,2) - Mathf.Pow(touchpos1.y,2) + Mathf.Pow(touchpos2.x,2) - Mathf.Pow(touchpos1.x,2);

        // x_coefficient * x + y_coefficient * y = rad_out
        // x = (rad_out / x_coefficient) - (y_coefficient/x_coefficient) * y

        float rad_over_x = rad_out / x_coefficient;
        float coeff_rat = y_coefficient/x_coefficient;

        // x = (rad_over_x - coeff_rat * y)

        // (rad_over_x - coeff_rat*y)^2 - 2*pos_1.x*(rad_over_x - coeff_rat * y) + pos_1.x^2 + y^2 - 2*pos_1.y*y + pos_1.y^2 = desired_dist1^2

        // (rad_over_x^2) - (2*rad_over_x*coeff_rat)*y + (coeff_rat^2)*y^2


        /*

        Vector2 touchvec0 = (radius*Vector3.up).normalized; //current_touches[0].start_pos - current_touches[0].center_start_pos;
        Vector2 touchvec1 = (mouse_start_pos - center_start_pos).normalized;

        print ("mouse_start_pos: "+ mouse_start_pos + "   center_start_pos: " + center_start_pos);

        Debug.DrawLine(transform.position, touchpos1, Color.red);

        Debug.DrawRay(transform.position, touchvec0*100f, Color.blue);
        Debug.DrawRay(transform.position, touchvec1*100f, Color.yellow);
                
        float angle = Vector2.Angle(touchvec0, touchvec1);
        float theta = 180f - (angle/2f + 90f);
        float dist_to_center = (Vector3.Distance(touchpos0, touchpos1)/2f) * Mathf.Tan(theta);

        Vector3 midpoint = (touchpos0 - touchpos1)/2f + touchpos1;
        Vector3 toward_center = Vector3.Cross(touchpos1 - touchpos0, Vector3.forward).normalized;
        Vector3 center_point = midpoint + (toward_center * dist_to_center);


        touchpos0.z = 0f;
        touchpos1.z = 0f;
        center_point.z = 0f;

        Debug.DrawRay(midpoint, toward_center*10f, Color.green);

        transform.position = center_point;
        radius = Vector3.Distance(touchpos0, center_point);*/
 

        ////////////////////////////////

        


        if (node_manager.mode == node_manager.input_mode.touch) {

            // look through every touch on the screen.
            // if it began this frame and it is on this circle, add it to the list of current touches
            for (int i = 0; i < Input.touchCount; i++) {
                if (Input.GetTouch(i).phase == TouchPhase.Began) {

                    Vector3 touchpos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);

                    if (Mathf.Abs(Vector2.Distance(transform.position, touchpos) - radius) < node_manager.selection_width) {

                        touch_data temp = new touch_data();
                        temp.index = i;
                        temp.start_pos = touchpos;
                        temp.center_start_pos = transform.position;

                        current_touches.Add(temp);
                    }


                }
            }

            // look through every current touch. if it's been cancelled or finished, remove it from the list
            List<touch_data> new_touches = new List<touch_data>();
            for (int i = 0; i < current_touches.Count; i++) {
                if (Input.GetTouch(current_touches[i].index).phase == TouchPhase.Ended || Input.GetTouch(current_touches[i].index).phase == TouchPhase.Canceled) {
                    // this touch is over.
                    // don't add it to the list
                }
                else {
                    new_touches.Add(current_touches[i]);
                }
            }

            current_touches = new_touches;


            if (current_touches.Count >= 2) {
                // if we have at least 2 touches, we'll scale. we're only going to pay attention to the first 2 touches.
                /*
                Vector3 touchpos0 = Camera.main.ScreenToWorldPoint(Input.GetTouch(current_touches[0].index).position);
                Vector3 touchpos1 = Camera.main.ScreenToWorldPoint(Input.GetTouch(current_touches[1].index).position);

                Vector2 touchvec0 = current_touches[0].start_pos - current_touches[0].center_start_pos;
                Vector2 touchvec1 = current_touches[1].start_pos - current_touches[1].center_start_pos;

 
                float angle = Vector2.Angle(touchvec0, touchvec1);
                float theta = 180f - (angle/2f + 90f);
                float dist_to_center = (Vector3.Distance(touchpos0, touchpos1)/2f) * Mathf.Tan(theta);

                Vector3 midpoint = (touchpos0 - touchpos1)/2f + touchpos1;
                Vector3 toward_center = Vector3.Cross(touchpos1 - touchpos0, Vector3.forward).normalized;
                Vector3 center_point = midpoint + (toward_center * dist_to_center);


                touchpos0.z = 0f;
                touchpos1.z = 0f;
                center_point.z = 0f;

                transform.position = center_point;
                radius = Vector3.Distance(touchpos0, center_point);
                */
            }
            else if (current_touches.Count == 1) {
                // if we just have 1 touch, we'll drag.

                Vector3 touchpos = Camera.main.ScreenToWorldPoint(Input.GetTouch(current_touches[0].index).position);
                transform.position = (current_touches[0].center_start_pos - current_touches[0].start_pos) + touchpos;

            }


        }


        // technically we don't need to redraw this every frame, but that's actually a pretty negligible optimization (i tried it) and it introduces some annoying bugs.
        // so whatever. no need to prematurely optimize
        redraw_circle();

        if (intersecting){

            if (node_manager.currently_selected_circle == this || current_touches.Count > 0){
                set_color(highlighted_error_color);
            }
            else {
                set_color(error_color);
            }

        }
        else if (node_manager.currently_selected_circle == this || current_touches.Count > 0){
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
