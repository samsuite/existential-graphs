using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pan_camera : MonoBehaviour {

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

                    transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
                }
            }
        }

	}
}
