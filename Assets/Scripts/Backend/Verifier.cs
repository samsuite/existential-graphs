using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Verifier : MonoBehaviour
{
    public parenting_manager par;

    void Start()
    {
        par.ConvertAndPrint();
    }

    public void verify()
    {
        Debug.Log("VERIFYING!!!");

        par.ConvertAndPrint();
        AlphaChecker c = new AlphaChecker();
        if (!par.prev_state.Equals(par.curr_state))
        {
            Pair<ExistentialGraph, ExistentialGraph> difp = c.Find_Difference_Between(par.prev_state, par.curr_state);
            if (c.Is_Double_Cut(difp.item1, difp.item2))
                Debug.Log("It's a Double Cut!");
            else if (c.Is_Iteration(difp.item1, difp.item2))
                Debug.Log("It's Iteration!");
            else if (c.Is_Deiteration(difp.item1, difp.item2))
                Debug.Log("It's Deiteration!");
            else if (c.Is_Insertion(difp.item1, difp.item2))
                Debug.Log("It's Insertion!");
            else if (c.Is_Erasure(difp.item1, difp.item2))
                Debug.Log("It's Erasure!");
        }

    }

    public void Done()
    {
        par.ConvertAndPrint();
        print(par.prev_state.Equals(par.curr_state));
    }

}
