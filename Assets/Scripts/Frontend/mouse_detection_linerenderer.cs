using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouse_detection_linerenderer : MonoBehaviour {

    // Use this for initialization

    System.Type circle_type = typeof(linerenderer_circle);

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); ;
            //print("cuh lick: " + (Input.mousePosition).ToString());
            print("Converted: " + hit.point);

            Object[] circles = Object.FindObjectsOfType(circle_type);

            for( int i = 0; i < circles.Length; i++ )
            {
                linerenderer_circle cir  = circles[i] as linerenderer_circle;
                print("\t[" + i.ToString() + "] - " + cir.radius.ToString());

                if( Mathf.Abs(Vector2.Distance(cir.center, hit.point) - cir.radius) < .1f )
                {
                    print("and we clicked on it");
                }
            }

        }
	}
}
