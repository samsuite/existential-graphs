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

		if (c.A_DC_Exists(difp.item1, difp.item2)) {
                    Debug.Log("It's a Double Cut!");
                    output_text.text = "Double Cut";
                }
                else if (c.Is_Iteration(difp.item1, difp.item2)) {
                    Debug.Log("It's Iteration!");
                    output_text.text = "Iteration";
                }
                else if (c.Is_Deiteration(difp.item1, difp.item2)) {
                    Debug.Log("It's Deiteration!");
                    output_text.text = "Deiteration";
                }
                else if (c.Is_Insertion(difp.item1, difp.item2)) {
                    Debug.Log("It's Insertion!");
                    output_text.text = "Insertion";
                }
                else if (c.Is_Erasure(difp.item1, difp.item2)) {
                    Debug.Log("It's Erasure!");
                    output_text.text = "Erasure";
                }
                else {
                    Debug.Log("No valid rule found!");
                    output_text.text = "No valid rule";
                }
            }
            catch (Exception e)
            {
		Debug.Log("EXCEPTION?" + e.ToString());
                return;
            }

        }
        else
        {
            Debug.Log("It's Identity!");
            output_text.text = "Identity";
        }
    }

    public void Done()
    {
        //par.ConvertAndPrint();
	//We need a way of accessing the goal state
    }

}
