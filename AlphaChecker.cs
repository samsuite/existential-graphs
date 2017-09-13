using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
public class AlphaChecker //: IInferable
{

    private ExistentialGraph prev_step;
    private ExistentialGraph current_step;

	
	public bool A_Simple_Negation_Exists(ExistentialGraph prev, ExistentialGraph current) {
		
		HashSet<String> prev_children = new HashSet<String>(prev.Get_Immediate_Subgraphs().Select(graph => graph.Label()).ToList());
        HashSet<String> current_children = new HashSet<String>(current.Get_Immediate_Subgraphs().Select(graph => graph.Label()).ToList());
		
		foreach (String g in prev_children)
            if (current_children.Contains("((" + g + "))"))
               return true;

        foreach (String g in current_children)
            if (prev_children.Contains("((" + g + "))"))
               return true;	
			
        return false;
	}
	
    public bool A_DC_Exists(ExistentialGraph prev, ExistentialGraph current)
    {
		
        HashSet<String> prev_children = new HashSet<String>(prev.Get_Immediate_Subgraphs().Select(graph => graph.Label()).ToList());
        HashSet<String> current_children = new HashSet<String>(current.Get_Immediate_Subgraphs().Select(graph => graph.Label()).ToList());
		
		
		Counter<ExistentialGraph> prev_counts = new Counter<ExistentialGraph>();
		Counter<ExistentialGraph> current_counts = new Counter<ExistentialGraph>();
		foreach(ExistentialGraph g in prev.Get_Immediate_Subgraphs()) {
			prev_counts.Add(g);
		}
		
		foreach(ExistentialGraph g in current.Get_Immediate_Subgraphs()) {
			current_counts.Add(g);
		}
		
		foreach(ExistentialGraph g in prev.Get_Immediate_Subgraphs()) {
			
			if(current_counts.ContainsElement(g))
				current_counts.Decrement(g,1);
			
		}
		
		foreach(ExistentialGraph g in current.Get_Immediate_Subgraphs()) {
			
			if(prev_counts.ContainsElement(g))
				prev_counts.Decrement(g,1);
			
		}
		
		
		List<String> prev_leftovers = new List<String>();
		List<String> current_leftovers = new List<String>();
		
		foreach(ExistentialGraph g in prev_counts.Keys()) {
			
			for(int i = 0; i < prev_counts.Count(g); ++i) {
				prev_leftovers.Add(g.Label());
			}
			
		}
		
		foreach(ExistentialGraph g in current_counts.Keys()) {
			
			for(int i = 0; i < current_counts.Count(g); ++i) {
				current_leftovers.Add(g.Label());
			}
			
		}
		
		prev_leftovers.Sort();
		current_leftovers.Sort();
		
		if(prev_children.Count == 0)
			return false;
		
		string combined_prev_uniques = prev_leftovers.Count == 0 ? "" : prev_leftovers.Aggregate((acc, next) => acc + next);
		string combined_current_uniques = current_leftovers.Count == 0 ? "" : current_leftovers.Aggregate((acc, next) => acc + next);
		
		int count_prev_open_parens = combined_prev_uniques.Count(x => x == '(');
		int count_current_open_parens = combined_current_uniques.Count(x => x == '(');
		
		return ("((" + combined_prev_uniques + "))" == combined_current_uniques || "((" + combined_current_uniques + "))" == combined_prev_uniques); //||
				//((count_prev_open_parens + 2 * prev_leftovers.Count) == count_current_open_parens || (count_current_open_parens + 2 * current_leftovers.Count) == count_prev_open_parens);

    }
    private bool A_Difference_Exists(ExistentialGraph g1, ExistentialGraph g2)
    {
		
        return g1.Get_Immediate_Subgraphs().Count != g2.Get_Immediate_Subgraphs().Count;
    }

	private Counter<ExistentialGraph> Collect_Ancestors(ExistentialGraph sg) {
		
		Counter<ExistentialGraph> counts = new Counter<ExistentialGraph>();
		
		foreach(ExistentialGraph child in sg.Get_Immediate_Subgraphs()) {
			counts.Add(child);
		}
		
		ExistentialGraph parent = sg.Get_Parent();
		
		while(parent != null) {
			foreach(ExistentialGraph child in parent.Get_Immediate_Subgraphs()) {
				if(!child.Equals(parent))
					counts.Add(child);
			}
			parent = parent.Get_Parent();
		}
		return counts;
	}
	
