using System;
using System.Linq;
using System.Collections.Generic;

public abstract class AlphaChecker : IInferable
{

    protected ExistentialGraph prev_step;
    protected ExistentialGraph current_step;

    public abstract bool Could_Be_Inferred();
    public abstract bool Can_Be_Inferred();

    private bool A_DC_Exists(ExistentialGraph g1, ExistentialGraph g2)
    {
        return g1.Label() == "((" + g2.Label() + "))" || g2.Label() == "((" + g1.Label() + "))";
    }

    private bool A_Difference_Exists(ExistentialGraph g1, ExistentialGraph g2)
    {
        return g1.Get_Immediate_Subgraphs().Count != g2.Get_Immediate_Subgraphs().Count;
    }

    protected Tuple<ExistentialGraph, ExistentialGraph> Find_Difference_Between(ExistentialGraph previous_step, ExistentialGraph current_step)
    {

        if (A_DC_Exists(previous_step, current_step))
            return Tuple.Create(previous_step.Get_Parent(), current_step.Get_Parent());
        if (A_Difference_Exists(previous_step, current_step))
            return Tuple.Create(previous_step, current_step);

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

}
