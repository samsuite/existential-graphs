using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveState : System.Object {

	public struct save_var {
        public Vector3 position;
        public string name;
    }

    public struct save_cut {
        public Vector3 position;
        public float radius;
    }

    public List<save_var> all_vars = new List<save_var>();
    public List<save_cut> all_cuts = new List<save_cut>();



    // create lists of variables and cuts based on the current scene state
    public void CreateStateFromScene()
    {
        all_vars.Clear();
        all_cuts.Clear();

        for( int i = 0; i < node_manager.all_vars.Count; i++ )
        {
            save_var temp_var = new save_var();
            temp_var.position = node_manager.all_vars[i].transform.position;
            temp_var.name = node_manager.all_vars[i].var_name.text;

            all_vars.Add(temp_var);
        }

        for( int i = 0; i < node_manager.all_cuts.Count; i++ )
        {
            save_cut temp_cut = new save_cut();
            temp_cut.position = node_manager.all_cuts[i].transform.position;
            temp_cut.radius = node_manager.all_cuts[i].radius;

            all_cuts.Add(temp_cut);
        }
    }

    // recreate the saved state
    public void LoadStateToScene()
    {
        // first, delete everything
        node_manager.EraseAll();

        for( int i = 0; i < all_vars.Count; i++ ){
            node_manager.AddVariable(all_vars[i].position, all_vars[i].name);
        }

        for( int i = 0; i < all_cuts.Count; i++ ){
            node_manager.AddCircle(all_cuts[i].position, all_cuts[i].radius);
        }
    }

}
