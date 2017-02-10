using UnityEngine;
using System.Collections;

public class InferenceRules : MonoBehaviour {

    // alright, let's set some ground rules for these... rules.
    // each rule has two arguments:
    // -- tree (the head node for the location where the rule will be applied)
    // -- parameters (a Params instance containing any special information the rule might need)
    
    
    // after a rule completes, we save the tree and add it to the "tree of trees."

    // we're not worrying about making sure rules are applicable before we apply them  -- YET.
    // we'll get there soon!



    // add a cut as the child of node 'mom'
    public static GameObject addCut (Node mom, Params parameters){
        GameObject newObj = new GameObject();
        Node newCut = newObj.AddComponent<Node>();

        //newCut.initAsCut(parameters.vectorline);

        newObj.transform.parent = mom.transform;
        newCut.setNameAndPos(parameters.variable_name, parameters.posX, parameters.posY);

        return newObj;

    }

    // add a variable as the child of node 'mom'
    public static GameObject addVar (Node mom, Params parameters){
        GameObject newObj = new GameObject();
        Node newVar = newObj.AddComponent<Node>();

        newVar.initAsVar(parameters.variable_name);

        newObj.transform.parent = mom.transform;
        newVar.setNameAndPos(parameters.variable_name, parameters.posX, parameters.posY);

        return newObj;

    }
}
