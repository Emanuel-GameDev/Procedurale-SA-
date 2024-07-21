using System;
using System.Collections.Generic;
using System.Linq;

public class CorridorsGenerator
{
    public CorridorsGenerator()
    {
    }

    public List<BSPNode> CreateCorridor(List<BSPRoomNode> allNodesCollection, int corridorWidth)
    {
        List<BSPNode> corridorList = new List<BSPNode>();
        Queue<BSPRoomNode> structuresToCheck = new Queue<BSPRoomNode>(
            allNodesCollection.OrderByDescending(node => node.treeLayerIndex).ToList());

        while(structuresToCheck.Count > 0)
        {
            var node = structuresToCheck.Dequeue();
            if (node.ChildrenNodeList.Count == 0)
                continue; // non possiamo fre un corridoio tra il nulla

            CorridorNode corridor = new CorridorNode(node.ChildrenNodeList[0], node.ChildrenNodeList[1], corridorWidth);
            corridorList.Add(corridor);
        }

        return corridorList;
    }
}