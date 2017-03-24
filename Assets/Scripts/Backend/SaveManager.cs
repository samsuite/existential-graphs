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

    bool saved_scene = false;
    bool loaded_scene = false;
    public bool save_scene = false;
    public bool load_scene = false;

    void Start () {
        string file_name = "test tree";
        state = new SaveState();
    }





    /// just for testing ///
    ////////////////////////

    void Update () {

        if (!saved_scene && save_scene) {
            saveSceneToState();
        }
        if (!loaded_scene && load_scene) {
            loadSceneFromState();
        }

        saved_scene = save_scene;
        loaded_scene = load_scene;
    }

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
