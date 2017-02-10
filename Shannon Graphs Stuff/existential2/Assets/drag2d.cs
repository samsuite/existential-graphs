using UnityEngine;
using System.Collections;

public class drag2d : MonoBehaviour {
	public GameObject dragged;
	public Vector2 object_center;
	public Vector2 point_touched;
	public Vector2 offset;
	public Vector2 new_center;

	RaycastHit2D hit;

	public bool dragging = false;



	// Use this for initialization
	void Start () {

	}


	void OnTriggerStay(Collider other) {
		if (Input.GetMouseButtonUp (0)) {
			dragged.transform.parent = other.transform;		
		}
	}
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButtonDown(0)){
			Vector2 rayPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast(rayPos,Vector2.zero, 0f);

			if(hit){
				dragged = hit.collider.gameObject;
				object_center = dragged.transform.position;
				point_touched =  new Vector2 ((Camera.main.ScreenToWorldPoint (Input.mousePosition).x), (Camera.main.ScreenToWorldPoint (Input.mousePosition).y));
				offset = point_touched - object_center;
				dragging = true;

			}
		}
		if(Input.GetMouseButton(0)){
			if(dragging==true){
				point_touched= new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
				new_center= point_touched - offset;
				dragged.transform.position = new Vector2(new_center .x, new_center .y);
			}
		}

		if(Input.GetMouseButtonUp(0)){ //button released
			dragging = false;
		}

	} 

}