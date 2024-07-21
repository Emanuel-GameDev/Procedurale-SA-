using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    [Header("SELF REFERENCES")]
    [SerializeField] private MeshCollider tilableFLoor;

    [Header("ASSET REFERENCES")]
    [SerializeField] private WorldTile worldTilePrefab;
    [SerializeField] private List<RoomObject> roomObjectsPrefabList;

    [Header("SETTINGS")]
    [SerializeField] private int minObjectsCount;
    [SerializeField] private int maxObjectsCount;
    [SerializeField, Tooltip("Numero di tentativi per il posizionamento degli oggetti, dopo questo num non verranno inseriti altri oggetti")]
    private int placementTries;

    [Header("OBJECTS")]
    [SerializeField] private float objectDensity;
    [SerializeField] private float objectScale;

    public WorldTile[,] roomTiles;

    private void Awake()
    {
        Destroy(tilableFLoor);
    }

    public void Initialize()
    {
        // Spawn i tile dentro alla room
        Vector3 floorSize = tilableFLoor.bounds.size;
        Vector3 floorMin = tilableFLoor.bounds.min;

        Vector3 tileSize = worldTilePrefab.size;

        int tilesX = Mathf.FloorToInt(floorSize.x / tileSize.x);
        int tilesZ = Mathf.FloorToInt(floorSize.z / tileSize.z);

        roomTiles = new WorldTile[tilesX, tilesZ];

        for (int x = 0; x < tilesX; x++)
        {
            for (int z = 0; z < tilesZ; z++)
            {
                Vector3 tilePosition = floorMin + new Vector3(tileSize.x * x + tileSize.x / 2, 0, tileSize.z * z + tileSize.z / 2);
                WorldTile newTile = Instantiate(worldTilePrefab, tilePosition, Quaternion.identity);

                newTile.transform.SetParent(transform);
                roomTiles[x, z] = newTile;
            }
        }
    }

    public void StartSpawningObjects()
    {
        for (int x = 0; x < roomTiles.GetLength(0); x++)
        {
            for (int z = 0; z < roomTiles.GetLength(1); z++)
            {
                WorldTile currTile = roomTiles[x, z];
                Vector3 spawnPosition = currTile.transform.position;

                float objectNoise = Mathf.PerlinNoise(x * objectScale, z * objectScale);

                if (objectNoise < objectDensity)
                {
                    SpawnObjects(currTile, spawnPosition, roomObjectsPrefabList);
                    
                }
            }
        }
    }

    private void SpawnObjects(WorldTile tile, Vector3 spawnPosition, List<RoomObject> roomObjectsPrefabList)
    {
        if (!tile.occupied)
        {
            RoomObject newObject = Instantiate(roomObjectsPrefabList[UnityEngine.Random.Range(0, roomObjectsPrefabList.Count)]);
            newObject.transform.position = spawnPosition;

            tile.occupied = true;
        }
    }
}
