  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(node_manager))]
public class parenting_manager : MonoBehaviour
{
    public ExistentialGraph curr_state;
    public ExistentialGraph prev_state;

    public GameObject parenting_button;

    public static bool parenting = false;
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


    public void ConvertAndPrint ()
    {
        prev_state = curr_state;
        
        //print("current state: " + curr_state);
        //print("prev state: " + prev_state);

        curr_state = ConvertToTree();
        //print("Converted scene to ISONode");
        
        print("current state: " + curr_state);

        string print_msg = curr_state.ToString();
        print(print_msg);

        char[] delimiter = { '\n' };
        string[] lines = print_msg.Split(delimiter);

        for (int i= 0; i < lines.Length-1; i++)
        {
            print(lines[i]);
        }

    }


    public void SwitchParenting()
    {
        print("Switched parenting from " + parenting.ToString() + " to " + (!parenting).ToString());
        parenting = !parenting;

        active_highlight ah = parenting_button.GetComponent<active_highlight>();

        if(parenting)
        {
            ah.EnableHighlight();
        }
        else
        {
            ah.DisableHighlight();
        }
    }


    public static ExistentialGraph ConvertToTree()
    {

        // convert everything to a tree

        // start by trying to parent everything, and stop if you cannot parent everything
        if ( !ParentAll() )
        {
            return null;
        }

        ParentAll();

        // initialize an IsoNode that is the canvas level

        // loop through all the cuts/variables and find all that are on the canvas level
        // method strategy : propogate down through
        // assumption: transform.root returns the highest parent transform, and if transform.root = self.transform, it is on the canvas layer

        List<circle_drawer> parent_cuts = new List<circle_drawer>();
        List<variable_drawer> parent_vars = new List<variable_drawer>();

        // all parent / canvas level cuts
        foreach (circle_drawer c in node_manager.all_cuts)
        {
            if (c.transform.root == c.transform)
            {
                parent_cuts.Add(c);
            }
        }

        // all parent / canvas level vars
        foreach (variable_drawer v in node_manager.all_vars)
        {
            if (v.transform.root == v.transform)
            {
                parent_vars.Add(v);
            }
        }

        //print("number of parent cuts: " + parent_cuts.Count);
        //print("number of parent variables: " + parent_vars.Count);

        // create canvas level IsoNode that we will add parent cuts and parent vars onto
        ExistentialGraph root = new ExistentialGraph.Cut();

        foreach( variable_drawer v in parent_vars)
        {
            ExistentialGraph iso_v = new ExistentialGraph.Var(v.var_name.text);
            root.Add_Subgraph(iso_v);
        }

        foreach (circle_drawer c in parent_cuts)
        {
            root.Add_Subgraph(make_cut(c));
        }

        UnParentAll();
        return root;

    }


    private static ExistentialGraph make_cut(circle_drawer cir)
    {
        ExistentialGraph iso_c = new ExistentialGraph.Cut();

        for( int i=0; i < cir.transform.childCount; i++ )
        {
            GameObject child = cir.transform.GetChild(i).gameObject;

            circle_drawer child_cut = child.GetComponent<circle_drawer>();
            variable_drawer child_var = child.GetComponent<variable_drawer>();

            if(child_cut != null)
            {
                iso_c.Add_Subgraph(make_cut(child_cut));
            }

            else if(child_var != null)
            {
                ExistentialGraph iso_v = new ExistentialGraph.Var(child_var.var_name.text);
                iso_c.Add_Subgraph(iso_v);
            }

            else
            {
                //print("this gameobject [" + child.name + "] does not have a circle_drawer component OR a variable_drawer component");
            }
        }
        
        return iso_c;
    }


    public static bool ParentAll()
    {
        // Check to see if there are intersecting circles before proceeding
        foreach (circle_drawer c in node_manager.all_cuts)
        {
            if (c.intersecting)
            {
                print("Can't enter parent mode -- you have cuts that are intersecting");
                return false;
            }
        }

        // Check to see if there are intersecting variables before proceeding
        foreach (variable_drawer v in node_manager.all_vars)
        {
            if (v.intersecting)
            {
                print("Can't enter parent mode -- you have variables that are intersecting");
                return false;
            }
        }

        // Start by unparenting all circles
        UnParentAll();

        // parent all circles
        foreach (circle_drawer a in node_manager.all_cuts)
        {
            circle_drawer smallest_parent = null;
            foreach (circle_drawer b in node_manager.all_cuts)
            {
                if ( b.radius > a.radius && a != b && node_manager.contains(b, a) )   // these are the only candidates that circle A could be parented to
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
                a.transform.SetParent(smallest_parent.transform);
            }
        }

        // parent all variables
        foreach (variable_drawer v in node_manager.all_vars)
        {
            circle_drawer smallest_parent = null;
            foreach (circle_drawer b in node_manager.all_cuts)
            {
                if (b.radius > node_manager.variable_selection_radius && node_manager.contains(b, v))   // these are the only candidates that circle A could be parented to
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

        return true;

    }

    public static void UnParentAll()
    {
        foreach (circle_drawer a in node_manager.all_cuts)
        {
            a.transform.SetParent(null);// = null;
        }

        foreach ( variable_drawer v in node_manager.all_vars )
        {
            v.transform.SetParent(null);// = null;
        }
    }


    



}