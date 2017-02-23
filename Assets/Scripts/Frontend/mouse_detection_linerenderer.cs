using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouse_detection_linerenderer : MonoBehaviour {

    System.Type circle_type = typeof(linerenderer_circle);

	void Update ()
    {
        /*if(Input.GetMouseButtonDown(0))
        {
            Object[] circles = Object.FindObjectsOfType(circle_type);

            for( int i = 0; i < circles.Length; i++ )
            {
                linerenderer_circle cir  = circles[i] as linerenderer_circle;
                Vector2 center = new Vector2(cir.transform.position.x, cir.transform.position.y);
                Vector2 clicked_point = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

                if( Mathf.Abs(Vector2.Distance(center, clicked_point) - cir.radius) < .1f )
                {
                    print("and we clicked on it");
                }
            }

        }*/



        Object[] circles = Object.FindObjectsOfType(circle_type);

        for( int i = 0; i < circles.Length; i++ )
        {
            linerenderer_circle cir  = circles[i] as linerenderer_circle;
            Vector2 center = new Vector2(cir.transform.position.x, cir.transform.position.y);
            Vector2 clicked_point = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

            if( Mathf.Abs(Vector2.Distance(center, clicked_point) - cir.radius) < 0.1f ) {
                cir.highlighted = true;
            }
            else {
                cir.highlighted = false;
            }
        }

	}
}
