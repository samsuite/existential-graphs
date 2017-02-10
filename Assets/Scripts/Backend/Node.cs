using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {

    // is this a variable? otherwise it's a cut.
    public bool is_variable;

    // if this is a variable, whats its name?
    [HideInInspector]
    public string value;

    // if this is a cut, there's a vectorline associated with it.
    //[HideInInspector]
    //public VectorLine vecline;

    // a unique ID to identify this node.
    // the way CurryBoy did this was totally ludicrous so I'm doing it a different way!
    // we'll have a static variable that keeps track of the last used ID, and we'll increment it
    // each time we initialize a new node. I guess we might run into problems if someone makes more than
    // 4,294,967,295 nodes, but I'm not too concerned.
    // you can tell this is professional because I decided to use unsigned integers.
    [HideInInspector]
    public uint ID;
    static uint lastID;

    Node root;

    void Awake () {
        // set the last ID to 0 if hasn't been initialized yet.
        // I guess this means the first node will have an ID of 1 -- that seems fine.
        if (lastID == null){
            lastID = 0;
        }

         root = GameObject.FindWithTag("ROOT").GetComponent<Node>();
    }

    // has this node already been initialized as a cut or variable?
    bool is_initialized = false;



    /// INITIALIZATION
    // this should probably also add a component that renders circles or variable names or whatever.

    /*
    // initialize this node as a cut
    public void initAsCut(VectorLine line) {
        if (!is_initialized){
            is_variable = false;
            value = null;
            vecline = line;

            initGeneric();
        }
        else {
            Debug.Log("Uh oh -- tried to initialize the same node more than once.");
        }
    }*/

    // initialize this node as a variable
    public void initAsVar(string val) {
        if (!is_initialized){
            is_variable = true;
            value = val;

            initGeneric();
        }
        else {
            Debug.Log("Uh oh -- tried to initialize the same node more than once.");
        }
    }
    
    // private function for things that both initializers have to do (no reason to duplicate code)
    void initGeneric() {
        lastID++;
        ID = lastID;
        root.recursiveReposition();
        
        is_initialized = true;
    }



    /// HELPFUL FUNCTIONS
    
    public void setNameAndPos (string variable_name, float x, float y) {
        if (is_variable){
            gameObject.name = "var \'" + variable_name + "\' [" + ID + "]";
        }
        else {
            gameObject.name = "cut [" + ID + "]";
        }

        // set the z value based on the node's depth in the hierarchy
        transform.position = new Vector3(x, y, getDepth()*0.1f); // WARNING -- DOES NOT WORK AS INTENDED
    }


    // returns a list of any child nodes
    public List<Node> getChildren () {
        List<Node> children = new List<Node>();

        foreach (Transform child in transform){
            if (child.gameObject.GetComponent<Node>()){
                children.Add(child.gameObject.GetComponent<Node>());
            }
        }

        return children;
    }

    // returns the depth of the node in the tree. 0 is the very top, 1 has a single parent, etc.
    public int getDepth () {
        int depth = 0;

        if (transform.parent != null){
            GameObject par = transform.parent.gameObject;
        
            while (par.GetComponent<Node>() != null){
                depth++;

                if (par.transform.parent != null){
                    par = par.transform.parent.gameObject;
                }
                else {
                    break;
                }
            }
        }
        return depth;
    }

    // is this node on an even level?
    public bool onEvenLevel () {
        int depth = getDepth();
        if (depth%2 == 1){ // root node is a cut, which means the first "layer" should be even, even though it's inside one cut
            return true;
        }
        return false;
    }

    // hey, why not? might be handy
    public bool onOddLevel () {
        return !onEvenLevel();
    }

    // reposition this node and all children so their z-values are correctly based on their depth in the tree
    public void recursiveReposition () {
        transform.position = new Vector3(transform.position.x, transform.position.y, 500-getDepth());

        foreach (Transform child in transform){
            if (child.gameObject.GetComponent<Node>()){
                child.gameObject.GetComponent<Node>().recursiveReposition();
            }
        }
    }
}
