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

	public void set_text(string t){
        var_name.text = t;
    }
}
