using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BSPStructureHelper
{
    internal static List<BSPNode> GetLowestLeavesFromGraph(BSPNode rootNode)
    {
        if (rootNode.ChildrenNodeList.Count == 0)
            return new List<BSPNode> { rootNode };

        List<BSPNode> resultList = new List<BSPNode>();
        Queue<BSPNode> nodesToCheck = new Queue<BSPNode>();

        foreach (BSPNode childNode in rootNode.ChildrenNodeList)
        {
            nodesToCheck.Enqueue(childNode);
        }

        while (nodesToCheck.Count > 0)
        {
            var currentNode = nodesToCheck.Dequeue();

            if (currentNode.ChildrenNodeList.Count == 0)
                resultList.Add(currentNode);
            else
            {
                foreach (BSPNode childNode in currentNode.ChildrenNodeList)
                {
                    nodesToCheck.Enqueue(childNode);
                }
            }
        }

        return null;
    }

    internal static Vector2Int GenerateBottomLeftCorner(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, float roomBottomCornerMultiplier, int offset)
    {
        int minX = bottomLeftAreaCorner.x + offset;
        int maxX = topRightAreaCorner.x - offset;
        int minY = bottomLeftAreaCorner.y + offset;
        int maxY = topRightAreaCorner.y - offset;   

        return new Vector2Int(UnityEngine.Random.Range(minX, (int)(minX + (maxX - minX) * roomBottomCornerMultiplier)),
                              UnityEngine.Random.Range(minY, (int)(minY + (maxY - minY) * roomBottomCornerMultiplier)));
        
    }

    internal static Vector2Int CalculateMiddlePoint(Vector2Int v1, Vector2Int v2)
    {
        Vector2 sum = v1 + v2;
        Vector2 tempVector = sum / 2;
        return new Vector2Int((int)tempVector.x, (int)tempVector.y);
    }
}
