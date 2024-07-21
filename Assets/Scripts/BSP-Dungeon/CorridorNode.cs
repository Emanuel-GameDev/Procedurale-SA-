using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RelativePosition
{
    Up, Down, Right, Left
}

internal class CorridorNode : BSPNode
{
    private BSPNode structure1;
    private BSPNode structure2;
    private int corridorWidth;
    private int modifierDistanceFromWall = 1;

    public CorridorNode(BSPNode node1, BSPNode node2, int corridorWidth) : base(null) // questo fa si che i corridoi non si collegano tra loro
    {
        this.structure1 = node1;
        this.structure2 = node2;
        this.corridorWidth = corridorWidth;

        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        var relativePositionOfStructure2 = CheckPositionStructure2AgainstStructure1();
        switch (relativePositionOfStructure2)
        {
            case RelativePosition.Up:
                ProcessRoomInRelationUpOrDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Down:
                ProcessRoomInRelationUpOrDown(this.structure2, this.structure1);
                break;
            case RelativePosition.Right:
                ProcessRoomInRelationRightOrLeft(this.structure1, this.structure2);
                break;
            case RelativePosition.Left:
                ProcessRoomInRelationRightOrLeft(this.structure2, this.structure1);
                break;
            default:
                break;
        }
    }


    private RelativePosition CheckPositionStructure2AgainstStructure1()
    {
        // i calcoli che seguono servono a definire dove si trovano le due strutture in esame.
        // Usando delle funzioni di angoli riusciamo a capire se sta sopra, sotto a dx o a sx

        Vector2 middlePointStructure1Temp = ((Vector2)structure1.topRightAreaCorner + structure1.bottomLeftAreaCorner) / 2;
        Vector2 middlePointStructure2Temp = ((Vector2)structure2.topRightAreaCorner + structure2.bottomLeftAreaCorner) / 2;

        float angle = CalculateAngle(middlePointStructure1Temp, middlePointStructure2Temp);
        if ((angle < 45 && angle >= 0) || (angle > -45 && angle < 0))
        {
            return RelativePosition.Right;
        }
        else if (angle > 45 && angle < 135)
        {
            return RelativePosition.Up;
        }
        else if (angle > -135 && angle < -45)
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }
    }

    private float CalculateAngle(Vector2 middlePointStructure1Temp, Vector2 middlePointStructure2Temp)
    {
        return Mathf.Atan2(
            middlePointStructure2Temp.y - middlePointStructure1Temp.y,
            middlePointStructure2Temp.x - middlePointStructure1Temp.x) * Mathf.Rad2Deg;
    }

    #region QUINTO BLOCCO
    private void ProcessRoomInRelationRightOrLeft(BSPNode structure1, BSPNode structure2)
    {
        BSPNode leftStructure = null;
        List<BSPNode> leftStructureChildren = BSPStructureHelper.GetLowestLeavesFromGraph(structure1);
        BSPNode rightStructure = null;
        List<BSPNode> rightStructureChildren = BSPStructureHelper.GetLowestLeavesFromGraph(structure2);

        // abbiamo ordinato le strutture in modo che le prime siano quelle più a destra
        // volendo connettere quelle di sx con quelle di dx dobbiamo assicurarci che non ci siano altre strutture in mezzo,
        // quindi ordinando le strutture per quelle che stanno più a dx e quelle che stanno più a sx possiamo quasi garantire che non
        // ci saranno altre strutture in mezzo
        var sortedLeftStructure = leftStructureChildren.OrderByDescending(child => child.topRightAreaCorner.x).ToList();
        if (sortedLeftStructure.Count == 1)
        {
            leftStructure = sortedLeftStructure[0];
        }
        else
        {
            int maxX = sortedLeftStructure[0].topRightAreaCorner.x;
            sortedLeftStructure = sortedLeftStructure.Where(children => Math.Abs(maxX - children.topRightAreaCorner.x) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedLeftStructure.Count);
            leftStructure = sortedLeftStructure[index];
        }

        var possibleNeighboursInRightStructureList = rightStructureChildren.Where(
            child => GetValidYForNeighourLeftRight(
                leftStructure.topRightAreaCorner,
                leftStructure.bottomRightAreaCorner,
                child.topLeftAreaCorner,
                child.bottomLeftAreaCorner
                ) != -1
            ).OrderBy(child => child.bottomRightAreaCorner.x).ToList();

        if (possibleNeighboursInRightStructureList.Count <= 0)
        {
            rightStructure = structure2;
        }
        else
        {
            rightStructure = possibleNeighboursInRightStructureList[0];
        }
        int y = GetValidYForNeighourLeftRight(leftStructure.topLeftAreaCorner, leftStructure.bottomRightAreaCorner,
            rightStructure.topLeftAreaCorner,
            rightStructure.bottomLeftAreaCorner);
        while (y == -1 && sortedLeftStructure.Count > 1)
        {
            sortedLeftStructure = sortedLeftStructure.Where(
                child => child.topLeftAreaCorner.y != leftStructure.topLeftAreaCorner.y).ToList();
            leftStructure = sortedLeftStructure[0];
            y = GetValidYForNeighourLeftRight(leftStructure.topLeftAreaCorner, leftStructure.bottomRightAreaCorner,
            rightStructure.topLeftAreaCorner,
            rightStructure.bottomLeftAreaCorner);
        }
        bottomLeftAreaCorner = new Vector2Int(leftStructure.bottomRightAreaCorner.x, y);
        topLeftAreaCorner = new Vector2Int(rightStructure.topLeftAreaCorner.x, y + this.corridorWidth);
    }

    private int GetValidYForNeighourLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        if (rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return BSPStructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return BSPStructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return BSPStructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall)
                ).y;
        }
        if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return BSPStructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        return -1;
    }

    private void ProcessRoomInRelationUpOrDown(BSPNode structure1, BSPNode structure2)
    {
        BSPNode bottomStructure = null;
        List<BSPNode> structureBottmChildren = BSPStructureHelper.GetLowestLeavesFromGraph(structure1);
        BSPNode topStructure = null;
        List<BSPNode> structureAboveChildren = BSPStructureHelper.GetLowestLeavesFromGraph(structure2);

        var sortedBottomStructure = structureBottmChildren.OrderByDescending(child => child.topRightAreaCorner.y).ToList();

        if (sortedBottomStructure.Count == 1)
        {
            bottomStructure = structureBottmChildren[0];
        }
        else
        {
            int maxY = sortedBottomStructure[0].topLeftAreaCorner.y;
            sortedBottomStructure = sortedBottomStructure.Where(child => Mathf.Abs(maxY - child.topLeftAreaCorner.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomStructure.Count);
            bottomStructure = sortedBottomStructure[index];
        }

        var possibleNeighboursInTopStructure = structureAboveChildren.Where(
            child => GetValidXForNeighbourUpDown(
                bottomStructure.topLeftAreaCorner,
                bottomStructure.topRightAreaCorner,
                child.bottomLeftAreaCorner,
                child.bottomRightAreaCorner)
            != -1).OrderBy(child => child.bottomRightAreaCorner.y).ToList();
        if (possibleNeighboursInTopStructure.Count == 0)
        {
            topStructure = structure2;
        }
        else
        {
            topStructure = possibleNeighboursInTopStructure[0];
        }
        int x = GetValidXForNeighbourUpDown(
                bottomStructure.topLeftAreaCorner,
                bottomStructure.topRightAreaCorner,
                topStructure.bottomLeftAreaCorner,
                topStructure.bottomLeftAreaCorner);
        while (x == -1 && sortedBottomStructure.Count > 1)
        {
            sortedBottomStructure = sortedBottomStructure.Where(child => child.topLeftAreaCorner.x != topStructure.topLeftAreaCorner.x).ToList();
            bottomStructure = sortedBottomStructure[0];
            x = GetValidXForNeighbourUpDown(
                bottomStructure.topLeftAreaCorner,
                bottomStructure.topRightAreaCorner,
                topStructure.bottomLeftAreaCorner,
                topStructure.bottomRightAreaCorner);
        }
        bottomLeftAreaCorner = new Vector2Int(x, bottomStructure.topLeftAreaCorner.y);
        topRightAreaCorner = new Vector2Int(x + this.corridorWidth, topStructure.bottomLeftAreaCorner.y);
    }

    private int GetValidXForNeighbourUpDown(Vector2Int bottomNodeLeft,
        Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return BSPStructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return BSPStructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        if (bottomNodeLeft.x >= (topNodeLeft.x) && bottomNodeLeft.x <= topNodeRight.x)
        {
            return BSPStructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)

                ).x;
        }
        if (bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return BSPStructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)

                ).x;
        }
        return -1;
    }
    #endregion
}