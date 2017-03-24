using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_manager : MonoBehaviour {

    public static List<circle_drawer> all_cuts = new List<circle_drawer>();
    public static List<variable_drawer> all_vars = new List<variable_drawer>();

    public GameObject circle_prefab_in;
    public GameObject variable_prefab_in;
    static GameObject circle_prefab;
    static GameObject variable_prefab;
    public static circle_drawer currently_selected_circle;
    public static variable_drawer currently_selected_variable;

    [HideInInspector]
    public static bool currently_scaling_cut = false;
    [HideInInspector]
    public static bool currently_moving_cut = false;
    [HideInInspector]
    public static bool currently_moving_var = false;

    static Vector2 mouse_position;
    static Vector2 clicked_point;
    static Vector2 offset;
    
    public static float min_circle_radius = 0.5f;
    public static float selection_width = 0.1f; // originall 0.5f;
    public static float variable_selection_radius = 0.25f;


    public static bool on_button = false;

    void Awake () {
        circle_prefab = circle_prefab_in;
        variable_prefab = variable_prefab_in;
    }
    
    void Update () {

        /*
        if (Input.GetKeyDown(KeyCode.A)) {
            build_hierarchy();
        }
        */


        // get the 2d worldspace position of the mouse
        mouse_position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);


        // ok, what are we doing right now?
        if (currently_scaling_cut)         // we're in the middle of scaling a cut
        {
            // the circle radius is the distance to the mouse -- unless the cut is too small, in which case it snaps to the min size
            currently_selected_circle.radius = Mathf.Max(Vector2.Distance(mouse_position, currently_selected_circle.transform.position), min_circle_radius);
            
            // if the player releases the left mouse button
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)){
                currently_scaling_cut = false;
                currently_selected_circle = null;
            }

        }
        else if (currently_moving_cut)    // we're in the middle of moving a cut
        {
            currently_selected_circle.transform.position = new Vector3(offset.x+mouse_position.x, offset.y+mouse_position.y, 0f);

            // if the player releases the left mouse button
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)){
                currently_moving_cut = false;
            }

        }
        else if (currently_moving_var)    // we're in the middle of moving a variable
        {
            currently_selected_variable.transform.position = new Vector3(offset.x+mouse_position.x, offset.y+mouse_position.y, 0f);

            // if the player releases the left mouse button
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)){
                currently_moving_var = false;
            }
        }
        else                               // we're not editing anything right now, so we can start
        {
            // is the mouse on the edge of a circle or variable? if so, select it. variable have priority over circles
            currently_selected_circle = null;
            currently_selected_variable = null;
            for( int i = 0; i < all_cuts.Count; i++ )
            {
                circle_drawer cir = all_cuts[i];
                Vector2 center = new Vector2(cir.transform.position.x, cir.transform.position.y);

                if( Mathf.Abs(Vector2.Distance(center, mouse_position) - cir.radius) < selection_width ) {
                    currently_selected_circle = cir;
                }
            }

            for( int i = 0; i < all_vars.Count; i++ )
            {
                variable_drawer var = all_vars[i];
                Vector2 center = new Vector2(var.transform.position.x, var.transform.position.y);

                if( Vector2.Distance(center, mouse_position) < variable_selection_radius ) {
                    currently_selected_variable = var;
                    currently_selected_circle = null;   // even if we are over a circle, we're over a variable so we don't care lol. deselect the circle
                }
            }



            if (Input.GetMouseButtonDown(0)){
                // we clicked. do we have anything selected? if so, move that. otherwise, make a new circle

                if (currently_selected_circle) {
                    currently_moving_cut = true;
                    clicked_point = mouse_position;
                    offset = new Vector2(currently_selected_circle.transform.position.x,currently_selected_circle.transform.position.y) - clicked_point;
                }
                else if (currently_selected_variable) {
                    currently_moving_var = true;
                    clicked_point = mouse_position;
                    offset = new Vector2(currently_selected_variable.transform.position.x,currently_selected_variable.transform.position.y) - clicked_point;
                }
                else {
                    if(!on_button)
                    {
                        currently_selected_circle = AddCircle(mouse_position);
                        clicked_point = mouse_position;
                        currently_scaling_cut = true;
                    }
                }
            }

            if (Input.GetMouseButtonDown(1)){
                // we right-clicked. do we have a circle selected? if so, scale it. otherwise, make a new variable

                if (currently_selected_circle) {
                    currently_scaling_cut = true;
                }
                else if (!currently_selected_variable){ // if we're not selecting a variable already
                    currently_selected_variable = AddVariable(mouse_position, "Q");   // add a new one
                    currently_moving_var = true;
                    clicked_point = mouse_position;
                    offset = Vector2.zero;
                }
            }

            if (Input.GetMouseButtonDown(2)){
                // we middle-clicked. do we have anything selected? if so, delete it

                if (currently_selected_circle) {
                    RemoveCircle(currently_selected_circle);
                }
                else if (currently_selected_variable) {
                    RemoveVariable(currently_selected_variable);
                }
            }
        }
    }

    // put a new circle in the scene
    public static circle_drawer AddCircle(Vector2 pos, float radius = min_circle_radius)
    {    
        circle_drawer new_cut = Instantiate(circle_prefab, new Vector3(pos.x,pos.y,0f), Quaternion.identity).GetComponent<circle_drawer>();
        new_cut.radius = radius;
        all_cuts.Add(new_cut);

        return new_cut;
    }


    // put a new variable in the scene
    public static variable_drawer AddVariable(Vector2 pos, string name)
    {    
        variable_drawer new_var = Instantiate(variable_prefab, new Vector3(pos.x,pos.y,0f), Quaternion.identity).GetComponent<variable_drawer>();
        all_vars.Add(new_var);
        new_var.set_text(name);

        return new_var;
    }

    // delete a circle and remove it from the list
    public static void RemoveCircle(circle_drawer cir)
    {
        all_cuts.Remove(cir);
        Destroy(cir.gameObject);
    }

    // delete a variable and remove it from the list
    public static void RemoveVariable(variable_drawer var)
    {
        all_vars.Remove(var);
        Destroy(var.gameObject);
    }

    // clear all cuts and vars from the scene
    public static void EraseAll()
    {
        // have to delete in reverse order so we don't mess with the indices as we clear out the lists
        for (int i = all_vars.Count-1; i >= 0; i--) {
            RemoveVariable(all_vars[i]);
        }
        for (int i = all_cuts.Count-1; i >= 0; i--) {
            RemoveCircle(all_cuts[i]);
        }
    }

    // does this other circle intersect with this other circle?
    public static bool intersect (circle_drawer cir_a, circle_drawer cir_b){

        float dist = Vector2.Distance(new Vector2(cir_a.transform.position.x, cir_a.transform.position.y), new Vector2(cir_b.transform.position.x, cir_b.transform.position.y));
        float threshold = 0.05f;

        if (dist > (cir_a.radius + cir_b.radius) + threshold) {
            // no overlap
            return false;
        }
        else if (dist <= (cir_a.radius - cir_b.radius) - threshold) 
        {
            // b is completely inside a
            return false;
        }
        else if (dist <= (cir_b.radius - cir_a.radius) - threshold) 
        {
            // a is completely inside b
            return false;
        }
        
        // we got overlap
        return true;
    }

    // does this circle intersect with this variable?
    public static bool intersect (circle_drawer cir, variable_drawer var){

        float dist = Vector2.Distance(new Vector2(cir.transform.position.x, cir.transform.position.y), new Vector2(var.transform.position.x, var.transform.position.y));
        float threshold = 0.05f;

        if (dist > (cir.radius + variable_selection_radius) + threshold) {
            // no overlap
            return false;
        }
        else if (dist <= (cir.radius - variable_selection_radius) - threshold)
        {
            // variable is completely inside circle
            return false;
        }
        
        // we got overlap
        return true;
    }

    // does this variable intersect with this other variable?
    public static bool intersect (variable_drawer var_a, variable_drawer var_b){

        float dist = Vector2.Distance(new Vector2(var_a.transform.position.x, var_a.transform.position.y), new Vector2(var_b.transform.position.x, var_b.transform.position.y));
        float threshold = 0.05f;

        if (dist > (variable_selection_radius*2) + threshold) {
            // no overlap
            return false;
        }

        // we got overlap
        return true;
    }

    // does circle a enclose circle b?
    public static bool contains (circle_drawer cir_a, circle_drawer cir_b){

        float dist = Vector2.Distance(new Vector2(cir_a.transform.position.x, cir_a.transform.position.y), new Vector2(cir_b.transform.position.x, cir_b.transform.position.y));
        float threshold = 0.05f;

        if (dist <= (cir_a.radius - cir_b.radius) - threshold) 
        {
            // b is completely inside a
            return true;
        }

        return false;
    }


    // does this circle encolse this variable?
    public static bool contains (circle_drawer cir, variable_drawer var){

        float dist = Vector2.Distance(new Vector2(cir.transform.position.x, cir.transform.position.y), new Vector2(var.transform.position.x, var.transform.position.y));
        float threshold = 0.05f;
        
        if (dist <= (cir.radius - variable_selection_radius) - threshold)
        {
            // variable is completely inside circle
            return true;
        }
        
        return false;
    }


    static int compare_circle_size(circle_drawer x, circle_drawer y) {
        if (x.radius > y.radius) {
            return -1;
        }
        else if (x.radius < y.radius) {
            return 1;
        }
        else { 
            return 0;
        }
    }


    public static void build_hierarchy () {

        print ("building...");

        // clear all parenting
        for (int i = 0; i < all_cuts.Count; i++) {
            all_cuts[i].transform.parent = null;
        }
        for (int i = 0; i < all_vars.Count; i++) {
            all_vars[i].transform.parent = null;
        }

        // make sure there are no intersections
        // in cuts...
        for (int i = 0; i < all_cuts.Count; i++) {
            if (all_cuts[i].intersecting) {
                Debug.LogError("can't build hierarchy -- intersections exist");
                return;
            }
        }
        // and vars.
        for (int i = 0; i < all_vars.Count; i++) {
            if (all_vars[i].intersecting) {
                Debug.LogError("can't build hierarchy -- intersections exist");
                return;
            }
        }

        all_cuts.Sort(compare_circle_size);

        // parent cuts
        for (int i = 0; i < all_cuts.Count; i++) {
            for (int j = 0; j < i; j++) {
                if (contains(all_cuts[j], all_cuts[i])) {
                    all_cuts[i].transform.parent = all_cuts[j].transform;
                }
            }
        }

        // parent vars
        for (int i = 0; i < all_vars.Count; i++) {
            for (int j = 0; j < all_cuts.Count; j++) {
                if (contains(all_cuts[j], all_vars[i])) {
                    all_vars[i].transform.parent = all_cuts[j].transform;
                }
            }
        }

        // bada bing bada boom
    }


    // shannon's added functions
    public void ClickAddVariable(string name)
    {
        Vector2 pos;
        //mouse_position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);		
        pos = new Vector2(0f, 0f);
        currently_selected_variable = Instantiate(variable_prefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<variable_drawer>();
        all_vars.Add(currently_selected_variable);
        currently_selected_variable.set_text(name);
        currently_moving_var = false;
        clicked_point = mouse_position;
        offset = Vector2.zero;
    }
    public void OnButton()
    {
        on_button = true;
    }
    public void OffButton()
    {
        on_button = false;
    }



}
