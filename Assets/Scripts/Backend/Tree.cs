using System;
using System.Collections.Generic;

///-----------------------------------------------------------------
///   Namespace:      N/A
///   Class:          Tree
///   Description:    Implementation of an N-Ary Tree for use in EGs
///-----------------------------------------------------------------

public class Tree
{

    /* Properties and Fields */

    public ISONode root;

    /* Constructors */

    public Tree()
    {
        this.root = new ISONode();
    }

    public Tree(ISONode r)
    {
        this.root = r;
    }

    /*  Private Helper Methods */

    private int Assign_Label(ISONode root)
    {
        if(root.Is_Leaf()) { return 1; }

        int label = 0;

        foreach(ISONode child in root.getChildren())
        {
            label +=  1 + Assign_Label(child);
        }

        return label;
    }

    private int Num_Leaves(ISONode n)
    {
        int leaves = 0;

        foreach(ISONode child in n.getChildren())
        {
            if(child.Is_Leaf())
            {
                leaves += 1;
            }
        }

        return leaves;
    }

    private bool A_Difference_Exists_Between(ISONode n1, ISONode n2)
    {
        return n1.getChildren().Count != n2.getChildren().Count || Num_Leaves(n1) != Num_Leaves(n2);
    }

    private bool Isomorphic_Pair_Exists(List<ISONode> L1, List<ISONode> L2)
    {
        foreach(ISONode n in L2)
        {
            foreach(ISONode m in L1)
            {
                if (SubTrees_Are_Isomorphic(n,m))
                {
                    L1.Remove(m);
                    L2.Remove(n);
                    return true;
                }
            }
        }
        return false;
    }

    private ISONode Find_Difference(ISONode root_of_modified_tree, ISONode root_of_original_tree)
    {

        ISONode found_ISONode = null;

        if(SubTrees_Are_Isomorphic(root_of_modified_tree, root_of_original_tree))
        {
            return found_ISONode;
        }

        if(A_Difference_Exists_Between(root_of_modified_tree, root_of_original_tree))
        {
            return root_of_original_tree;
        }

        if(root_of_modified_tree.Is_Leaf() || root_of_original_tree.Is_Leaf())
        {
            return found_ISONode;
        }

        List<ISONode> mod_children = new List<ISONode>(root_of_modified_tree.getChildren());
        List<ISONode> orig_children = new List<ISONode>(root_of_original_tree.getChildren());

        while(Isomorphic_Pair_Exists(mod_children, orig_children));

        found_ISONode = Find_Difference(mod_children[0], orig_children[0]);

        return found_ISONode;

    }

    /* Public Methods */

    public bool Is_Isomorphic_With(Tree other)
    {
        return Assign_Label(this.root) == Assign_Label(other.root);
    }

    public bool SubTrees_Are_Isomorphic(ISONode n1, ISONode n2)
    {
        return Assign_Label(n1) == Assign_Label(n2);
    }


    public ISONode Find_Difference(Tree other)
    {
        return this.Find_Difference(this.root, other.root);
    }

    public void Remove_SubGraph(ISONode root_of_subtree)
    {

        if(root_of_subtree.Is_Leaf())
        {
            return;
        }

        foreach(ISONode child in root_of_subtree.getChildren())
        {
            Remove_SubGraph(child);
        }

        root_of_subtree.parent.getChildren().Remove(root_of_subtree);
        root_of_subtree = null;
    }


    public void Remove_Double_Cut(ISONode n)
    {

        /* This is ugly, will change it to be better looking later */
        n.parent.getChildren().Add(n.getChildren()[0].getChildren()[0]);
        ISONode temp1 = n.getChildren()[0];
        ISONode temp2 = n.getChildren()[0].getChildren()[0];
        n.getChildren()[0].Remove(temp2);
        temp2 = null;
        n.getChildren().Remove(temp1);
        temp1 = null;

    }
}
