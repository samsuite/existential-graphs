using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class variable_drawer : MonoBehaviour {

    public struct touch_data {
        public int index;                   // the index of this touch
        public Vector3 start_pos;           // the starting position of this touch
        public Vector3 center_start_pos;    // the position of the center of the circle when this touch starts
    }

    public Text var_name;

    [HideInInspector]
    public bool intersecting = false;

    public Color normal_color = Color.black;
    public Color highlighted_color = Color.blue;
    public Color error_color = Color.red;
    public Color highlighted_error_color = Color.magenta;

    List<touch_data> current_touches = new List<touch_data>();


    void Update () {


        if (node_manager.mode == node_manager.input_mode.touch) {

            int num_old_touches = current_touches.Count;

            // look through every touch on the screen.
            // if it began this frame and it is on this circle, add it to the list of current touches
            for (int i = 0; i < Input.touchCount; i++) {
                if (Input.GetTouch(i).phase == TouchPhase.Began) {

                    Vector3 touchpos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);

                    if (Vector2.Distance(transform.position, touchpos) < node_manager.variable_selection_radius) {

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


            if (current_touches.Count > 0) {
                // otherwise (as long as we have at least 1 touch), we'll drag.

                // is select mode on?
                if (node_manager.select_mode_on) {
                    // if this object isn't already in the list of selected objects
                    if (!node_manager.selected_objects.Contains(gameObject)) {
                        // add it
                        node_manager.selected_objects.Add(gameObject);
                    }

                    for (int i = 0; i < node_manager.selected_objects.Count; i++) {
                        node_manager.selected_objects[i].transform.position += Camera.main.ScreenToWorldPoint(Input.GetTouch(current_touches[0].index).deltaPosition);
                    }

                }
                else {
                    transform.position += Camera.main.ScreenToWorldPoint(Input.GetTouch(current_touches[0].index).deltaPosition);
                }
            }
        }






        if (intersecting){

            if (node_manager.currently_selected_variable == this){
                set_color(highlighted_error_color);
            }
            else {
                set_color(error_color);
            }

        }
        else if (node_manager.currently_selected_variable == this){
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

            if (node_manager.intersect(cir, this)){
                intersecting = true;
            }
        }
        
        // check if we're intersecting with a variable
        for( int i = 0; i < node_manager.all_vars.Count; i++ )
        {
            variable_drawer var  = node_manager.all_vars[i];

            if (this != var){
                if (node_manager.intersect(this, var)){
                    intersecting = true;
                }
            }
        }
    }



	public void set_text(string t){
        var_name.text = t;
    }

    public void set_color(Color col){
        var_name.color = col;
    }

}
