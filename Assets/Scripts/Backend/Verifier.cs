using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Verifier : MonoBehaviour
{
    public parenting_manager par;

    void Start () {
        par.ConvertAndPrint();
    }

    /*
    public Verifier(ExistentialGraph prev_step, ExistentialGraph current_step)
    {
        dc = new DoubleCutChecker(prev_step, current_step);
        iter = new IterationChecker(prev_step, current_step);
        deiter = new DeiterationChecker(prev_step, current_step);
        insert = new InsertionChecker(prev_step, current_step);
        erasure = new ErasureChecker(prev_step, current_step);
    }

     */

 
    public void verify()
    {
        Debug.Log("VERIFYING!!!");

        par.ConvertAndPrint();
        AlphaChecker c = new AlphaChecker();
        Pair<ExistentialGraph, ExistentialGraph> difp = c.Find_Difference_Between(par.prev_state, par.curr_state);

        if (c.Is_Double_Cut(difp.item1, difp.item2))
            print("It's a Double Cut!");

        //List<Rule> inferred_rules = new List<Rule>();
        //bool found_rule = false;
        //if (dc.Could_Be_Inferred() && dc.Can_Be_Inferred())
        //{
        //    inferred_rules.Add(Rule.doublecut);
        //    found_rule = true;
        //}

        //if (iter.Could_Be_Inferred() && iter.Can_Be_Inferred())
        //{
        //    inferred_rules.Add(Rule.iteration);
        //    found_rule = true;
        //}

        //if (deiter.Could_Be_Inferred() && deiter.Can_Be_Inferred())
        //{
        //    inferred_rules.Add(Rule.deiteration);
        //    found_rule = true;
        //}

        //if (insert.Could_Be_Inferred() && insert.Can_Be_Inferred())
        //{
        //    inferred_rules.Add(Rule.insertion);
        //    found_rule = true;
        //}

        //if (erasure.Could_Be_Inferred() && erasure.Can_Be_Inferred())
        //{
        //    inferred_rules.Add(Rule.erasure);
        //    found_rule = true;
        //}

        //if (!found_rule)
        //    inferred_rules.Add(Rule.invalid);

        //return inferred_rules;

        //print (inferred_rules);
    }

    public void Done()
    {
        par.ConvertAndPrint();
        print(par.prev_state.Equals(par.curr_state));
    }

}
