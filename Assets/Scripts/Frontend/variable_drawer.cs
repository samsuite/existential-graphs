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

    void Update () {

        if (node_manager.currently_selected_variable == this){
            set_color(highlighted_color);
        }
        else if (intersecting){
            set_color(error_color);
        }
        else {
            set_color(normal_color);
        }


    }



	public void set_text(string t){
        var_name.text = t;
    }

    public void set_color(Color col){
        var_name.color = col;
    }
}
