using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveNode : MonoBehaviour {

    public SaveManager.node_type type;
    public Vector2 position;
    public string name;
    public List<SaveNode> children;
    public float radius;
    public Vector3 offset;
}
