using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BSPDungeonCreator : MonoBehaviour
{
    public int DungeonWidth;
    public int DungeonHeight;
    public int MinRoomWidth;
    public int MinRoomHeight;
    public int MaxIterations;

    [Range(0, 0.3f)]
    public float RoomBottomCornerModifier;
    [Range(0.7f, 1f)]
    public float RoomTopCornerModifier;
    [Range(0, 2)]
    public int RoomOffset;

    public Material FloorMaterial;

    public int corridorWidth;

    [Header("WallSettings")]
    [SerializeField] GameObject wallVertical;
    [SerializeField] GameObject wallHorizontal;

    List<Vector3Int> possibleDoorVecticalPosition;
    List<Vector3Int> possibileDoorHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;

    private void Start()
    {
        CreateDungeon();
        
    }

    private void CreateDungeon()
    {
        var dungeonGenerator = new BSPDungeonGenerator(DungeonWidth, DungeonHeight);

        // preparare il dungeon = lista di stanze
        List<BSPNode> roomList = dungeonGenerator.CalculateDungeon(
            MaxIterations, MinRoomWidth, MinRoomHeight, RoomBottomCornerModifier, RoomTopCornerModifier, RoomOffset, corridorWidth);

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.SetParent(transform);

        possibileDoorHorizontalPosition = new List<Vector3Int>();
        possibleDoorVecticalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();

        // creare la mesh delle stanze
        foreach (BSPNode node in roomList)
        {
            CreateMesh(node.bottomLeftAreaCorner, node.topRightAreaCorner);
        }



        CreateWalls(wallParent);
    }

    private void CreateWalls(GameObject wallparent)
    {
        // Creiamo le mura in base alle posizioni
        foreach (var wallPos in possibleWallHorizontalPosition)
        {
            Instantiate(wallHorizontal, wallPos, Quaternion.identity, wallparent.transform);
        }

        foreach (var wallPos in possibleWallVerticalPosition)
        {
            Instantiate(wallVertical, wallPos, Quaternion.identity, wallparent.transform);
        }
    }

    private void CreateMesh(Vector2Int bottomLeftCorner, Vector2Int topRightCorner)
    {
        Vector3 bottomLeftVertex = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightVertex = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftVertex = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightVertex = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftVertex,
            topRightVertex,
            bottomLeftVertex,
            bottomRightVertex
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = FloorMaterial;

        // eseguiamo il codice sotto solo per pigrizia, perché noi programmatori siamo pigri
        // Creaiamo le posizioni

        //orizzonatale
        for (int row = (int)bottomLeftVertex.x; row < (int)bottomRightVertex.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftVertex.z);
            AddWallToPositionList(wallPosition, possibileDoorHorizontalPosition, possibleWallHorizontalPosition);
        }

        for (int row = (int)topLeftVertex.x; row < (int)topRightVertex.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightVertex.z);
            AddWallToPositionList(wallPosition, possibileDoorHorizontalPosition, possibleWallHorizontalPosition);
        }

        //verticale
        for (int column = (int)bottomLeftVertex.z; column < (int)topLeftVertex.z; column++)
        {
            var wallPosition = new Vector3(bottomLeftVertex.z, 0, column);
            AddWallToPositionList(wallPosition, possibleDoorVecticalPosition, possibleWallVerticalPosition);
        }

        for (int column = (int)bottomRightVertex.z; column < (int)topRightVertex.z; column++)
        {
            var wallPosition = new Vector3(bottomRightVertex.z, 0, column);
            AddWallToPositionList(wallPosition, possibleDoorVecticalPosition, possibleWallVerticalPosition);
        }
    }

    private void AddWallToPositionList(Vector3 wallPosition, List<Vector3Int> doorSpaceList, List<Vector3Int> wallPositionList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);

        if (wallPositionList.Contains(point))
        {
            doorSpaceList.Add(point);
            wallPositionList.Remove(point);
        }
        else
        {
            wallPositionList.Add(point);
        }

    }
}
