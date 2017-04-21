using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Verifier : MonoBehaviour
{
    public parenting_manager par;
    public Text output_text;

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
            try
            {
                Pair<ExistentialGraph, ExistentialGraph> difp = c.Find_Difference_Between(par.prev_state, par.curr_state);
                if (c.Is_Double_Cut(difp.item1, difp.item2)) {
                    Debug.Log("It's a Double Cut!");
                    output_text.text = "It's a Double Cut!";
                }
                else if (c.Is_Iteration(difp.item1, difp.item2)) {
                    Debug.Log("It's Iteration!");
                    output_text.text = "It's Iteration!";
                }
                else if (c.Is_Deiteration(difp.item1, difp.item2)) {
                    Debug.Log("It's Deiteration!");
                    output_text.text = "It's Deiteration!";
                }
                else if (c.Is_Insertion(difp.item1, difp.item2)) {
                    Debug.Log("It's Insertion!");
                    output_text.text = "It's Insertion!";
                }
                else if (c.Is_Erasure(difp.item1, difp.item2)) {
                    Debug.Log("It's Erasure!");
                    output_text.text = "It's Erasure!";
                }
                else {
                    Debug.Log("No valid rule found!");
                    output_text.text = "No valid rule found!";
                }
            }
            catch (Exception e)
            {
                return;
            }

        }
    }

    public void Done()
    {
        par.ConvertAndPrint();
        print(par.prev_state.Equals(par.curr_state));
    }

}
