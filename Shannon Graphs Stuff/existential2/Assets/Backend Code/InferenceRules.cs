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

        newCut.initAsCut();
        newObj.name = "cut [" + newCut.ID + "]";

        // something to note: the camera is facing the -Z direction, so higher Z values are closer. this
        // is convenient because we can have the Z values of our nodes be directly proportional
        // to their depth in the hierarchy. neat.

        // the camera is at 1000 Z right now, which means we can have 1,000 nested cuts before we reach it.
        // seems like plenty!
        newObj.transform.parent = mom.transform;
        newObj.transform.position = new Vector3(parameters.posX, parameters.posY, newCut.getDepth());

        return newObj;

    }

    // add a variable as the child of node 'mom'
    public static GameObject addVar (Node mom, Params parameters){
        GameObject newObj = new GameObject();
        Node newVar = newObj.AddComponent<Node>();

        newVar.initAsVar(parameters.variable_name);
        newObj.name = "var \'" + parameters.variable_name + "\' [" + newVar.ID + "]";

        newObj.transform.parent = mom.transform;
        newObj.transform.position = new Vector3(parameters.posX, parameters.posY, newVar.getDepth()*0.1f);

        return newObj;

    }
}
