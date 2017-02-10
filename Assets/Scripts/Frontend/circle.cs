using Vectrosity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circle: MonoBehaviour {

    public float min_radius = 0.5f;

	Vector3 point_touched;
	Vector3 point_released;
	Vector3 point_current;

	VectorLine current_line;
    GameObject current_object;
	List<List<Vector3>> circles;

	Vector3 center;
	CircleCollider2D circ_collider;
	float obj_radius;

	Vector3 drag_offset;
    Vector3 dragged_origin;
    bool orig_hit; string dragged_name;
    string resized_name;
    float resized_radius;

    GameObject clicked_circle;
	//bool orig_hit;

    bool drawing_circle = false;
    bool dragging_circle = false;
	bool resizing_circle = false;

    public class circ
    {
        public float radius;
        public Vector3 origin;
        public string name;
        public circ(float rad, Vector3 org, string n)
        {
            radius = rad;
            origin = org;
            name = n;
        }
    }
    List<circ> circ_list;



    void Start() {
        // instantiate lists
		circles = new List<List<Vector3>>();
        circ_list = new List<circ>();
    }


	void Update() {
		
        // when lmb is pressed
		if (Input.GetMouseButtonDown (0)) {
			Vector2 rayPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast(rayPos,Vector2.zero, 0f);

            bool should_start_circle = false; /// should we start a new circle? false if we're dragging something, true if we clicked on empty space
            Node parent = GameObject.FindWithTag("ROOT").GetComponent<Node>();

			if (hit) {
                // the raycast hit something

                print ("hit "+hit.transform.gameObject.name);

                orig_hit = true;
				point_touched = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				CircleCollider2D other_collider = hit.transform.gameObject.GetComponent<CircleCollider2D>();
                clicked_circle = hit.collider.gameObject;

                // figure out if we hit a circle close enough to the edge that we should drag it
				center = new Vector3(other_collider.offset.x + hit.transform.gameObject.transform.position.x,other_collider.offset.y + hit.transform.gameObject.transform.position.y,-10);
				obj_radius = other_collider.radius;

                float dist_mag = (point_touched - center).magnitude;
				float dist_edge = obj_radius - dist_mag;

				if (dist_edge < 0.5f && dist_edge > -0.5f) { // clicked close enough to edge
                    dragged_name = hit.transform.gameObject.name;
                    drag_offset = point_touched - clicked_circle.transform.position;
					dragging_circle = true;
                    should_start_circle = false;

				}
                else {
					should_start_circle = true;
                    parent = other_collider.gameObject.GetComponent<Node>();
				}

			}
            else {
				// the raycast didn't hit anything
				orig_hit = false;
				point_touched = Camera.main.ScreenToWorldPoint (Input.mousePosition);

                should_start_circle = true;
			}


            if (should_start_circle){
                add_circle(parent);
                drawing_circle = true;
            }
		}


        // if lmb is being held
		if (Input.GetMouseButton (0)) {

			point_current = Camera.main.ScreenToWorldPoint (Input.mousePosition);

            // are we dragging a circle?
			if (dragging_circle) {

                // reposition it based on the mouse position
				clicked_circle.transform.position = point_current - drag_offset;
                dragged_origin = clicked_circle.transform.position;
            }

            // are we drawing a new circle?
			if (drawing_circle) {

                // figure out how big it should be
                Vector3 distance = point_touched - point_current;
			    float radius = (distance.magnitude) / 2;
			    Vector3 origin = point_touched - (distance / 2);

                // and draw it
			    current_line.MakeCircle (origin, radius);
				current_line.Draw();

                // and update the collider
                CircleCollider2D col = current_object.GetComponent<CircleCollider2D> ();
		        col.radius = radius;
		        col.offset = origin;
			}
		}


        // when lmb is released
        if (Input.GetMouseButtonUp(0))
        {

            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

            // we don't want the raycast to pay attention to the circle we're dragging
            if (dragging_circle)
            {
                clicked_circle.GetComponent<CircleCollider2D>().enabled = false;
            }

            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

            if (dragging_circle)
            {

                clicked_circle.GetComponent<CircleCollider2D>().enabled = true;

                if (hit)
                {
                    // the raycast hit something
                    point_touched = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    CircleCollider2D other_collider = hit.transform.gameObject.GetComponent<CircleCollider2D>();

                    clicked_circle.transform.parent = hit.transform;
                }
                else
                {
                    clicked_circle.transform.parent = GameObject.FindWithTag("ROOT").transform;
                }

                clicked_circle.transform.localPosition = new Vector3(clicked_circle.transform.localPosition.x, clicked_circle.transform.localPosition.y, -1f);
            }

            dragging_circle = false;

            Vector3 distance = point_touched - point_current;
            float radius = (distance.magnitude) / 2;
            Vector3 origin = point_touched - (distance / 2);

            // are we drawing a circle?
            if (drawing_circle)
            {
                drawing_circle = false;

                bool should_cancel_circle = false; // we need to be careful not to delete circles more than once

                // if the radius is too small
                if (radius < min_radius)
                {
                    print("cut was too small");
                    should_cancel_circle = true;
                }

                foreach (var value in circ_list)
                { ///runs trough circle list checks for intersection
                    // if we started dragging in one circle and ended in another
                    if (Mathf.Pow((radius - value.radius), 2) <= Mathf.Pow((origin.x - value.origin.x), 2) + Mathf.Pow((origin.y - value.origin.y), 2) && Mathf.Pow((origin.x - value.origin.x), 2) + Mathf.Pow((origin.y - value.origin.y), 2) <= Mathf.Pow((radius + value.radius), 2))
                    {
                        {
                            should_cancel_circle = true;
                            print("cut interrupted"); break;
                            should_cancel_circle = true;
                        }
                    }

                    /*
                    // if we started dragging in one circle and ended in another
                    if ((orig_hit && !hit) || (!orig_hit && hit) || (orig_hit && hit) && clicked_circle != hit.transform.gameObject ){
                        print ("cut interrupted");
                        should_cancel_circle = true;
                    }
                    */

                    // TODO:
                    // also need to test if the circles are intersecting in some other way


                    if (should_cancel_circle)
                    {
                        cancel_circle();
                    }
                    else
                    {
                        string name = current_object.name;
                        print(name);
                        circ temp = new circ(radius, origin, name);
                        print(temp.origin);
                        circ_list.Add(temp);
                        GameObject.FindWithTag("ROOT").GetComponent<Node>().recursiveReposition();
                    }
                }
            }
        }
        

        // when rmb is pressed
		if (Input.GetMouseButtonDown (1)) {
			Vector2 rayPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast(rayPos,Vector2.zero, 0f);

			if (hit) {
                // the raycast hit something

                point_touched = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				CircleCollider2D other_collider = hit.transform.gameObject.GetComponent<CircleCollider2D>();

				center = new Vector3(other_collider.offset.x + hit.transform.gameObject.transform.position.x,other_collider.offset.y + hit.transform.gameObject.transform.position.y,-10);
				obj_radius = other_collider.radius;

                float dist_mag = (point_touched - center).magnitude;
				float dist_edge = obj_radius - dist_mag;

				clicked_circle = hit.collider.gameObject;
				if (dist_edge < 0.5f && dist_edge > -0.5f) { // clicked close enough to edge
                    resized_name =hit.transform.gameObject.name;
					resizing_circle = true;

                    current_object = clicked_circle;
                    current_line = current_object.GetComponent<Node>().vecline;
                }

			}
            else {
				resizing_circle = false;
				// do nothing
			}
		}

        // if rmb is being held
		if (Input.GetMouseButton (1)) {
			point_current = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			Vector3 distance = center - point_current;
            float radius = (distance.magnitude);// / 2; //in this case

			if (resizing_circle) {
                // and update the collider
                CircleCollider2D col = current_object.GetComponent<CircleCollider2D> ();
                col.radius = radius;
                resized_radius = radius;

                current_line.MakeCircle(col.offset, radius); //MAY NEED TO LOOK AT THIS
                current_line.Draw();
            }
		}

        // if rmb is being held
		if (Input.GetMouseButtonUp (1)) {
                if (resizing_circle == true)
                {
                    foreach (var value in circ_list)
                    {
                        if (value.name == resized_name)
                        {
                            value.radius = resized_radius;
                            print("changed it!");
                            break;
                        }
                    }
                }
                resizing_circle = false;
                dragging_circle = false;
        }

	}
    //// END OF UPDATE


    public GameObject add_circle(Node parent) {

        // create a list of points for the circle
        circles.Add(new List<Vector3>(200));

        // set up the vectorline
        current_line = new VectorLine ("cut line", circles[circles.Count-1], 6.0f);

        //Node parent = GameObject.FindWithTag("ROOT").GetComponent<Node>();
        Params parameters = new Params();
        parameters.vectorline = current_line;
        parameters.posX = 0;
        parameters.posY = 0;

        // create a new cut
        current_object = InferenceRules.addCut(parent, parameters);

        // set values and add required components
		VectorManager.ObjectSetup (current_object, current_line, Visibility.Dynamic, Brightness.None);
		CircleCollider2D col = current_object.AddComponent<CircleCollider2D> ();


        Vector3 distance = point_touched - point_current;
		float radius = (distance.magnitude) / 2;
		Vector3 origin = point_touched - (distance / 2);

		col.radius = radius;
		col.offset = origin;

        current_line.MakeCircle (origin, radius);
		current_line.Draw();

        return current_object;
    }


    public GameObject add_circle_of_size(Node parent, Vector2 pos, float rad, Vector3 offset) {

        // create a list of points for the circle
        circles.Add(new List<Vector3>(200));

        // set up the vectorline
        current_line = new VectorLine ("cut line", circles[circles.Count-1], 6.0f);

        //Node parent = GameObject.FindWithTag("ROOT").GetComponent<Node>();
        Params parameters = new Params();
        parameters.vectorline = current_line;
        parameters.posX = pos.x;
        parameters.posY = pos.y;

        // create a new cut
        current_object = InferenceRules.addCut(parent, parameters);

        // set values and add required components
		VectorManager.ObjectSetup (current_object, current_line, Visibility.Dynamic, Brightness.None);
		CircleCollider2D col = current_object.AddComponent<CircleCollider2D> ();


		col.radius = rad;
		col.offset = offset;

        current_line.MakeCircle (offset, rad);
		current_line.Draw();

        return current_object;
    }



    void cancel_circle() {
        VectorLine.Destroy (ref current_line, current_object);
        circles.RemoveAt(circles.Count-1);
    }


}