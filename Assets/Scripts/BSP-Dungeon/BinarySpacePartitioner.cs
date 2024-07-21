using System.Collections.Generic;
using UnityEngine;

public class BinarySpacePartitioner : MonoBehaviour
{
    BSPRoomNode rootNode;

    public BSPRoomNode RootNode => rootNode;

    public BinarySpacePartitioner(int dungeonWidth, int dungeonHeight)
    {
        rootNode = new BSPRoomNode(Vector2Int.zero, new Vector2Int(dungeonWidth, dungeonHeight), null, 0);
    }

    public List<BSPRoomNode> PrepareNodeCollection(int maxIteration, int minRoomWidth, int minRoomHeight)
    {
        Queue<BSPRoomNode> graph = new Queue<BSPRoomNode>();
        List<BSPRoomNode> resultList = new List<BSPRoomNode>();

        graph.Enqueue(rootNode);
        resultList.Add(rootNode);

        int iterations = 0;

        while (iterations < maxIteration && graph.Count > 0)
        {
            iterations++;

            var currentNode = graph.Dequeue();
            if (currentNode.Width >= (minRoomWidth * 2) || currentNode.Lenght >= (minRoomHeight * 2))
            {
                //siamo dentro lo spazio che ci serve da dividere
                SplitTheSpace(currentNode, resultList, minRoomWidth, minRoomHeight, graph);
            }
        }

        return resultList;
    }

    private void SplitTheSpace(BSPRoomNode currentNode, List<BSPRoomNode> resultList, int minRoomWidth, int minRoomHeight, Queue<BSPRoomNode> graph)
    {
        BSPLine dividingLine = GetDividingLineSpace(currentNode.bottomLeftAreaCorner, currentNode.topRightAreaCorner, minRoomWidth, minRoomHeight);

        BSPRoomNode node1 = null;
        BSPRoomNode node2 = null;

        switch (dividingLine.Orientation)
        {
            case BSPOrientation.horinzontal:
                node1 = new BSPRoomNode(currentNode.bottomLeftAreaCorner, new Vector2Int(currentNode.topRightAreaCorner.x, dividingLine.Coordinates.y),
                                        currentNode,
                                        currentNode.treeLayerIndex + 1);
                node2 = new BSPRoomNode(new Vector2Int(currentNode.bottomLeftAreaCorner.x, dividingLine.Coordinates.y), currentNode.topRightAreaCorner,
                                        currentNode,
                                        currentNode.treeLayerIndex + 1);

                break;
            case BSPOrientation.vertical:
                node1 = new BSPRoomNode(currentNode.bottomLeftAreaCorner, new Vector2Int(dividingLine.Coordinates.x, currentNode.topRightAreaCorner.y),
                                        currentNode,
                                        currentNode.treeLayerIndex + 1);
                node2 = new BSPRoomNode(new Vector2Int(dividingLine.Coordinates.x, currentNode.bottomLeftAreaCorner.y), currentNode.topRightAreaCorner,
                                        currentNode,
                                        currentNode.treeLayerIndex + 1);
                break;
            default:
                break;
        }

        AddNodesToCollections(resultList, graph, node1);
        AddNodesToCollections(resultList, graph, node2);
    }

    private void AddNodesToCollections(List<BSPRoomNode> resultList, Queue<BSPRoomNode> graph, BSPRoomNode node)
    {
        resultList.Add(node);
        graph.Enqueue(node);
    }

    private BSPLine GetDividingLineSpace(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int minRoomWidth, int minRoomHeight)
    {
        BSPOrientation orientation;
        bool heightStatus = (topRightAreaCorner.y - bottomLeftAreaCorner.y) >= minRoomHeight * 2;
        bool widthStatus = (topRightAreaCorner.x - bottomLeftAreaCorner.x) >= minRoomWidth * 2;

        if (heightStatus && widthStatus)
        {
            orientation = (BSPOrientation)UnityEngine.Random.Range(0, 2);
        }
        else if (heightStatus)
        {
            orientation = BSPOrientation.horinzontal;
        }
        else
        {
            orientation = BSPOrientation.vertical;
        }

        return new BSPLine(orientation, GetCoordinatesForOrientation(orientation, bottomLeftAreaCorner, topRightAreaCorner, minRoomWidth, minRoomHeight));

    }

    private Vector2Int GetCoordinatesForOrientation(BSPOrientation orientation, Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int minRoomWidth, int minRoomHeight)
    {
        Vector2Int coordinates = Vector2Int.zero;

        switch (orientation)
        {
            case BSPOrientation.horinzontal:
                coordinates = new Vector2Int(0, UnityEngine.Random.Range(bottomLeftAreaCorner.y + minRoomHeight, topRightAreaCorner.y - minRoomHeight));
            break;

            case BSPOrientation.vertical:
                coordinates = new Vector2Int(UnityEngine.Random.Range(bottomLeftAreaCorner.x + minRoomWidth, topRightAreaCorner.x - minRoomWidth), 0);
                break;
        }

        return coordinates;
    }
}
