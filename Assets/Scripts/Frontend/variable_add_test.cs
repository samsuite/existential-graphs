using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class variable_add_test : MonoBehaviour {

    public void add_variable () {

        Node parent = GameObject.FindWithTag("ROOT").GetComponent<Node>();

        Params parameters = new Params();
        parameters.posX = 0f;
        parameters.posY = 0f;
        parameters.variable_name = "n";

        InferenceRules.addVar(parent, parameters);
    }
}
