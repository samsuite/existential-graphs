using System;
using System.Collections.Generic;

public class IsoNode
{

    /*

        Member Variables

    */

    public List<IsoNode> children;// { get; }
    public IsoNode parent;// { get; set;}
    public bool is_var;// { get; }
    public bool is_cut;// { get; }
    public bool is_root;// { get; }

    public string value { get; set; }

    public IsoNode()
    {
        this.children = new List<IsoNode>();
        // this.is_var = false;
        // this.is_cut = false;
        // this.is_root = true;
    }
    //
    // public Node(bool variable = false)
    // {
    //     this.children = new List<Node>();
    //     this.is_var = variable;
    //     this.is_cut = !variable;
    //     this.is_root = false;
    // }
    //
    // public Node(List<Node> children, bool variable = false,  bool root = true)
    // {
    //     this.children = children;
    //
    //     foreach(Node child in children)
    //     {
    //         child.parent = this;
    //     }
    //
    //     this.is_var = variable;
    //     this.is_cut = !variable;
    //     this.is_root = root;
    // }


    /*

        Methods that are valid for every type of node

    */
    public bool Is_Leaf()
    {
        return this.children.Count == 0;
    }

    public void Add_Child(IsoNode n)
    {

        n.parent = this;
        this.children.Add(n);

    }

    private int Depth_Helper(IsoNode n, int depth)
    {
        if(n.is_root)
        {
            return depth;
        }
        return Depth_Helper(n.parent, depth + 1);

    }
    public int Depth()
    {
        return Depth_Helper(this,0);
    }

}
