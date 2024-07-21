using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BSPDungeonGenerator : MonoBehaviour
{
    private int dungeonWidth;
    private int dungeonHeight;

    private List<BSPRoomNode> allRoomNodeList;

    public BSPDungeonGenerator(int dungeonWidth, int dungeonHeight)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonHeight = dungeonHeight;

        allRoomNodeList = new List<BSPRoomNode>();
    }

    public List<BSPNode> CalculateDungeon(int maxIteration, int minRoomWidth, int minRoomHeight, float roomBottonCornerModifier, float roomTopCornerModifier, int roomOffset, int corridorWidth)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonHeight);
        allRoomNodeList = bsp.PrepareNodeCollection(maxIteration, minRoomWidth, minRoomHeight);

        List<BSPNode> roomSpaces = BSPStructureHelper.GetLowestLeavesFromGraph(bsp.RootNode);
        List<BSPRoomNode> roomList = BSPRoomGenerator.GenerateRoomInGivenSpace(roomSpaces, roomBottonCornerModifier, roomTopCornerModifier, roomOffset);

        CorridorsGenerator corridorGenerator = new CorridorsGenerator();
        var corridorList = corridorGenerator.CreateCorridor(allRoomNodeList, corridorWidth);

        return new List<BSPNode>(roomList).Concat(corridorList).ToList();
    }
}
