using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour {

    public enum node_type {
        Variable,
        Cut,
        Root
    };

    SaveState state;

    void Start () {
        string file_name = "test tree";
        state = new SaveState();
    }





    /// just for testing ///
    ////////////////////////

    public void saveSceneToState (){
        print ("saving");
        state.CreateStateFromScene();
    }

    public void loadSceneFromState (){
        print ("loading");
        state.LoadStateToScene();
    }

    ////////////////////////
}
