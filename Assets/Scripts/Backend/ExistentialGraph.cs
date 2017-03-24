using System;
using System.Collections.Generic;




/*


    Rules for Inference Rules as noted in file InferenceRules.cs

    Two Arguments:
        Node n <- Head node
        Parameters <- Parameters for Node (Location data, id, etc)  <- NOT IMPLEMENTED YET


*/
public class ExistentialGraph
{

    /* Properties */

    private Tree _graph;


    /* Constructor */

    public ExistentialGraph(ISONode sheet_of_assertion)
    {
        this._graph = new Tree(sheet_of_assertion);
    }

    public ExistentialGraph(Tree t)
    {
        this._graph = t;
    }

    /* Private Helper Methods */

    private bool Can_Place_DC(ISONode n)
    {
        return true;
    }

    private bool Can_Remove_DC(ISONode n)
    {
        if (n.is_cut && n.children.Count == 1 && n.children[0].is_cut){
            return true;
        }
        return false;
    }

    private bool Can_Iterate(ISONode n)
    {
        return true;
    }

    /*  This might have to change depending on input node */
    private bool Can_Deiterate(ISONode n)
    {
        if(n.parent == null)
        {
            return false;
        }

        foreach(ISONode child in n.parent.getChildren())
        {

            if(this._graph.SubTrees_Are_Isomorphic(child, n))
            {
                return true;
            }

        }

        return Can_Deiterate(n.parent);
    }


    private bool Can_Insert(ISONode n)
    {
        return n.Is_On_Odd_Level();
    }



    private bool Can_Erase(ISONode n)
    {
        return n.Is_On_Even_Level();
    }

    /* Inference Rules */ // WORK IN PROGRESS
    public void Insert(ISONode location_to_add,ISONode subgraph)
    {

        if(!Can_Insert(location_to_add))
        {
            return;
        }

        location_to_add.Add_Child(subgraph);

    }

    public void Erase(ISONode location_to_perform_erasure, ISONode subgraph)
    {
        if(!Can_Erase(location_to_perform_erasure))
        {
            return;
        }

        this._graph.Remove_SubGraph(subgraph);
    }

    public void Apply_Double_Cut(ISONode location_to_add)
    {

        ISONode parent = location_to_add.parent;
        parent.Remove_Child(location_to_add);
        ISONode cut1 = new ISONode();
        cut1.Init_As_Cut();
        ISONode cut2 = new ISONode();
        cut2.Init_As_Cut();
        cut2.Add_Child(location_to_add);
        cut1.Add_Child(cut2);
        parent.Add_Child(cut1);

    }

    public void Remove_Double_Cut(ISONode location)
    {
        this._graph.Remove_Double_Cut(location);
    }

    public void Iterate(ISONode location, ISONode subgraph)
    {
        if(!Can_Iterate(location))
        {
            return;
        }

        Insert(location, subgraph);
    }

    public void Deiterate(ISONode subgraph)
    {
        if(!Can_Deiterate(subgraph))
        {
            return;
        }

        Erase(subgraph.parent, subgraph);
    }

    /*  Functions to add */
    // IsInstanceOf <rule> methods
    // verify methods




}
