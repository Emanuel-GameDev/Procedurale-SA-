using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionalGridGenerator : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public int tileCount;
    public Material tileMaterial;
    public Dictionary<GestionalTile, List<GestionalTile>> worldSectionTileMap;
    public int innerTileCount;
    public Material innerTileMaterial;
    public float houseSpreadDuration;
    [Tooltip("true = use perling for generation, true = use random walk for generation")]
    public bool usePerling = false;

    public GameObject roadPrefab;
    public GameObject stopLightPrefab;
    public GameObject housePrefab;

    private void Start()
    {
        var initialTileList = GenerateGrid(gameObject, gridWidth, gridHeight, tileCount, tileMaterial, true);

        worldSectionTileMap = new Dictionary<GestionalTile, List<GestionalTile>>();

        foreach (var singleTile in initialTileList)
        {
            worldSectionTileMap.Add(singleTile, GenerateGrid(singleTile.gameObject, (int)singleTile.squareDimension, (int)singleTile.squareDimension, innerTileCount, innerTileMaterial, false));
        }

        GameManager.instance.SetCamera(this);
    }

    public List<GestionalTile> GenerateGrid(GameObject parent, int gridWidth, int gridHeight, int tileSquareCount, Material tileMaterial, bool singleTileStaticStatus)
    {
        List<GestionalTile> resultList = new List<GestionalTile>();
        float tileSizeX = (float)gridWidth / tileSquareCount;
        float tileSizeZ = (float)gridHeight / tileSquareCount;

        for (int x = 0; x < tileSquareCount; x++)
        {
            for (int z = 0; z < tileSquareCount; z++)
            {
                Mesh newMesh = GenerateNewMesh(tileSizeX, tileSizeZ);
                GameObject newTile = new GameObject($"Tile_{x}_{z}");
                newTile.transform.SetParent(parent.transform);
                newTile.transform.localPosition = new Vector3(x * tileSizeX, 0, z * tileSizeZ);

                MeshFilter meshFilter = newTile.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = newTile.AddComponent<MeshRenderer>();

                meshRenderer.material = tileMaterial;
                meshFilter.mesh = newMesh;

                GestionalTile gestionalTile = newTile.AddComponent<GestionalTile>();
                gestionalTile.squareDimension = tileSizeX;
                gestionalTile.gridGenerator = this;

                gestionalTile.isStatic = singleTileStaticStatus;
                if (!gestionalTile.isStatic)
                    newTile.AddComponent<MeshCollider>();

                resultList.Add(gestionalTile);

            }
        }

        return resultList;
    }

    internal void UpdateTiles(List<GestionalTile> occupiedTiles)
    {
        foreach (GestionalTile tile in occupiedTiles)
        {
            List<GestionalTile> temp = GetOccupied(tile.GetSurroundingTiles());
            tile.SetOccupied(temp);
            if (tile.gameObject.GetComponentInChildren<Road>() != null)
                tile.gameObject.GetComponentInChildren<Road>().TriggerRoad();
        }
    }

    internal List<GestionalTile> GetOccupied(List<GestionalTile> surroundings)
    {
        List<GestionalTile> resultList = new List<GestionalTile>();

        foreach (GestionalTile tile in surroundings)
        {
            if (tile.occupied && tile.tileType != TileType.house)
                resultList.Add(tile);
        }

        return resultList;
    }

    public static Mesh GenerateNewMesh(float xPosition, float zPosition)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertces = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(xPosition,0,0),
            new Vector3(0,0,zPosition),
            new Vector3(xPosition,0,zPosition),
        };

        int[] triangles = new int[]
        {
            0,2,1,2,3,1
        };

        mesh.vertices = vertces;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;

    }
}




//[Serializable]
//public class WorldSection
//{
//    public int gridWidth;
//    public int gridHeight;
//    public int tileCount;
//    public Material tileMaterial;
//    public WorldSection childSection;

//    public Dictionary<GestionalTile,List<GestionalTile>> worldSectionTileMap { get; }

//    public WorldSection()
//    {
//        worldSectionTileMap = new Dictionary<GestionalTile, List<GestionalTile>>();
//    }

//    public void AddNewTile(GestionalTile tile,List<GestionalTile> gestionalTiles = null)
//    {
//        if (worldSectionTileMap.ContainsKey(tile)) return;

//        worldSectionTileMap.Add(tile, gestionalTiles);
//    }
//    public void RemoveTile(GestionalTile tile)
//    {
//        if (!worldSectionTileMap.ContainsKey(tile)) return;

//        worldSectionTileMap.Remove(tile);
//    }
//}