	public int TotalNodesInGraph(ExistentialGraph g) {
		
		if(g.Is_Leaf()) {
			return 1;
		}
		
		int count = 0;
		foreach(ExistentialGraph sg in g.Get_Immediate_Subgraphs()) {
			
			count += 1 + TotalNodesInGraph(sg);
			
		}
		
		return count;
		
	}

	
	public Counter<ExistentialGraph> Collect_Leaf_Counts(ExistentialGraph g, Counter<ExistentialGraph> acc) {
		
		if(g.Is_Leaf()) {
			acc.Add(g);
		}
		
		foreach(ExistentialGraph sg in g.Get_Immediate_Subgraphs()) {
			Collect_Leaf_Counts(sg, acc);
		}
		
		return acc;
	}
	
	public bool Graphs_Differ_In_Contents_Not_Size(ExistentialGraph g1, ExistentialGraph g2) {
		
		if(TotalNodesInGraph(g1) != TotalNodesInGraph(g2))
			return false;
		
		Counter<ExistentialGraph> t1 = Collect_Leaf_Counts(g1, new Counter<ExistentialGraph>());
		Counter<ExistentialGraph> t2 = Collect_Leaf_Counts(g2, new Counter<ExistentialGraph>());
		
		foreach(ExistentialGraph g in t1.Keys()){
			if(t1.Count(g) != t2.Count(g))
				return true;
		}
		
		return false;
		
		
	}
	
		
		
    public Pair<ExistentialGraph, ExistentialGraph> Find_Difference_Between(ExistentialGraph previous_step, ExistentialGraph current_step)
    {

		//if (Graphs_Differ_In_Contents_Not_Size(prev_step, current_step))
			//return Pair.Make(prev_step, current_step);

		
        if (A_Simple_Negation_Exists(previous_step, current_step))
            return Pair.Make(previous_step, current_step);
        
        if (A_Difference_Exists(previous_step, current_step))
            return Pair.Make(previous_step, current_step);

        List<ExistentialGraph> labels_prev = previous_step.Get_Immediate_Subgraphs();
        List<ExistentialGraph> labels_current = current_step.Get_Immediate_Subgraphs();

        HashSet<ExistentialGraph> h1 = new HashSet<ExistentialGraph>(labels_prev);
        HashSet<ExistentialGraph> h2 = new HashSet<ExistentialGraph>(labels_current);
		
		Counter<ExistentialGraph> prev_counts = new Counter<ExistentialGraph>();
		Counter<ExistentialGraph> current_counts = new Counter<ExistentialGraph>();
		
		foreach(ExistentialGraph g in labels_prev) {
			prev_counts.Add(g);
		}
		
		foreach(ExistentialGraph g in labels_current) {
			current_counts.Add(g);
		}
		
        ExistentialGraph branch_to_consider_prev = new ExistentialGraph.Cut();
        ExistentialGraph branch_to_consider_current = new ExistentialGraph.Cut();

        labels_prev.ForEach(subgraph =>
        {
			
            if (current_counts.Count(subgraph) == 0)
                branch_to_consider_prev = subgraph;
			else 
				current_counts.Decrement(subgraph,1);
        }
        );

        labels_current.ForEach(subgraph =>
        {
            if (prev_counts.Count(subgraph) == 0)
                branch_to_consider_current = subgraph;
			else 
				prev_counts.Decrement(subgraph, 1);
        }
        );

        return Find_Difference_Between(branch_to_consider_prev, branch_to_consider_current);

    }

	public bool Is_Odd_Wrap(ExistentialGraph prev, ExistentialGraph current) {

		
		int count_prev_open_parens = prev.Label().Count(x => x == '(');
		int count_current_open_parens = current.Label().Count(x => x == '(');	
		
		return Math.Abs(count_current_open_parens - count_prev_open_parens) > 0 && (Math.Abs(count_current_open_parens - count_prev_open_parens)) % 2 == 1 && (prev.Label() == "(" + current.Label() + ")" || "(" + current.Label() == prev.Label() + ")");
		
	}
	
