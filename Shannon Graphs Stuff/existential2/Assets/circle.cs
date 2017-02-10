using System.Collections;
using Vectrosity;
using System.Collections.Generic;
using UnityEngine;

public class circle: MonoBehaviour {
	public Vector3 point_touched;
	public Vector3 point_released;
	public Vector3 point_current;
	public Vector3 distance;
	public float radius;
	public Vector3 origin;
	public VectorLine myLine;
	public List<Vector3>[] circles;
	public int i;
	public bool flagged;
	public GameObject[] object_array;
	public Vector3 touch_dist;
	public Vector3 center;
	public CircleCollider2D collide;
	public float obj_radius;
	public float dist_mag;
	public float dist_edge;

	public bool dragging;
	Vector3 offset;
	public GameObject clicked;
	public Vector3 obj_center;
	public Vector3 new_center;
	public string clicked_name;
	public bool orig_hit;
	public bool resizing;

	public RaycastHit hit;

	void Start () {
		i = 0;
		circles = new List<Vector3>[100];//max 100 circles
		circles[i] = new List<Vector3> (200); //200 vertices (actually 100??)
		myLine = new VectorLine("circleline", circles[i], 6.0f);
		object_array = new GameObject[100]; //objects for circles to attach to
		object_array[i] = new GameObject();
		object_array [i].name = "Object " + i;

		//myLine.collider = true;
		//myLine.MakeCircle (new Vector3 (-2,-2,-2), 5);
		//myLine.Draw ();
	}
	void Update () {
		
		if (Input.GetMouseButtonDown (0)) {
			Vector2 rayPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast(rayPos,Vector2.zero, 0f);
			if (hit == false) {//click does not touch an object
				orig_hit = false;
				clicked_name = "miss";
				point_touched = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				flagged = true;
			} else {//does hit something, right now a circle
				orig_hit = true;
				point_touched = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				CircleCollider2D collide = hit.transform.gameObject.GetComponent (typeof(CircleCollider2D)) as CircleCollider2D;
				//center = collide.offset; //this is a constant value;
				center = new Vector3(collide.offset.x + hit.transform.gameObject.transform.position.x,collide.offset.y + hit.transform.gameObject.transform.position.y,-10);
				touch_dist = point_touched - center;
				obj_radius = collide.radius;
				dist_mag = touch_dist.magnitude;
				dist_edge = obj_radius - dist_mag;
				clicked = hit.collider.gameObject;
				clicked_name = clicked.name;
				if (dist_edge <.5 && dist_edge >-.5) { //clicked close enough to edge
					flagged = false; //don't want a circle drawn
					//this is when I want click and drag to work
					//dragged = hit.collider.gameObject;
					obj_center = clicked.transform.position;
					offset = point_touched - obj_center;
					dragging = true;
				} else {//click in middle of circle
					flagged = true;//do want a circle drawn
				}
			}
		}



		if (Input.GetMouseButton (0)) {
			point_current= Camera.main.ScreenToWorldPoint (Input.mousePosition);
			distance = point_touched - point_current;
			radius = (distance.magnitude) / 2;
			origin = point_touched - (distance / 2);
			myLine.MakeCircle (origin, radius);
			if (dragging == true) {
				new_center= point_current - offset;
				clicked.transform.position = new Vector3(new_center .x, new_center .y, new_center .z);
			}
			if (flagged == true) {
				myLine.Draw (); //shows the circle growth
			}
		}



		if (Input.GetMouseButtonUp (0)) {
			dragging = false;
			Vector2 rayPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast(rayPos,Vector2.zero, 0f);
			if (hit) { 
				if (clicked == hit.collider.gameObject && flagged == true) {//hit's something, can't start in one circle and end in another
					if (radius > .5) { //gets rid of tiny circles
						VectorManager.ObjectSetup (object_array [i], myLine, Visibility.Dynamic, Brightness.None);
						myLine.Draw ();
						object_array [i].AddComponent<CircleCollider2D> ();
						CircleCollider2D temp = object_array [i].GetComponent (typeof(CircleCollider2D)) as CircleCollider2D;
						temp.radius = radius;
						temp.offset = origin;
						i = i + 1;
						circles [i] = new List<Vector3> (200); //200 vertices (actually 100??)

                        // fancy jimbo
                        if (clicked == null)
                        {
                            print("clicked on the root");
                        }



						object_array [i] = new GameObject ();
						object_array [i].name = "Object " + i;
						myLine = new VectorLine ("circleline", circles [i], 6.0f);
					} else {
						print ("Sorry cut was too small");
						myLine.MakeCircle (origin, 0);
						myLine.Draw ();
					}
				} else { //released on a different object don't draw
					myLine.MakeCircle (origin, 0); //deals with this line
					if (flagged == true) {
						myLine.Draw ();
					}
				}
			} else if (hit == false) {
				if (orig_hit == false) {
					point_released = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					distance = point_touched - point_released;
					radius = (distance.magnitude) / 2;
					origin = point_touched - (distance / 2);
					myLine.MakeCircle (origin, radius);
					if (flagged == true) {
						if (radius > .5) {//DON'T MAKE TINY CIRCLES;
							VectorManager.ObjectSetup (object_array [i], myLine, Visibility.Dynamic, Brightness.None);
							myLine.Draw ();
							object_array [i].AddComponent<CircleCollider2D> ();
							CircleCollider2D temp = object_array [i].GetComponent (typeof(CircleCollider2D)) as CircleCollider2D;
							temp.radius = radius;
							temp.offset = origin;
							i = i + 1;
							circles [i] = new List<Vector3> (200); //200 vertices (actually 100??)
							object_array [i] = new GameObject ();
							object_array [i].name = "Object " + i;
							myLine = new VectorLine ("circleline", circles [i], 6.0f);
						} else {
							print ("Sorry! Cut was too small!");
							myLine.MakeCircle (origin, 0);
							myLine.Draw ();
					
						}
					}
				} else { //started in a circle went out of bounds;
					myLine.MakeCircle (origin, 0); //deals with this line
					if (flagged == true) {
						myLine.Draw ();
					}
				}
			}
		}

		if (Input.GetMouseButtonDown (1)) { //right click down
			Vector2 rayPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast(rayPos,Vector2.zero, 0f);
			if (hit == false) {//click does not touch an object
				resizing = false;
				//do nothing
			} else {//does hit something, right now a circle
				point_touched = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				CircleCollider2D collide2 = hit.transform.gameObject.GetComponent (typeof(CircleCollider2D)) as CircleCollider2D;
				//center = collide.offset; //this is a constant value;
				center = new Vector3(collide2.offset.x + hit.transform.gameObject.transform.position.x,collide2.offset.y + hit.transform.gameObject.transform.position.y,-10);
				touch_dist = point_touched - center;
				obj_radius = collide2.radius;
				dist_mag = touch_dist.magnitude;
				dist_edge = obj_radius - dist_mag;
				clicked = hit.collider.gameObject;
				clicked_name = clicked.name;
				if (dist_edge < .5 && dist_edge > -.5) { //this is the case we want resizing in
					resizing = true;
					Destroy (hit.transform.gameObject);
					myLine.MakeCircle (center, obj_radius);
					myLine.Draw ();
					print (center+" "+ obj_radius);
				}
			}
		}

		if (Input.GetMouseButton (1)) {
			point_current= Camera.main.ScreenToWorldPoint (Input.mousePosition);
			distance = point_touched - point_current;
			radius = (distance.magnitude) / 2; //in this case
			if (resizing == true) {
				myLine.MakeCircle (center, obj_radius+radius); //MAY NEED TO LOOK AT THIS
				myLine.Draw ();
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			point_current= Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector2 rayPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast(rayPos,Vector2.zero, 0f);
			if (hit == false){
				distance = point_touched - point_current;
				radius = (distance.magnitude) / 2; //in this case
				if (resizing == true) {
					myLine.MakeCircle (center, obj_radius+radius); //MAY NEED TO LOOK AT THIS
					myLine.Draw ();
					object_array [i].AddComponent<CircleCollider2D> ();
					CircleCollider2D temp = object_array [i].GetComponent (typeof(CircleCollider2D)) as CircleCollider2D;
					temp.radius = radius;
					temp.offset = origin;
					i = i + 1;
					circles [i] = new List<Vector3> (200); //200 vertices (actually 100??)
					object_array [i] = new GameObject ();
					object_array [i].name = "Object " + i;
					myLine = new VectorLine ("circleline", circles [i], 6.0f);
				}
			}
		}
			

	}
}

//need to check that when releasing in a circle the release distance is greater than .5 away from the edge
//destroy game object and create new one with circle and collider of correct sizes