using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BSPNode : MonoBehaviour
{
    public Vector2Int bottomLeftAreaCorner { get; set; }
    public Vector2Int bottomRightAreaCorner { get; set; }
    public Vector2Int topLeftAreaCorner { get; set; }
    public Vector2Int topRightAreaCorner { get; set; }

    private List<BSPNode> childrenNodeList;
    public List<BSPNode> ChildrenNodeList { get { return childrenNodeList; } }

    public BSPNode parentNode { get; set; }

    public int treeLayerIndex { get; set; }

    protected BSPNode(BSPNode parent)
    {
        parentNode = parent;
        childrenNodeList = new List<BSPNode>();

        if (parentNode != null)
            parentNode.childrenNodeList.Add(this);
    }
}
