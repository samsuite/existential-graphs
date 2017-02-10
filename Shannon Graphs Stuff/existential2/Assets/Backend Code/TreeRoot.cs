using UnityEngine;
using System.Collections.Generic;

public class TreeRoot : MonoBehaviour {

    Node t_node;

    void Start () {
        t_node = transform.GetComponent<Node>();
        //t_node.initAsCut();

        string file_name = "test tree";


        //saveTree( file_name );

        //print("donezo runzo");


        loadTree( file_name );

    }

    
	void saveTree ( string file_name ) {
        string out_text = "";
        out_text += addChildrenToSave( t_node, 0);


        System.IO.File.WriteAllText(System.Environment.CurrentDirectory + @"/Tree Saves/" + file_name + ".tree", out_text);
    }

    string addChildrenToSave ( Node n, int level ) {
        string output = "";
        for(int i=0; i < n.getDepth(); i++){
            output += "\t";
        }


        output += System.String.Format("var: {0} x: {1} y: {2}\n", n.value, n.transform.position.x, n.transform.position.y);
        
        List<Node> childs= n.getChildren();
        for(int i=0; i < childs.Count; i++){
            output += addChildrenToSave(childs[i], level+1);
        }

        return output;
    }


    void loadTree ( string file_name ) {
        string path = System.Environment.CurrentDirectory + @"/Tree Saves/" + file_name + ".tree";
        string line;
        int counter = 0;

        List<string> lines = new List<string>();

        System.IO.StreamReader file = new System.IO.StreamReader(path);
        while ( (line = file.ReadLine()) != null ) {
            lines.Add(line);
            counter++;
        }
        file.Close();


        constructTree(lines, 0, this.GetComponent<Node>());
    }

    /*void constructTree ( List<string> file_lines, ref int line_no, Node prev_parent) {
        print("constructTree call");
        int depth = calcDepth(file_lines[line_no]);
        print(depth.ToString() + " : " + (prev_parent.getDepth() + 1).ToString());
        if( depth < prev_parent.getDepth() + 1 ){
            return;
        }
        bool is_next = true;
        while ( calcDepth(file_lines[line_no] < prev_parent.getDepth() + 1 ) {

            Node new_n = makeNodeFromLine(file_lines[line_no], prev_parent);
            if ( !new_n.is_variable ) {
                line_no++;
                constructTree(file_lines, ref line_no, new_n);
            }

            if ( line_no < file_lines.Count - 1 ) {
                is_next = depth <= calcDepth(file_lines[line_no+1]);
                if(is_next)
                    line_no++;
            }
            else
                is_next = false;
            
        }

        print("End of call");

    }*/

    void constructTree ( List<string> file_lines, int line_no, Node prev_parent) {

        // for every direct child:
            // create a node
            // if it's a cut, call this function on it

        int mydepth = calcDepth(file_lines[line_no]);
        line_no ++;


        // while the current line is deeper than the parent line
        while (calcDepth(file_lines[line_no]) > mydepth) {
            // if this is a direct child
            if (calcDepth(file_lines[line_no]) == mydepth + 1){
                Node new_n = makeNodeFromLine(file_lines[line_no], prev_parent);

                // if it's a cut, recurse!
                if ( !new_n.is_variable ) {
                    constructTree(file_lines, line_no, new_n);
                }
            }

            // move to the next line
            line_no ++;

            // if the line is empty, we're at the end of the file
            //print(file_lines[line_no]);
            if (file_lines[line_no] == ""){
                return;
            }
        }
    }

    Node makeNodeFromLine (string line, Node parent) {
        // remove tabs in front of the line

        char[] tabDelimiter = { '\t' };
        string [] tab_split = line.Split( tabDelimiter );
        int non_tab_index = -1;
        for(int i=0; i<tab_split.Length; i++) {
            if(tab_split[i] != ""){
                non_tab_index = i;
                break;
            }
        }

        string line_information = tab_split[non_tab_index];
        

        char[] delimiterChars = { ' ' };
        string[] pieces = line_information.Split( delimiterChars );


        Params p = new Params();
        p.variable_name = pieces[1];
        p.posX = float.Parse( pieces[3] );
        p.posY = float.Parse( pieces[5] );

        Node n;
        if (pieces[0] == "cut:") {
            n = InferenceRules.addCut(parent, p).GetComponent<Node>();
        }
        else {
            n = InferenceRules.addVar(parent, p).GetComponent<Node>();
        }

        return n;
    }

    int calcDepth (string ln) {
        int num_tabs = 0;
        while (ln[num_tabs] == '\t') {
            num_tabs++;
        }
        return num_tabs;
    }

}
