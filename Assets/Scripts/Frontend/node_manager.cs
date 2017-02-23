using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_manager : MonoBehaviour {

    public static List<circle_drawer> all_cuts = new List<circle_drawer>();
    public static List<variable_drawer> all_vars = new List<variable_drawer>();

    public GameObject circle_prefab;
    public GameObject variable_prefab;
    public static circle_drawer currently_selected_circle;
    public static variable_drawer currently_selected_variable;

    bool currently_scaling_cut = false;
    bool currently_moving_cut = false;
    bool currently_moving_var = false;

    Vector2 mouse_position;
    Vector2 clicked_point;
    Vector2 offset;

    float min_circle_radius = 0.5f;
    float selection_width = 0.05f;

    void Update () {

        // get the 2d worldspace position of the mouse
        mouse_position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);



        // ok, what are we doing right now?
        if (currently_scaling_cut)         // we're in the middle of scaling a cut
        {
            // the circle radius is the distance to the mouse -- unless the cut is too small, in which case it snaps to the min size
            currently_selected_circle.radius = Mathf.Max(Vector2.Distance(mouse_position, clicked_point), min_circle_radius);
            
            // if the player releases the left mouse button
            if (!Input.GetMouseButton(0)){
                currently_scaling_cut = false;
                currently_selected_circle = null;
            }

        }
        else if (currently_moving_cut)    // we're in the middle of moving a cut
        {
            currently_selected_circle.transform.position = new Vector3(offset.x+mouse_position.x, offset.y+mouse_position.y, 0f);

            // if the player releases the left mouse button
            if (!Input.GetMouseButton(0)){
                currently_moving_cut = false;
                currently_selected_circle = null;
            }

        }
        else if (currently_moving_var)    // we're in the middle of moving a variable
        {
            
        }
        else                               // we're not editing anything right now, so we can start
        {
            // is the mouse on the edge of a circle? if so, select it
            currently_selected_circle = null;
            for( int i = 0; i < all_cuts.Count; i++ )
            {
                circle_drawer cir  = node_manager.all_cuts[i];
                Vector2 center = new Vector2(cir.transform.position.x, cir.transform.position.y);

                if( Mathf.Abs(Vector2.Distance(center, mouse_position) - cir.radius) < selection_width ) {
                    currently_selected_circle = cir;
                }
            }




            if (Input.GetMouseButtonDown(0)){
                // we clicked. do we have a circle selected? if so, move that circle. otherwise, make a new one

                if (currently_selected_circle) {
                    currently_moving_cut = true;
                    clicked_point = mouse_position;
                    offset = new Vector2(currently_selected_circle.transform.position.x,currently_selected_circle.transform.position.y) - clicked_point;
                }
                else {
                    AddCircle(mouse_position);
                }
            }
            if (Input.GetMouseButtonDown(1)){
                AddVariable(mouse_position, "Q");
            }
        }
    }

    // put a new circle in the scene
    void AddCircle(Vector2 pos)
    {    
        currently_selected_circle = Instantiate(circle_prefab, new Vector3(pos.x,pos.y,0f), Quaternion.identity).GetComponent<circle_drawer>();
        clicked_point = pos;
        currently_scaling_cut = true;

        all_cuts.Add(currently_selected_circle);
    }


    // put a new variable in the scene
    void AddVariable(Vector2 pos, string name)
    {    
        currently_selected_variable = Instantiate(variable_prefab, new Vector3(pos.x,pos.y,0f), Quaternion.identity).GetComponent<variable_drawer>();
        all_vars.Add(currently_selected_variable);

        currently_selected_variable.set_text(name);
    }
}
