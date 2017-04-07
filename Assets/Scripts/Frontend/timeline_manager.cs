using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeline_manager : MonoBehaviour {

    public struct time_connection {
        public timeline_node start;
        public timeline_node end;
        public GameObject line;
    }

    public List<time_connection> all_connections = new List<time_connection>();
    public List<List<timeline_node>> all_nodes = new List<List<timeline_node>>();
    public List<RectTransform> all_columns = new List<RectTransform>();

    public GameObject column_prefab;
    public GameObject node_prefab;
    public GameObject connection_prefab;

    public RectTransform timeline;

    void Start () {
        timeline_node node1 = AddFirstNode();
        timeline_node node2 = AddNode(node1);
        timeline_node node3 = AddNode(node1);
        timeline_node node4 = AddNode(node2);
        timeline_node node5 = AddNode(node2);
        timeline_node node6 = AddNode(node2);
        timeline_node node7 = AddNode(node2);
        timeline_node node8 = AddNode(node1);
        timeline_node node9 = AddNode(node8);
        timeline_node node10 = AddNode(node9);
        timeline_node node11 = AddNode(node10);
    }

	void Update () {
		for (int i = 0; i < all_connections.Count; i++) {
            SetLinePosition(all_connections[i]);
        }
	}

    void SetLinePosition (time_connection conn) {
        conn.line.transform.position = (conn.end.transform.position - conn.start.transform.position)/2f + conn.start.transform.position;
        conn.line.transform.localScale = new Vector3(Vector3.Distance(conn.end.transform.position, conn.start.transform.position), 1f, 1f);

        if (conn.end.transform.position.y > conn.start.transform.position.y) {
            conn.line.transform.eulerAngles = new Vector3(0f, 0f, Vector3.Angle(Vector3.right, conn.end.transform.position - conn.start.transform.position));
        }
        else {
            conn.line.transform.eulerAngles = new Vector3(0f, 0f, -1f*Vector3.Angle(Vector3.right, conn.end.transform.position - conn.start.transform.position));
        }
    }

    timeline_node AddFirstNode () {

        timeline_node my_node = Instantiate(node_prefab).GetComponent<timeline_node>();
        RectTransform my_column = Instantiate(column_prefab).GetComponent<RectTransform>();

        my_column.transform.parent = timeline;
        my_node.transform.parent = my_column.transform;

        all_columns.Add(my_column);
        all_nodes.Add(new List<timeline_node>());

        all_nodes[0].Add(my_node);

        return my_node;
    }

    timeline_node AddNode (timeline_node previous) {

        // first, find the previous node in the graph
        int col_ind = -1;
        int row_ind = -1;
        bool found = false;

        for (int i = 0; i < all_nodes.Count; i++) {
            for (int j = 0; j < all_nodes[i].Count; j++) {
                if (all_nodes[i][j] == previous) {
                    col_ind = i;
                    row_ind = j;
                    found = true;

                    break;
                }
            }

            if (found) {
                break;
            }
        }

        if (!found) {
            return null;
            // previous node isn't in graph somehow
        }

        int new_col_ind = col_ind+1;
        RectTransform my_column;
        timeline_node my_node = Instantiate(node_prefab).GetComponent<timeline_node>();

        // do we need to make a new column for this entry?
        if (new_col_ind < all_columns.Count) {
            my_column = all_columns[new_col_ind];
            my_node.transform.parent = my_column.transform;

            all_nodes[new_col_ind].Add(my_node);

        }
        else {
            my_column = Instantiate(column_prefab).GetComponent<RectTransform>();
            my_column.transform.parent = timeline;
            my_node.transform.parent = my_column.transform;

            all_columns.Add(my_column);
            all_nodes.Add(new List<timeline_node>());

            all_nodes[new_col_ind].Add(my_node);
        }



        GameObject new_line = Instantiate(connection_prefab);
        new_line.transform.parent = timeline;

        time_connection new_connection = new time_connection();
        new_connection.start = previous;
        new_connection.end = my_node;
        new_connection.line = new_line;

        all_connections.Add(new_connection);



        return my_node;
    }


}
