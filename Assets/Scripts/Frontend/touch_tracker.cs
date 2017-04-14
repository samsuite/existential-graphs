using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touch_tracker : MonoBehaviour {

    public int index;


	void Update () {
		if (index < Input.touchCount) {
            transform.position = Camera.main.ScreenToWorldPoint(Input.GetTouch(index).position);
        }

        //transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
	}
}
