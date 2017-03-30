using System;
using System.Collections.Generic;

public class ISONode
{

    /* Properties and Fields */

    public List<ISONode> children;
    public ISONode parent;
    public bool is_var;
    public bool is_cut;
    public string value;

    /* Constructor(s) */
    public ISONode()
    {
        this.children = new List<ISONode>();
        this.parent = null;
    }

    public void Init_As_Cut()
    {
        this.is_cut = true;
        this.is_var = false;
        this.value = "cut";

    }

    public void Init_As_Var(string name)
    {
        this.is_cut = false;
        this.is_var = true;
        this.value = name;
    }

    /* Private Helper Methods */

    private int Depth_Helper(ISONode n, int depth)
    {
        if(n == null)
        {
            return depth;
        }
        return Depth_Helper(n.parent, depth + 1);

    }

    private int Number_Of_Cuts(ISONode n, int cut_count)
    {
        if (n == null)
        {
            return cut_count;
        }

        if(n.is_cut)
        {
            return Number_Of_Cuts(n.parent, cut_count + 1);
        }

        return Number_Of_Cuts(n.parent, cut_count);
    }

    /*  Public Methods  */

    public bool Is_Leaf()
    {
        return this.children.Count == 0;
    }

    public bool Has_Nested_Areas()
    {
        if (this.is_cut)
        {
            foreach(ISONode child in this.children)
            {
                if(child.is_cut)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Add_Child(ISONode n)
    {

        n.parent = this;
        this.children.Add(n);

    }

    public void Remove_Child(ISONode n)
    {
        this.children.Remove(n);
    }

    public int Depth()
    {
        return Depth_Helper(this,0);
    }

    public List<ISONode> Get_Children()
    {
        return this.children;
    }

    public bool Is_On_Even_Level()
    {
        return Number_Of_Cuts(this, 0) % 2 == 1;
    }

    public bool Is_On_Odd_Level()
    {
        return !Is_On_Even_Level();
    }
    public override string ToString()
    {

        /* original
        return this.value;
        */

        // new and improved ;)
        string msg = "";

        for( int i = 0 ; i < this.Depth() - 1; i++ )
            msg += "   ";

        msg += this.value + '\n';
``
        // percolate step
        foreach ( ISONode child in this.getChildren() )
        {
            msg += child.ToString();
        }

        return msg;
    }

}
