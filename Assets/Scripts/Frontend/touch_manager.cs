using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touch_data {
    public int index;                   // the index of this touch
    public Vector3 start_pos;           // the starting position of this touch
    public Vector3 center_start_pos;    // the position of the center of the circle when this touch starts
    public GameObject my_obj;
}

public class touch_manager : MonoBehaviour {

    public static List<touch_data> current_touches = new List<touch_data>();

	void Update () {
		
        
        if (node_manager.mode == node_manager.input_mode.touch) {

            // look through every touch on the screen.
            // if it began this frame and it is on this circle, add it to the list of current touches
            for (int i = 0; i < Input.touchCount; i++) {
                if (Input.GetTouch(i).phase == TouchPhase.Began) {

                    Vector3 touchpos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                    touch_data temp = new touch_data();
                    temp.index = i;
                    temp.start_pos = touchpos;

                    // check all cuts
                    float min_rad = float.MaxValue;
                    for (int j = 0; j < node_manager.all_cuts.Count; j++) {

                        float dist_to_center = Vector3.Distance(new Vector3(touchpos.x, touchpos.y, 0f), new Vector3(node_manager.all_cuts[j].transform.position.x, node_manager.all_cuts[j].transform.position.y, 0f));

                        if (dist_to_center < node_manager.all_cuts[j].radius) {
                            if (node_manager.all_cuts[j].radius < min_rad) {
                                min_rad = node_manager.all_cuts[j].radius;
                                temp.my_obj = node_manager.all_cuts[j].gameObject;
                                temp.center_start_pos = temp.my_obj.transform.position;
                            }
                        }
                    }


                    // check all variables
                    for (int j = 0; j < node_manager.all_vars.Count; j++) {

                        float dist_to_center = Vector3.Distance(new Vector3(touchpos.x, touchpos.y, 0f), new Vector3(node_manager.all_vars[j].transform.position.x, node_manager.all_vars[j].transform.position.y, 0f));

                        if (dist_to_center < node_manager.variable_selection_radius) {
                            temp.my_obj = node_manager.all_vars[j].gameObject;
                        }
                    }
                    
                    if (temp.my_obj) {
                        temp.center_start_pos = temp.my_obj.transform.position;
                    }

                    current_touches.Add(temp);
                }
            }

            

            // look through every current touch. if it's been cancelled or finished, remove it from the list
            List<touch_data> new_touches = new List<touch_data>();
            for (int i = 0; i < current_touches.Count; i++) {
                if (current_touches[i].index < Input.touchCount) {
                    if (Input.GetTouch(current_touches[i].index).phase == TouchPhase.Ended || Input.GetTouch(current_touches[i].index).phase == TouchPhase.Canceled) {
                        // this touch is over.
                        // don't add it to the list
                    }
                    else {
                        new_touches.Add(current_touches[i]);
                    }
                }
            }

            current_touches = new_touches;

        }

	}
}
