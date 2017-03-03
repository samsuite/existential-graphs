using System.Collections.Generic;
using System;

///-----------------------------------------------------------------
///   Namespace:      N/A
///   Class:          Tree
///   Description:    Implementation of an N-Ary Tree for use in EGs
///-----------------------------------------------------------------

public class Tree
{

    /*
        Properties and Fields
    */

    public IsoNode root;// { get; }

    /*
        Constructor(s)
    */

    public Tree()
    {
        this.root = new IsoNode();
    }

    public Tree(IsoNode r)
    {
        this.root = r;
    }

    /*
        Methods
    */


    /*

        Function Name:  Height_Helper
        Parameters:  root: Node, height: int
        Description:  Helper Function to handle recursive calculation of height of tree

    */

    private int Height_Helper(IsoNode root, int height)
    {
        if(root.Is_Leaf())
        {
            return height;
        }

        List<IsoNode> children = root.children;
        int max_so_far = 0;

        foreach(IsoNode child in children)
        {
            max_so_far = Math.Max(max_so_far, Height_Helper(child, height + 1));
        }

        return max_so_far;
    }

    /*

        Function Name:  Height
        Parameters:  None
        Description:  Calculates the height of the tree

    */
    public int Height()
    {
        return Height_Helper(this.root, 0);
    }


    /*

        Function Name:  Nodes_By_Level_Helper
        Parameters:  root: Node, level: int, levels: Dict<int, List<Node>
        Description:  Helper Function to handle retrieving nodes by level in tree (DFS)

    */
    private void Nodes_By_Level_Helper(IsoNode root, int level, ref Dictionary<int, List<IsoNode> > levels)
    {

        if(!levels.ContainsKey(level))
        {
            levels[level] = new List<IsoNode>();
        }

        levels[level].Add(root);

        List<IsoNode> children = root.children;

        foreach(IsoNode child in children)
        {

            Nodes_By_Level_Helper(child, level + 1, ref levels);

        }

    }


    /*

        Function Name:  Nodes_By_Level
        Parameters:  None
        Description:  Returns a Dictionary with mappings of level to nodes by level

    */
    public Dictionary<int, List<IsoNode> > Nodes_By_Level()
    {

        Dictionary<int, List<IsoNode> > levels = new Dictionary<int, List<IsoNode> >();
        Nodes_By_Level_Helper(this.root, 0, ref levels);
        return levels;
    }



    /*

        Function Name:  Labels_By_Level_Helper
        Parameters:  root: Node, Dict<Node, string>
        Description:    Recursively traverses the tree, updating labels on the way up

    */
    public void Labels_By_Level_Helper(IsoNode root, ref Dictionary<IsoNode, string> labels)
    {

        if(root.Is_Leaf())
        {
            labels[root] = "10";

        }
        else
        {
            List<IsoNode> children = root.children;

            foreach(IsoNode child in children)
            {
                Labels_By_Level_Helper(child, ref labels);
            }

            List<string> sorted_children = new List<string>();


            foreach(IsoNode child in children)
            {

                sorted_children.Add(labels[child]);

            }

            sorted_children.Sort();

            string name = "";

            foreach(string label in sorted_children)
            {

                name += "1" + label + "0";
                // Console.WriteLine(label);

            }
            Console.WriteLine(name);

            labels[root] = name;
        }

    }


    /*

        Function Name:  Labels_By_Level
        Parameters:  None
        Description:    Returns a Dictionary containing mappings from nodes to canonical names

    */
    public Dictionary<IsoNode, string> Labels_By_Level()
    {
        Dictionary<IsoNode, string> labels = new Dictionary<IsoNode, string>();
        this.Labels_By_Level_Helper(this.root, ref labels);
        return labels;

    }

    /*

        Function Name:  Is_Isomorphic_With
        Parameters:  other: Tree
        Description:    Determines whether two trees are isomorphic

    */
    public bool Is_Isomorphic_With(Tree other)
    {

        IsoNode r1 = this.root;
        IsoNode r2 = other.root;
        Dictionary<IsoNode, string> labels1 = this.Labels_By_Level();
        Dictionary<IsoNode, string> labels2 = other.Labels_By_Level();

        // Console.WriteLine(labels1[r1]);
        // Console.WriteLine(labels2[r2]);
        if(labels1[r1] == labels2[r2])
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
