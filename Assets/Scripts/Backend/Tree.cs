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

    private int Height_Of_SubTree(ISONode root_of_subtree)
    {

        if(root_of_subtree.Is_Leaf())
        {
            return 1;
        }

        int height = 0;

        foreach(ISONode child in root_of_subtree.getChildren())
        {
             height = max(height, Height_Of_SubTree(child));
        }

        return 1 + height;
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
        return n1.getChildren().Count != n2.getChildren().Count || Height_Of_SubTree(n1) != Height_Of_SubTree(n2);
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
        List<ISONode> orig_children= new List<ISONode>(root_of_original_tree.getChildren());

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


        ISONode subgraph_to_move_up = n.getChildren()[0].getChildren()[0].getChildren()[0];
        subgraph_to_move_up.parent = n;

        ISONode temp1 = n.getChildren()[0];
        ISONode temp2 = n.getChildren()[0].getChildren()[0];

        temp2.Remove(subgraph_to_move_up);
        temp1.Remove(temp2);
        temp2 = null;
        temp1 = null;

        n.Add_Child(subgraph_to_move_up);
    }
}
