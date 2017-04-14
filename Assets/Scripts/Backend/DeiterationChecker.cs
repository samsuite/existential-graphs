using System;
using System.Linq;
using System.Collections.Generic;


public class DeiterationChecker : AlphaChecker
{

    private ExistentialGraph dif_point_prev;
    private ExistentialGraph dif_point_current;

    public DeiterationChecker(ExistentialGraph prev_step, ExistentialGraph current_step)
    {
        this.prev_step = prev_step;
        this.current_step = current_step;

        Tuple<ExistentialGraph, ExistentialGraph> dif_pair = this.Find_Difference_Between(prev_step, current_step);
        this.dif_point_prev = dif_pair.Item1;
        this.dif_point_current = dif_pair.Item2;
    }

    public override bool Could_Be_Inferred()
    {
        foreach(ExistentialGraph g in this.dif_point_prev.Get_Immediate_Subgraphs())
        {
            if (g.Exists_In_Upper_Subgraph())
                return true;
        }
        return false;
    }

    public override bool Can_Be_Inferred()
    {
        List<ExistentialGraph> prev_subgraphs = this.dif_point_prev.Get_Immediate_Subgraphs();
        List<ExistentialGraph> current_subgraphs = this.dif_point_current.Get_Immediate_Subgraphs();

        if (prev_subgraphs.Count == current_subgraphs.Count)
            return false;

        bool deletion_flag = false;

        HashSet<ExistentialGraph> prev = new HashSet<ExistentialGraph>(prev_subgraphs);

        foreach(ExistentialGraph g in prev)
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
