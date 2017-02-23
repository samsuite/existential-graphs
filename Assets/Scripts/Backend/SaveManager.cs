using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour {

    public enum node_type {
        Variable,
        Cut,
        Root
    };


    Node tree_root;
    SaveNode root_save_node;
    //circle circle_man;

    void Start () {
        string file_name = "test tree";
        tree_root = GameObject.FindGameObjectWithTag("ROOT").GetComponent<Node>();
        //circle_man = GameObject.FindGameObjectWithTag("CircleManager").GetComponent<circle>();
    }


    /// just for testing ///

    public void saveHierarchyToStructureTest (){
        root_save_node = saveHierarchyToStructure(tree_root);
    }

    public void loadStructureToHierarchyTest (){
        clearHierarchy(tree_root);
        for (int i = 0; i < root_save_node.children.Count; i++){
            loadStructureToHierarchy(root_save_node.children[i], tree_root);
        }
    }

    ////////////////////////



    // annhilates every child of a specified root node. use carefully!
    void clearHierarchy (Node root_node) {

        List<Node> children = root_node.getChildren();

        for (int i = 0; i < children.Count; i++){
            Destroy(children[i].gameObject);
        }
    }



    // recursively save the actual unity hierarchy to a tree of SaveNodes
	SaveNode saveHierarchyToStructure (Node root_node) {

        SaveNode new_save_node = new SaveNode();

        // where's the node?
        new_save_node.position = new Vector2(root_node.transform.position.x, root_node.transform.position.y);

        // what's its name?
        new_save_node.name = root_node.value;

        // what kind of node is it?

        if (root_node.gameObject.tag == "ROOT"){
            new_save_node.type = node_type.Root;
        }
        else if (root_node.is_variable){
            new_save_node.type = node_type.Variable;
        }
        else {
            new_save_node.type = node_type.Cut;
            if (root_node.gameObject.GetComponent<CircleCollider2D>()){
                new_save_node.radius = root_node.gameObject.GetComponent<CircleCollider2D>().radius;
                new_save_node.offset = root_node.gameObject.GetComponent<CircleCollider2D>().offset;
            }
        }

        // what are its children?
        List<Node> node_children = root_node.getChildren();
        new_save_node.children = new List<SaveNode>();

        for (int i = 0; i < node_children.Count; i++){
            new_save_node.children.Add(saveHierarchyToStructure(node_children[i]));
        }

        return new_save_node;
    }



    // save a tree of SaveNodes to a file
    void saveStructureToFile (SaveNode new_save_node, string file_name) {

        string out_text = composeStructureString(new_save_node, 0);
        System.IO.File.WriteAllText(System.Environment.CurrentDirectory + @"/Tree Saves/" + file_name + ".tree", out_text);
    }



    // load a file as a tree of SaveNodes
    SaveNode loadFileToStructure (string file_name){

        string path = System.Environment.CurrentDirectory + @"/Tree Saves/" + file_name + ".tree";
        string line;
        int counter = 0;

        // read inthe file as a list of lines
        List<string> lines = new List<string>();

        System.IO.StreamReader file = new System.IO.StreamReader(path);
        while ( (line = file.ReadLine()) != null ) {
            lines.Add(line);
            counter++;
        }
        file.Close();

        // build a tree of SaveNodes from the lines
        SaveNode new_save_node = buildStructureFromLines(lines, 0);
        return new_save_node;
    }

    

    // load a tree of SaveNodes into the actual unity hierarchy
    void loadStructureToHierarchy (SaveNode save_root, Node node_root) {
        
        Node n;
        
        if (save_root.type == node_type.Cut) {
            //n = circle_man.add_circle_of_size(node_root, save_root.position, save_root.radius, save_root.offset).GetComponent<Node>();
            //n.setNameAndPos(save_root.name, save_root.position.x, save_root.position.y);

            //VectorLine current_line = node_root.vecline;



            //print("num points: "+current_line.points3.Count);

            //current_line.MakeCircle (node_root.transform.position, save_root.radius);
			//current_line.Draw();
        }
        else {
            Params p = new Params();
            p.variable_name = save_root.name;
            p.posX = save_root.position.x;
            p.posY = save_root.position.y;

            n = InferenceRules.addVar(node_root, p).GetComponent<Node>();
        }
        

        for (int i = 0; i < save_root.children.Count; i++){
            //loadStructureToHierarchy(save_root.children[i], n);
        }

    }



    // recursively creates a string representing the state of a tree of SaveNodes
    string composeStructureString ( SaveNode n, int level ) {

        // add dashes to represent node depth
        string output = "";
        for(int i=0; i<level; i++){
            output += "-";
        }

        // write in the actual node info
        if (n.type == node_type.Variable){
            output += ("var: "+n.name+" x: "+n.position.x+" y: "+n.position.y+"\n");
        }
        else {
            output += ("cut: "+n.name+" x: "+n.position.x+" y: "+n.position.y+"\n");
        }
        
        // recurse over children
        for(int i=0; i < n.children.Count; i++){
            output += composeStructureString(n.children[i], level+1);
        }

        return output;
    }



    // build a tree of structs from a list of strings
    SaveNode buildStructureFromLines (List<string> file_lines, int line_no) {

        SaveNode new_save_node = makeSaveNodeFromLine(file_lines[line_no]);
        new_save_node.children = new List<SaveNode>();

        int mydepth = calcDepth(file_lines[line_no]);
        line_no ++;

        // while the current line is deeper than the parent line
        while (calcDepth(file_lines[line_no]) > mydepth) {
            // if this is a direct child
            if (calcDepth(file_lines[line_no]) == mydepth + 1){

                //recurse and add it as a child
                new_save_node.children.Add(buildStructureFromLines(file_lines, line_no));
            }

            // move to the next line
            line_no ++;

            // if the line is empty, we're at the end of the file
            if (file_lines[line_no] == ""){
                break;
            }
        }

        return new_save_node;
    }



    // make a SaveNode class from a single string
    SaveNode makeSaveNodeFromLine (string line) {
        // remove tabs in front of the line

        char[] dashDelimiter = { '-' };
        string [] dash_split = line.Split( dashDelimiter );
        int non_dash_index = -1;
        for(int i=0; i<dashDelimiter.Length; i++) {
            if(dash_split[i] != ""){
                non_dash_index = i;
                break;
            }
        }

        string line_information = dash_split[non_dash_index];
        
        char[] delimiterChars = { ' ' };
        string[] pieces = line_information.Split( delimiterChars );

        SaveNode new_save_node = new SaveNode();
        
        new_save_node.name= pieces[1];
        new_save_node.position = new Vector2(float.Parse(pieces[3]), float.Parse(pieces[5]));

        if (pieces[0] == "cut:") {
            new_save_node.type = node_type.Cut;
        }
        else {
            new_save_node.type = node_type.Variable;
        }

        return new_save_node;
    }



    // count how many consecutive dashes are at the start of a string (this represents the depth of an element)
    int calcDepth (string ln) {
        int num_dashes = 0;
        while (ln[num_dashes] == '-') {
            num_dashes++;
        }
        return num_dashes;
    }
}
