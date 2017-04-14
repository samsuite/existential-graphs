using System;
using System.Linq;
using System.Collections.Generic;

public class DoubleCutChecker : AlphaChecker
{
    private ExistentialGraph dif_point_prev;
    private ExistentialGraph dif_point_current;

    public DoubleCutChecker(ExistentialGraph prev_step, ExistentialGraph current_step)
    {
        this.prev_step = prev_step;
        this.current_step = current_step;

        Tuple<ExistentialGraph, ExistentialGraph> dif_pair = this.Find_Difference_Between(prev_step, current_step);
        this.dif_point_prev = dif_pair.item1;
        this.dif_point_current = dif_pair.item2;
    }

    public override bool Could_Be_Inferred()
    { 
        return true;
    }

    public override bool Can_Be_Inferred()
    {
        return this.dif_point_prev.Label() == "((" + this.dif_point_current.Label() + "))"
            || this.dif_point_current.Label() == "((" + this.dif_point_prev.Label() + "))";
    }

}
