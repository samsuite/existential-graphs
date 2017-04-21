using System;
using System.Linq;
using System.Collections.Generic;

public class AlphaChecker //: IInferable
{

    private ExistentialGraph prev_step;
    private ExistentialGraph current_step;


    private bool A_DC_Exists(ExistentialGraph g1, ExistentialGraph g2)
    {
        return g1.Label() == "((" + g2.Label() + "))" || g2.Label() == "((" + g1.Label() + "))";
    }

    private bool A_Difference_Exists(ExistentialGraph g1, ExistentialGraph g2)
    {
        return g1.Get_Immediate_Subgraphs().Count != g2.Get_Immediate_Subgraphs().Count;
    }

    public Pair<ExistentialGraph, ExistentialGraph> Find_Difference_Between(ExistentialGraph previous_step, ExistentialGraph current_step)
    {

        if (A_DC_Exists(previous_step, current_step))
        {
            if (previous_step.Get_Parent() == null || current_step.Get_Parent() == null)
                return Pair.Make(previous_step, current_step);
            return Pair.Make(previous_step.Get_Parent(), current_step.Get_Parent());
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


    private bool DC_Rule(ExistentialGraph prev, ExistentialGraph current)
    {
        return prev.Label() == "((" + current.Label() + "))"
            || current.Label() == "((" + prev.Label() + "))";
    }

    public bool Is_Double_Cut(ExistentialGraph prev, ExistentialGraph current)
    {

        HashSet<String> prev_children = new HashSet<String>(prev.Get_Immediate_Subgraphs().Select(graph => graph.Label()).ToList());
        HashSet<String> current_children = new HashSet<String>(current.Get_Immediate_Subgraphs().Select(graph => graph.Label()).ToList());
        if (prev_children.Count != current_children.Count)
            return false;

        foreach (String g in prev_children)
            if (current_children.Contains("((" + g + "))"))
                return true;

        foreach (String g in current_children)
            if (prev_children.Contains("((" + g + "))"))
                return true;
        return false;
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

    private bool Iteration_Rule_Possible(ExistentialGraph current)
    {
        foreach (ExistentialGraph g in current.Get_Immediate_Subgraphs())
        {
            if (g.Exists_In_Upper_Subgraph())
                return true;
        }
        return false;
    }

    public bool Is_Iteration(ExistentialGraph pre, ExistentialGraph current)
    {
        if (!Iteration_Rule_Possible(current))
            return false;

        List<ExistentialGraph> prev_subgraphs = pre.Get_Immediate_Subgraphs();
        List<ExistentialGraph> current_subgraphs = current.Get_Immediate_Subgraphs();

        if (prev_subgraphs.Count == current_subgraphs.Count)
            return false;

        bool added_flag = false;

        HashSet<ExistentialGraph> curr = new HashSet<ExistentialGraph>(current_subgraphs);

        foreach (ExistentialGraph g in curr)
        {
            bool count_flag = prev_subgraphs.Where(child => g.Equals(child)).Count() < current_subgraphs.Where(child => g.Equals(child)).Count();

            if (count_flag && added_flag)
                return false;
            if (count_flag && !added_flag)
                added_flag = true;

        }

        return (added_flag) ? true : false;

    }


    public bool Is_Deiteration(ExistentialGraph pre, ExistentialGraph current)
    {

        if (!Iteration_Rule_Possible(pre))
            return false;

        List<ExistentialGraph> prev_subgraphs = pre.Get_Immediate_Subgraphs();
        List<ExistentialGraph> current_subgraphs = current.Get_Immediate_Subgraphs();
        if (prev_subgraphs.Count == current_subgraphs.Count)
            return false;

        bool deletion_flag = false;
        HashSet<ExistentialGraph> prev = new HashSet<ExistentialGraph>(prev_subgraphs);

        foreach (ExistentialGraph g in prev)
        {
            bool count_flag = prev_subgraphs.Where(child => g.Equals(child)).Count() > current_subgraphs.Where(child => g.Equals(child)).Count();
            if (count_flag && deletion_flag)
                return false;
            if (count_flag && !deletion_flag)
                deletion_flag = true;
        }

        return (deletion_flag) ? true : false;
    }

}
