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

    public Node root { get; }

    /*
        Constructor(s)
    */

    public Tree()
    {
        this.root = new Node();
    }

    public Tree(Node r)
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

    private int Height_Helper(Node root, int height)
    {
        if(root.Is_Leaf())
        {
            return height;
        }

        List<Node> children = root.children;
        int max_so_far = 0;

        foreach(Node child in children)
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
    private void Nodes_By_Level_Helper(Node root, int level, ref Dictionary<int, List<Node> > levels)
    {

        if(!levels.ContainsKey(level))
        {
            levels[level] = new List<Node>();
        }

        levels[level].Add(root);

        List<Node> children = root.children;

        foreach(Node child in children)
        {

            Nodes_By_Level_Helper(child, level + 1, ref levels);

        }

    }


    /*

        Function Name:  Nodes_By_Level
        Parameters:  None
        Description:  Returns a Dictionary with mappings of level to nodes by level

    */
    public Dictionary<int, List<Node> > Nodes_By_Level()
    {

        Dictionary<int, List<Node> > levels = new Dictionary<int, List<Node> >();
        Nodes_By_Level_Helper(this.root, 0, ref levels);
        return levels;
    }



    /*

        Function Name:  Labels_By_Level_Helper
        Parameters:  root: Node, Dict<Node, string>
        Description:    Recursively traverses the tree, updating labels on the way up

    */
    public void Labels_By_Level_Helper(Node root, ref Dictionary<Node, string> labels)
    {

        if(root.Is_Leaf())
        {
            labels[root] = "10";

        }
        else
        {
            List<Node> children = root.children;

            foreach(Node child in children)
            {
                Labels_By_Level_Helper(child, ref labels);
            }

            List<string> sorted_children = new List<string>();


            foreach(Node child in children)
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
    public Dictionary<Node, string> Labels_By_Level()
    {
        Dictionary<Node, string> labels = new Dictionary<Node, string>();
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

        Node r1 = this.root;
        Node r2 = other.root;
        Dictionary<Node, string> labels1 = this.Labels_By_Level();
        Dictionary<Node, string> labels2 = other.Labels_By_Level();

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
