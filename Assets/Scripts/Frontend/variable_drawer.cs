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


    void Update () {

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
