using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(node_manager))]
public class parenting_manager : MonoBehaviour
{

    public bool parenting = false;
    private bool previous_value;

    void Awake()
    {
        previous_value = parenting;
    }

    void Update()
    {
        if (previous_value != parenting)    // aka if the value was just changed this frame
        {
            if (parenting)
            {
                ParentAll();
            }
            else
            {
                UnParentAll();
            }

            previous_value = parenting;
        }
    }

    public static void ParentAll()
    {
        UnParentAll();
        // parent all circles
        foreach (circle_drawer a in node_manager.all_cuts)
        {
            circle_drawer smallest_parent = null;
            foreach (circle_drawer b in node_manager.all_cuts)
            {
                if ( b.radius > a.radius && a != b && !node_manager.intersect(a,b) && concentric(a, b) )   // these are the only candidates that circle A could be parented to
                {   // !intersect covers the edge case that the circles are within our "degree of overlap", because concentric only just a math analysis of centers and radii
                    if( smallest_parent == null )
                    {
                        smallest_parent = b;
                    }
                    else
                    {
                        if ( b.radius < smallest_parent.radius )
                        {
                            smallest_parent = b;
                        }
                    }
                }
            }

            if ( smallest_parent != null )
            {
                a.transform.parent = smallest_parent.transform;
            }
        }

        // parent all variables
        foreach (variable_drawer v in node_manager.all_vars)
        {
            circle_drawer smallest_parent = null;
            foreach (circle_drawer b in node_manager.all_cuts)
            {
                if (b.radius > node_manager.variable_selection_radius && !node_manager.intersect(b,v) && concentric(v, b))   // these are the only candidates that circle A could be parented to
                {   // !intersect covers the edge case that the circles are within our "degree of overlap", because concentric only just a math analysis of centers and radii
                    if (smallest_parent == null)
                    {
                        smallest_parent = b;
                    }
                    else
                    {
                        if (b.radius < smallest_parent.radius)
                        {
                            smallest_parent = b;
                        }
                    }
                }
            }

            if (smallest_parent != null)
            {
                v.transform.SetParent(smallest_parent.transform);
            }
        }

    }

    public static void UnParentAll()
    {
        foreach (circle_drawer a in node_manager.all_cuts)
        {
            a.transform.parent = null;
        }

        foreach ( variable_drawer v in node_manager.all_vars )
        {
            v.transform.parent = null;
        }
    }


    // returns true if circle_a is within circle_b
    public static bool concentric(circle_drawer circle_a, circle_drawer circle_b)
    {
        float dist = Vector2.Distance(new Vector2(circle_a.transform.position.x, circle_a.transform.position.y), new Vector2(circle_b.transform.position.x, circle_b.transform.position.y));
        return dist < (circle_b.radius - circle_a.radius);
    }

    // returns true if v is within c
    public static bool concentric(variable_drawer v, circle_drawer c)
    {
        float dist = Vector2.Distance(new Vector2(v.transform.position.x, v.transform.position.y), new Vector2(c.transform.position.x, c.transform.position.y));
        return dist < (c.radius - node_manager.variable_selection_radius);
    }



}