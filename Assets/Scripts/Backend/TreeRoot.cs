using UnityEngine;
using System.Collections.Generic;

public class TreeRoot : MonoBehaviour {

    Node t_node;

    void Start () {
        t_node = transform.GetComponent<Node>();
    }

}
