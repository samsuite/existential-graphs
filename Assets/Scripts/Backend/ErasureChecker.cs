using System;
using System.Collections.Generic;

public class ErasureChecker : AlphaChecker
{

    private ExistentialGraph dif_point_prev;
    private ExistentialGraph dif_point_current;

    public ErasureChecker(ExistentialGraph prev_step, ExistentialGraph current_step)
    {
        this.prev_step = prev_step;
        this.current_step = current_step;

        Tuple<ExistentialGraph, ExistentialGraph> dif_pair = this.Find_Difference_Between(prev_step, current_step);
        this.dif_point_prev = dif_pair.Item1;
        this.dif_point_current = dif_pair.Item2;
    }


    public override bool Could_Be_Inferred()
    {
        return this.dif_point_prev.Is_On_Even_Level();
    }

    public override bool Can_Be_Inferred()
    {
        return this.dif_point_prev.Get_Immediate_Subgraphs().Count
            > this.dif_point_current.Get_Immediate_Subgraphs().Count;
    }
}