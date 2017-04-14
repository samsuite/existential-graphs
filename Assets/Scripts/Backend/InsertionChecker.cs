using System;
using System.Linq;
using System.Collections.Generic;

public class InsertionChecker : AlphaChecker
{
    private ExistentialGraph dif_point_prev;
    private ExistentialGraph dif_point_current;

    public InsertionChecker(ExistentialGraph prev_step, ExistentialGraph current_step)
    {
        this.prev_step = prev_step;
        this.current_step = current_step;

        Pair<ExistentialGraph, ExistentialGraph> dif_pair = this.Find_Difference_Between(prev_step, current_step);
        this.dif_point_prev = dif_pair.item1;
        this.dif_point_current = dif_pair.item2;
    }

    public override bool Could_Be_Inferred()
    {
        return this.dif_point_prev.Is_On_Odd_Level();
    }

    public override bool Can_Be_Inferred()
    {
        return this.dif_point_prev.Get_Immediate_Subgraphs().Count
            < this.dif_point_current.Get_Immediate_Subgraphs().Count;
    }
}