using System;
using System.Linq;
using System.Collections.Generic;

public class Verifier
{

    private AlphaChecker dc;
    private AlphaChecker iter;
    private AlphaChecker deiter;
    private AlphaChecker insert;
    private AlphaChecker erasure;

    public Verifier(ExistentialGraph prev_step, ExistentialGraph current_step)
    {
        this.dc = new DoubleCutChecker(prev_step, current_step);
        this.iter = new IterationChecker(prev_step, current_step);
        this.deiter = new DeiterationChecker(prev_step, current_step);
        this.insert = new InsertionChecker(prev_step, current_step);
        this.erasure = new ErasureChecker(prev_step, current_step);
    }

    public List<Rule> verify(ExistentialGraph g1, ExistentialGraph g2)
    {
        List<Rule> inferred_rules = new List<Rule>();
        bool found_rule = false;
        if (this.dc.Could_Be_Inferred() && this.dc.Can_Be_Inferred())
        {
            inferred_rules.Add(Rule.doublecut);
            found_rule = true;
        }

        if (this.iter.Could_Be_Inferred() && this.iter.Can_Be_Inferred())
        {
            inferred_rules.Add(Rule.iteration);
            found_rule = true;
        }

        if (this.deiter.Could_Be_Inferred() && this.deiter.Can_Be_Inferred())
        {
            inferred_rules.Add(Rule.deiteration);
            found_rule = true;
        }

        if (this.insert.Could_Be_Inferred() && this.insert.Can_Be_Inferred())
        {
            inferred_rules.Add(Rule.insertion);
            found_rule = true;
        }

        if (this.erasure.Could_Be_Inferred() && this.erasure.Can_Be_Inferred())
        {
            inferred_rules.Add(Rule.erasure);
            found_rule = true;
        }

        if (!found_rule)
            inferred_rules.Add(Rule.invalid);

        return inferred_rules;
    }

    public bool Done(ExistentialGraph g1, ExistentialGraph g2)
    {
        return g1.Equals(g2);
    }

}
