using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class variable_drawer : MonoBehaviour {

    public Text var_name;

    [HideInInspector]
    public bool intersecting = false;

    public Color normal_color = Color.black;
    public Color highlighted_color = Color.blue;
    public Color error_color = Color.red;
    public Color highlighted_error_color = Color.magenta;

    List<touch_data> my_touches = new List<touch_data>();

    void Update () {


        if (node_manager.mode == node_manager.input_mode.touch) {

            int num_old_touches = my_touches.Count;
            my_touches.Clear();

            for (int i = 0; i < touch_manager.current_touches.Count; i++) {
                if (touch_manager.current_touches[i].my_obj == this.gameObject) {
                    my_touches.Add(touch_manager.current_touches[i]);
                }
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

            if (my_touches.Count > 0) {
                // as long as we have at least 1 touch, we'll drag.

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
