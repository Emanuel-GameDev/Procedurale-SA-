using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPRoomGenerator
{
    public static List<BSPRoomNode> GenerateRoomInGivenSpace(List<BSPNode> roomSpace, float roomBottomCornerMultiplier, float roomTopCornerMultiplier, int roomOffset)
    {
        List<BSPRoomNode> resultList = new List<BSPRoomNode>();

        foreach (BSPNode space in roomSpace)
        {
            Vector2Int newBottomLeftPoint = BSPStructureHelper.GenerateBottomLeftCorner(space.bottomLeftAreaCorner, space.topRightAreaCorner, roomBottomCornerMultiplier, roomOffset);
        

                }

        return resultList;
    }
}
