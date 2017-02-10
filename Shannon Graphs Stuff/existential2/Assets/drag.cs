using UnityEngine;
using System.Collections;

public class drag : MonoBehaviour {
	public GameObject dragged;
	public Vector3 object_center;
	public Vector3 point_touched;
	public Vector3 offset;
	public Vector3 new_center;
	
	RaycastHit hit;
	
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
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast (ray, out hit)){
				dragged = hit.collider.gameObject;
				object_center = dragged.transform.position;
				point_touched = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				offset = point_touched - object_center;
				dragging = true;
				
			}
		}
		if(Input.GetMouseButton(0)){
			if(dragging==true){
				point_touched= Camera.main.ScreenToWorldPoint(Input.mousePosition);
				new_center= point_touched - offset;
				dragged.transform.position = new Vector3(new_center .x, new_center .y, new_center .z);
			}
		}
		
		if(Input.GetMouseButtonUp(0)){ //button released
			dragging = false;
		}
		
	} 
	
}