using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPRoomNode : BSPNode
{
    public int Width { get => topRightAreaCorner.x - bottomLeftAreaCorner.x; }
    public int Lenght { get => topRightAreaCorner.y - bottomLeftAreaCorner.y; }

    public BSPRoomNode(Vector2Int _bottomLeftAreaCorner, Vector2Int _topRightAreaCorner, BSPNode parent, int layerIndex) : base(parent)
    {
        bottomLeftAreaCorner = _bottomLeftAreaCorner;
        topRightAreaCorner = _topRightAreaCorner;

        bottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
        topLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, topRightAreaCorner.y);

        treeLayerIndex = layerIndex;
    }
}
