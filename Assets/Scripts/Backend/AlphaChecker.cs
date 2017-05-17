using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
public class AlphaChecker //: IInferable
{

    private ExistentialGraph prev_step;
    private ExistentialGraph current_step;

    public bool A_DC_Exists(ExistentialGraph prev, ExistentialGraph current)
    {

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
    private bool A_Difference_Exists(ExistentialGraph g1, ExistentialGraph g2)
    {

		
        return g1.Get_Immediate_Subgraphs().Count != g2.Get_Immediate_Subgraphs().Count;
    }

    public Pair<ExistentialGraph, ExistentialGraph> Find_Difference_Between(ExistentialGraph previous_step, ExistentialGraph current_step)
    {

        if (A_DC_Exists(previous_step, current_step))
        {
            return Pair.Make(previous_step, current_step);
        }
		
        if (A_Difference_Exists(previous_step, current_step))
            return Pair.Make(previous_step, current_step);

        List<ExistentialGraph> labels_prev = previous_step.Get_Immediate_Subgraphs();
        List<ExistentialGraph> labels_current = current_step.Get_Immediate_Subgraphs();

        HashSet<ExistentialGraph> h1 = new HashSet<ExistentialGraph>(labels_prev);
        HashSet<ExistentialGraph> h2 = new HashSet<ExistentialGraph>(labels_current);

        ExistentialGraph branch_to_consider_prev = new ExistentialGraph.Cut();
        ExistentialGraph branch_to_consider_current = new ExistentialGraph.Cut();

        labels_prev.ForEach(subgraph =>
        {
            if (!h2.Contains(subgraph))
                branch_to_consider_prev = subgraph;
        }
        );

        labels_current.ForEach(subgraph =>
        {
            if (!h1.Contains(subgraph))
                branch_to_consider_current = subgraph;
        }
        );

        return Find_Difference_Between(branch_to_consider_prev, branch_to_consider_current);

    }

    public bool Is_Insertion(ExistentialGraph prev, ExistentialGraph current)
    {
        /* We assume the difference point is one level above, so this shouldnt map logically
         * to what you may expect from Pierce's theory */
        return prev.Is_On_Even_Level() && current.Is_On_Even_Level()
               && prev.Get_Immediate_Subgraphs().Count < current.Get_Immediate_Subgraphs().Count;
    }

    public bool Is_Erasure(ExistentialGraph prev, ExistentialGraph current)
    {
        /* We assume the difference point is one level above, so this shouldnt map logically
        * to what you may expect from Pierce's theory */

        return prev.Is_On_Odd_Level() && current.Is_On_Odd_Level()
               && prev.Get_Immediate_Subgraphs().Count > current.Get_Immediate_Subgraphs().Count;
    }

    private List<ExistentialGraph> Generate_Iteration_Candidates(ExistentialGraph current)
    {
		List<ExistentialGraph> candidates = new List<ExistentialGraph>();
        foreach (ExistentialGraph g in current.Get_Immediate_Subgraphs())
        {
			
            if (g.Exists_In_Upper_Subgraph())
                candidates.Add(g);
        }
        return candidates;
    }
	
	private List<ExistentialGraph> Add_In_Level_Candidates_Iter(ExistentialGraph g1, ExistentialGraph g2, List<ExistentialGraph> candidates)
	{
		foreach(ExistentialGraph g in g2.Get_Immediate_Subgraphs())
		{
			Debug.Log(g.ToString());
			int count1 = g1.Get_Immediate_Subgraphs().Where(subgraph => subgraph.Equals(g)).Count();
			int count2 = g2.Get_Immediate_Subgraphs().Where(subgraph => subgraph.Equals(g)).Count();
			Debug.Log(count1 + count2);
			if(count2 > count1 && count1 > 0)
				candidates.Add(g);
		}
		return candidates;
	}

	private List<ExistentialGraph> Add_In_Level_Candidates_DeIter(ExistentialGraph g1, ExistentialGraph g2, List<ExistentialGraph> candidates)
	{
		foreach(ExistentialGraph g in g1.Get_Immediate_Subgraphs())
		{
			Debug.Log(g.ToString());
			int count1 = g1.Get_Immediate_Subgraphs().Where(subgraph => subgraph.Equals(g)).Count();
			int count2 = g2.Get_Immediate_Subgraphs().Where(subgraph => subgraph.Equals(g)).Count();
			Debug.Log(count1 + count2);
			if(count2 < count1 && count1 > 1)
				candidates.Add(g);
		}
		return candidates;
	}
	
    public bool Is_Iteration(ExistentialGraph pre, ExistentialGraph current)
    {
		List<ExistentialGraph> candidates = Generate_Iteration_Candidates(current);
		candidates = Add_In_Level_Candidates_Iter(pre, current,candidates);
		bool Iteration_Rule_Possible = candidates.Count != 0;
		Debug.Log(candidates.Count);
		if(!Iteration_Rule_Possible)
			return false;
		
		
        List<ExistentialGraph> prev_subgraphs = pre.Get_Immediate_Subgraphs();
        List<ExistentialGraph> current_subgraphs = current.Get_Immediate_Subgraphs();


		if (prev_subgraphs.Count == 0 && current_subgraphs.Count > 0 && pre.Get_Parent() == null)
			return false;
		
		
        bool added_flag = false;

        HashSet<ExistentialGraph> curr = new HashSet<ExistentialGraph>(current_subgraphs);
		HashSet<ExistentialGraph> prev = new HashSet<ExistentialGraph>(prev_subgraphs);
		
        foreach (ExistentialGraph g in candidates)
        {
	
            bool count_flag = (prev_subgraphs.Where(child => g.Equals(child)).Count() < current_subgraphs.Where(child => g.Equals(child)).Count());
            if (count_flag && !added_flag)
                added_flag = true;

        }

        return (added_flag) ? true : false;

    }


    public bool Is_Deiteration(ExistentialGraph pre, ExistentialGraph current)
    {

		List<ExistentialGraph> candidates = Generate_Iteration_Candidates(pre);
		candidates = Add_In_Level_Candidates_DeIter(pre, current,candidates);
		bool Iteration_Rule_Possible = candidates.Count != 0;
		Debug.Log("CANDIDATE COUNT: " + candidates.Count.ToString());
		if(!Iteration_Rule_Possible)
			return false;
		
		
        List<ExistentialGraph> prev_subgraphs = pre.Get_Immediate_Subgraphs();
        List<ExistentialGraph> current_subgraphs = current.Get_Immediate_Subgraphs();


		if (prev_subgraphs.Count > 0 && current_subgraphs.Count == 0 && pre.Get_Parent() == null)
			return false;
		
		
        bool deletion_flag = false;

        HashSet<ExistentialGraph> curr = new HashSet<ExistentialGraph>(current_subgraphs);
		HashSet<ExistentialGraph> prev = new HashSet<ExistentialGraph>(prev_subgraphs);
		
        foreach (ExistentialGraph g in candidates)
        {
	
            bool count_flag = (prev_subgraphs.Where(child => g.Equals(child)).Count() > current_subgraphs.Where(child => g.Equals(child)).Count());
			Debug.Log("COUNT_FLAG:" + count_flag.ToString());
            if (count_flag && !deletion_flag)
                deletion_flag = true;

        }

        return (deletion_flag) ? true : false;

    }

}
