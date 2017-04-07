using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touch_manager : MonoBehaviour {

	void Update () {
		
        if (Input.touchCount > 0) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, Camera.main.nearClipPlane));
            Debug.DrawLine(Vector3.zero, pos);
        }

	}
}
