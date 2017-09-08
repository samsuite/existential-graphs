using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExistentialGraph
{
    private List<ExistentialGraph> subgraphs;
    private Cut parent;
    private string label;

    public abstract bool Is_Cut();
    public abstract bool Is_Var();
    public abstract bool Is_Leaf();
    public abstract bool Has_Nested_Subgraphs();
    public abstract List<ExistentialGraph> Get_Immediate_Subgraphs();
    public abstract void Add_Subgraph(ExistentialGraph g);

    public ExistentialGraph Get_Parent()
    {
        return this.parent;
    }

    public string Label()
    {
	
		this.Assign_Label();
        return this.label;
    }


    public bool Exists_In_Upper_Subgraph()
    {

		if(this.Level() <= 2)
			return false;
	
        ExistentialGraph parent = this.Get_Parent().Get_Parent();
	
		List<ExistentialGraph> parent_children = new List<ExistentialGraph>();
		
        while (parent.Get_Parent() != null)
        {

            parent = parent.Get_Parent();
            parent_children = parent.Get_Immediate_Subgraphs();

            if (parent_children.Contains(this))
                return true;
        }
        parent_children = parent.Get_Immediate_Subgraphs();

        if (parent_children.Contains(this))
            return true;

        return false;

    }

    public bool Exists_In_Level()
    {
        return this.Get_Parent().Get_Immediate_Subgraphs().Where(subgraph => this.Equals(subgraph)).Count() >= 1;
    }

    private int Num_Cuts()
    {
        return (this.Get_Parent() == null) ? 0 : 1 + this.Get_Parent().Num_Cuts();
    }

    public bool Is_On_Even_Level()
    {
        return this.Num_Cuts() % 2 == 1;
    }

    public bool Is_On_Odd_Level()
    {
        return !this.Is_On_Even_Level();
    }

    public bool Is_Subgraph_Of(ExistentialGraph g)
    {
        if (this.Equals(g))
            return true;

        ExistentialGraph current = this;

        while (current.Get_Parent() != null)
        {
            current = current.Get_Parent();
            if (this.Equals(current))
                return true;
        }

        return false;
    }

    public int Level()
    {
        return (this.Get_Parent() == null) ? 1 : 1 + this.Get_Parent().Level();
    }


    private void Assign_Label()
    {

        if (this.Is_Leaf())
            return;

        this.subgraphs.ForEach(subgraph => subgraph.Assign_Label());

        List<string> child_labels = this.subgraphs.Select(subgraph => subgraph.label).ToList();
        child_labels.Sort();

        this.label = "(" + child_labels.Aggregate("", (acc, subgraph) => acc + subgraph) + ")";
    }


    public override bool Equals(object other)
    {
        return ((other as ExistentialGraph) == null) ? false : this.Label() == (other as ExistentialGraph).Label();
    }

    public override int GetHashCode()
    {
        return this.Label().GetHashCode();
    }

    public override string ToString()
    {
        string msg = "";

        for (int i = 0; i < this.Level() - 1; i++)
            msg += "   ";

        msg += (this.Is_Cut())? "cut\n" : this.Label() + '\n';

        if (this.Is_Var())
            return msg;

        foreach (ExistentialGraph child in this.Get_Immediate_Subgraphs())
        {
            msg += child.ToString();
        }

        return msg;
    }

    public sealed class Cut : ExistentialGraph
    {

        public Cut()
        {
            this.parent = null;
            this.subgraphs = new List<ExistentialGraph>();
            this.label = "()";
        }

        public override bool Is_Cut()
        {
            return true;
        }
        public override bool Is_Var()
        {
            return false;
        }
        public override bool Is_Leaf()
        {
            return this.subgraphs.Count == 0;
        }

        public override bool Has_Nested_Subgraphs()
        {
            return this.subgraphs.Where(subgraph => subgraph.Is_Cut()).Count() > 0;
        }
        public override List<ExistentialGraph> Get_Immediate_Subgraphs()
        {
            return this.subgraphs;
        }

   

        public override void Add_Subgraph(ExistentialGraph g)
        {
            g.parent = this;
            this.subgraphs.Add(g);
        }

    }


    public sealed class Var : ExistentialGraph
    {

        public Var(string label)
        {
            this.parent = new Cut();
            this.label = label;
        }

        public override bool Is_Cut()
        {
            return false;
        }

        public override bool Is_Var()
        {
            return true;
        }

        public override bool Is_Leaf()
        {
            return true;
        }

        public override bool Has_Nested_Subgraphs()
        {
            return false;
        }

        public override List<ExistentialGraph> Get_Immediate_Subgraphs()
        {
            return new List<ExistentialGraph>();
        }

        public override void Add_Subgraph(ExistentialGraph g)
        {
            throw new Exception("Var can't have child subgraphs!");
        }

    }
}