    public bool Is_Insertion(ExistentialGraph prev, ExistentialGraph current)
    {
		if(prev.Is_On_Even_Level() && current.Is_On_Even_Level()){
			Counter<ExistentialGraph> t1 = new Counter<ExistentialGraph>();
			Counter<ExistentialGraph> t2 = new Counter<ExistentialGraph>();
			List<ExistentialGraph> P = prev.Get_Immediate_Subgraphs();
			List<ExistentialGraph> C = current.Get_Immediate_Subgraphs();
			
			foreach(ExistentialGraph g in P) {
				t1.Add(g);
			}
			
			foreach(ExistentialGraph g in C) {
				t2.Add(g);
			}
			
			//Assert inserton conditions 
			foreach(ExistentialGraph sg in P) {
				if(!C.Contains(sg) || t1.Count(sg) != t2.Count(sg))
					return false;
			}
			
			//Assert that this is indeed an insertion as opposed to erasure
			return t2.TotalUniqueElements() > t1.TotalUniqueElements();
		
		}
		
		return false;
    }

    public bool Is_Erasure(ExistentialGraph prev, ExistentialGraph current)
    {

		if(prev.Is_On_Odd_Level() && current.Is_On_Odd_Level()) {

			Counter<ExistentialGraph> t1 = new Counter<ExistentialGraph>();
			Counter<ExistentialGraph> t2 = new Counter<ExistentialGraph>();
			List<ExistentialGraph> P = prev.Get_Immediate_Subgraphs();
			List<ExistentialGraph> C = current.Get_Immediate_Subgraphs();
			
			foreach(ExistentialGraph g in P) {
				t1.Add(g);
			}
			
			foreach(ExistentialGraph g in C) {
				t2.Add(g);
			}
			
			foreach(ExistentialGraph sg in C) {
				if(!P.Contains(sg) || t1.Count(sg) != t2.Count(sg)) 
					return false;
			}
			
			return t2.TotalUniqueElements() < t1.TotalUniqueElements();
		}
		return false;
    }

	
    public bool Is_Iteration(ExistentialGraph pre, ExistentialGraph current)
    {
		
		Counter<ExistentialGraph> t1 = Collect_Ancestors(pre);
		Counter<ExistentialGraph> t2 = Collect_Ancestors(current);
		List<ExistentialGraph> k1 = t1.Keys();
		List<ExistentialGraph> k2 = t2.Keys();
		
		foreach(ExistentialGraph c2 in current.Get_Immediate_Subgraphs()) {
			if(!pre.Get_Immediate_Subgraphs().Contains(c2) && !k1.Contains(c2)){ 
				return false;
			}
		}
				
		foreach(ExistentialGraph c2 in pre.Get_Immediate_Subgraphs()) {
			if(!current.Get_Immediate_Subgraphs().Contains(c2)){ 
				return false;
			}
		}
		
		foreach(ExistentialGraph c2 in current.Get_Immediate_Subgraphs()) {
			if(t1.Count(c2) > t2.Count(c2)) {
				return false;
			}
		}	


		foreach(ExistentialGraph c1 in pre.Get_Immediate_Subgraphs()){
			if(t1.Count(c1) > t2.Count(c1)) {
				return false;
			}
		}
		
		return true;

    }


    public bool Is_Deiteration(ExistentialGraph pre, ExistentialGraph current)
    {

		Counter<ExistentialGraph> t1 = Collect_Ancestors(pre);
		Counter<ExistentialGraph> t2 = Collect_Ancestors(current);
		List<ExistentialGraph> k1 = t1.Keys();
		List<ExistentialGraph> k2 = t2.Keys();
				
			
		foreach(ExistentialGraph c2 in pre.Get_Immediate_Subgraphs()) {
			if(!current.Get_Immediate_Subgraphs().Contains(c2) && !k2.Contains(c2)){ 
				return false;
			}
		}
		
		foreach(ExistentialGraph c2 in current.Get_Immediate_Subgraphs()) {
			if(!pre.Get_Immediate_Subgraphs().Contains(c2)){ 
				return false;
			}
		}
		
		foreach(ExistentialGraph c2 in current.Get_Immediate_Subgraphs()) {
			if(t1.Count(c2) < t2.Count(c2)) 
				return false;
		}	

		foreach(ExistentialGraph c1 in pre.Get_Immediate_Subgraphs()){
			if(t1.Count(c1) < t2.Count(c1))
				return false;
		}
		
		return true;
    }

}
