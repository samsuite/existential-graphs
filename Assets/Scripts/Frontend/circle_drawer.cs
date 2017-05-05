using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (LineRenderer))]
public class circle_drawer : MonoBehaviour {

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

    Vector3 mouse_start_pos = new Vector3();
    Vector3 center_start_pos = new Vector3();


    float dist_between_points;
    float initial_dist1;
    float initial_dist2;
    float initial_radius;
    float initial_angle;

    public Transform pos_1;
    public Transform pos_2;

    float desired_dist1;
    float desired_dist2;

    List<touch_data> my_touches = new List<touch_data>();


	void Start () {
        lines = GetComponent<LineRenderer>();
		redraw_circle();
	}

    void Update () {

        //print(transform.position);

        if (node_manager.mode == node_manager.input_mode.touch) {

            int num_old_touches = my_touches.Count;
            my_touches.Clear();

            for (int i = 0; i < touch_manager.current_touches.Count; i++) {
                if (touch_manager.current_touches[i].my_obj == this.gameObject) {
                    my_touches.Add(touch_manager.current_touches[i]);
                }
            }

            // if we now have 2 touches and we didn't before
            if (my_touches.Count == 2 && num_old_touches != 2) {
                // if this was the second touch you added

                Vector3 touch1_pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(my_touches[0].index).position);
                Vector3 touch2_pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(my_touches[1].index).position);

                dist_between_points = Vector2.Distance(touch1_pos, touch2_pos);
                initial_dist1 = Vector2.Distance(touch1_pos, transform.position);
                initial_dist2 = Vector2.Distance(touch2_pos, transform.position);
                initial_radius = radius;

                initial_angle = signed_angle((touch1_pos-transform.position), (touch2_pos-transform.position), Vector3.forward);
            }

			// check to parent everything back again in the parenting modular time period mode
			if (my_touches.Count == 0 && num_old_touches > 0)
			{
				// reparent everything when in parenting mode
				if (parenting_manager.parenting)
				{
					parenting_manager.ParentAll();
				}
			}


            if (my_touches.Count >= 2 && !node_manager.select_mode_on) {
                // if we have at least 2 touches, we'll scale. we're only going to pay attention to the first 2 touches.
                
                Vector3 touchpos1 = Camera.main.ScreenToWorldPoint(Input.GetTouch(my_touches[0].index).position);
                Vector3 touchpos2 = Camera.main.ScreenToWorldPoint(Input.GetTouch(my_touches[1].index).position);
                touchpos1.z = 0;
                touchpos2.z = 0;


                float current_dist_between_points = Vector2.Distance(touchpos1, touchpos2);
                float dist_ratio = current_dist_between_points/dist_between_points;

                float current_dist1 = Vector2.Distance(touchpos1, transform.position);
                float current_dist2 = Vector2.Distance(touchpos2, transform.position);

                desired_dist1 = initial_dist1*dist_ratio;
                desired_dist2 = initial_dist2*dist_ratio;


                // ok now we need to get the point based on the intersection of the circles

                Vector3 a_unit = (touchpos2 - touchpos1).normalized;
                Vector3 b_unit = -a_unit;
                Vector3 h_unit = Vector3.Cross(a_unit, Vector3.forward).normalized;

                float a_dist = (Mathf.Pow(desired_dist1, 2) - Mathf.Pow(desired_dist2, 2) + Mathf.Pow(current_dist_between_points, 2))/(2*current_dist_between_points);
                float h_dist = Mathf.Sqrt(Mathf.Pow(desired_dist1, 2) - Mathf.Pow(a_dist, 2));

                Vector3 newpos_1 = touchpos1 + (a_dist*a_unit) + (h_dist*h_unit);
                Vector3 newpos_2 = touchpos1 + (a_dist*a_unit) - (h_dist*h_unit);
                newpos_1.z = 0;
                newpos_2.z = 0;

                float newangle_1 = signed_angle((touchpos1-newpos_1), (touchpos2-newpos_1), Vector3.forward);
                float newangle_2 = signed_angle((touchpos1-newpos_2), (touchpos2-newpos_2), Vector3.forward);

                //print("angle 1:  " + newangle_1);
                //print("angle 2:  " + newangle_2);


                if (initial_angle < 0 == newangle_1 < 0) {
                    if (!float.IsNaN(newpos_1.x) && !float.IsNaN(newpos_1.y) ) {
                        transform.position = newpos_1;
                    }
                }
                else {
                    if (!float.IsNaN(newpos_2.x) && !float.IsNaN(newpos_2.y) ) {
                        transform.position = newpos_2;
                    }
                }

                if (initial_radius*dist_ratio > node_manager.min_circle_radius) {
                    radius = initial_radius*dist_ratio;
                }


                my_touches[0].start_pos = touchpos1;
                my_touches[0].center_start_pos = transform.position;

                my_touches[1].start_pos = touchpos2;
                my_touches[1].center_start_pos = transform.position;

            }
            else if (my_touches.Count > 0) {
                // otherwise (as long as we have at least 1 touch), we'll drag.

                // is select mode on?
                if (node_manager.select_mode_on) {
                    // if this object isn't already in the list of selected objects
                    if (!node_manager.selected_objects.Contains(gameObject)) {
                        // add it
                        node_manager.selected_objects.Add(gameObject);
                    }

                    for (int i = 0; i < node_manager.selected_objects.Count; i++) {
                        node_manager.selected_objects[i].transform.position += Camera.main.ScreenToWorldPoint(Input.GetTouch(my_touches[0].index).deltaPosition);
                    }

                }
                else {
                    Vector3 touch_pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(my_touches[0].index).position);
                    transform.position = (my_touches[0].center_start_pos - my_touches[0].start_pos) + touch_pos;

                    transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
                    //transform.position += Camera.main.ScreenToWorldPoint(Input.GetTouch(current_touches[0].index).deltaPosition);
                }
            }
        }


        // technically we don't need to redraw this every frame, but that's actually a pretty negligible optimization (i tried it) and it introduces some annoying bugs.
        // so whatever. no need to prematurely optimize
        redraw_circle();

        if (intersecting){

            if ((node_manager.mode == node_manager.input_mode.mouse && node_manager.currently_selected_circle == this) || (node_manager.mode == node_manager.input_mode.touch && my_touches.Count > 0)){
                set_color(highlighted_error_color);
            }
            else {
                set_color(error_color);
            }

        }
        else if ((node_manager.mode == node_manager.input_mode.mouse && node_manager.currently_selected_circle == this) || (node_manager.mode == node_manager.input_mode.touch && my_touches.Count > 0)){
            set_color(highlighted_color);
        }
        else {
            set_color(normal_color);
        }

        // is select mode on?
        if (node_manager.select_mode_on) {
            // is this object in the list of selected objects?

            if (node_manager.selected_objects.Contains(gameObject)) {
                set_color(highlighted_color);
            }
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

    public static float signed_angle (Vector3 v1, Vector3 v2, Vector3 n){
        return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
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
