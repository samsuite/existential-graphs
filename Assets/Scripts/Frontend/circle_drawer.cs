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
    float initial_angle;

    public Transform pos_1;
    public Transform pos_2;

    float desired_dist1;
    float desired_dist2;


	void Start () {
        lines = GetComponent<LineRenderer>();
		redraw_circle();
	}

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos_1.position, desired_dist1);
        Gizmos.DrawWireSphere(pos_2.position, desired_dist2);
    }

    void Update () {

        ///////////// just for testing
        /*
        Vector3 touchpos1 = pos_1.position;
        Vector3 touchpos2 = pos_2.position;

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

        float newangle_1 = signed_angle((touchpos1-newpos_1), (touchpos2-newpos_1), Vector3.forward);
        float newangle_2 = signed_angle((touchpos1-newpos_2), (touchpos2-newpos_2), Vector3.forward);

        print("angle 1:  " + newangle_1);
        print("angle 2:  " + newangle_2);


        if (initial_angle < 0 == newangle_1 < 0) {
            transform.position = newpos_1;
        }
        else {
            transform.position = newpos_2;
        }

        radius = initial_radius*dist_ratio;
        */
        


        if (node_manager.mode == node_manager.input_mode.touch) {

            int num_old_touches = current_touches.Count;

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

            // if we now have 2 touches and we didn't before
            if (current_touches.Count == 2 && num_old_touches != 2) {
                // if this was the second touch you added

                Vector3 touch1_pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(current_touches[0].index).position);
                Vector3 touch2_pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(current_touches[1].index).position);

                dist_between_points = Vector2.Distance(touch1_pos, touch2_pos);
                initial_dist1 = Vector2.Distance(touch1_pos, transform.position);
                initial_dist2 = Vector2.Distance(touch2_pos, transform.position);
                initial_radius = radius;

                initial_angle = signed_angle((touch1_pos-transform.position), (touch2_pos-transform.position), Vector3.forward);
            }


            if (current_touches.Count >= 2) {
                // if we have at least 2 touches, we'll scale. we're only going to pay attention to the first 2 touches.
                
                Vector3 touchpos1 = Camera.main.ScreenToWorldPoint(Input.GetTouch(current_touches[0].index).position);
                Vector3 touchpos2 = Camera.main.ScreenToWorldPoint(Input.GetTouch(current_touches[1].index).position);


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

                float newangle_1 = signed_angle((touchpos1-newpos_1), (touchpos2-newpos_1), Vector3.forward);
                float newangle_2 = signed_angle((touchpos1-newpos_2), (touchpos2-newpos_2), Vector3.forward);

                print("angle 1:  " + newangle_1);
                print("angle 2:  " + newangle_2);


                if (initial_angle < 0 == newangle_1 < 0) {
                    transform.position = newpos_1;
                }
                else {
                    transform.position = newpos_2;
                }

                radius = initial_radius*dist_ratio;

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
