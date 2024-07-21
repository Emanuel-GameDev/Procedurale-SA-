using System;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    [SerializeField] Terrain terrain;
    [SerializeField] int width = 10;
    [SerializeField] int length = 10;
    [SerializeField] float maxHeight = 10;

    [Header("Envrioment Settings")]
    [SerializeField] float treesDensity = 0.5f;
    [SerializeField] float treesNoiseScale = 0.5f;

    [SerializeField] float rocksDensity = 0.5f;
    [SerializeField] float rocksNoiseScale = 0.5f;

    [SerializeField] float bushesDensity = 0.5f;
    [SerializeField] float bushesNoiseScale = 0.5f;

    [Header("Perlin Layers")]
    [SerializeField] List<PerlinSettings> perlinLayers = new();

    [Header("Elements Objects")]
    [SerializeField] List<GameObject> treesPrefabs = new();
    [SerializeField] List<GameObject> rocksPrefabs = new();
    [SerializeField] List<GameObject> bushesPrefabs = new();



    private void OnValidate()
    {
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    private void Start()
    {
        PopulateTerrain(terrain.terrainData);
    }

    private void PopulateTerrain(TerrainData terrainData)
    {
        bool[,] objectMap = new bool[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                float currentHeight = terrainData.GetHeight(x, z);

                Vector3 spawnPosition = new Vector3(x, currentHeight, z);

                float treeNoise = Mathf.PerlinNoise(x * treesNoiseScale, z * treesNoiseScale);
                float rockNoise = Mathf.PerlinNoise(x * rocksNoiseScale, z * rocksNoiseScale);
                float bushNoise = Mathf.PerlinNoise(x * bushesNoiseScale, z * bushesNoiseScale);

                if (treeNoise < treesDensity)
                    SpawnObjectOnMap(objectMap, x, z, spawnPosition, treesPrefabs);
                if (rockNoise < rocksDensity)
                    SpawnObjectOnMap(objectMap, x, z, spawnPosition, rocksPrefabs);
                if (bushNoise < bushesDensity)
                    SpawnObjectOnMap(objectMap, x, z, spawnPosition, bushesPrefabs);
            }
        }
    }

    private void SpawnObjectOnMap(bool[,] objectMap, int x, int z, Vector3 spawnPosition, List<GameObject> prefabList)
    {
        if (!objectMap[x, z])
        {
            GameObject newTree = Instantiate(treesPrefabs[UnityEngine.Random.Range(0, prefabList.Count)]);
            newTree.transform.position = spawnPosition;

            objectMap[x, z] = true;
        }
    }

    private TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width;
        terrainData.size = new Vector3(width, maxHeight, length);
        float[,] heights = new float[width, length];

        foreach (PerlinSettings settings in perlinLayers)
        {
            HeightsPerlin(ref heights, width, length, settings.MinHeight, settings.MaxHeight, settings.Scale, settings.Seed, settings.MaxRandomness, settings.UseLerp, settings.Subtract);
        }

        terrainData.SetHeights(0, 0, heights);

        return terrainData;
    }

    private void HeightsPerlin(ref float[,] heights, int width, int length, float minHeight, float maxHeight, float scale, int seed, int maxRandomness, bool useLerp, bool subtract)
    {
        System.Random random = new System.Random(seed);
        float offsetX = random.Next(0, maxRandomness);
        float offsetY = random.Next(0, maxRandomness);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                float xCoordinate = (float)x / width * scale + offsetX;
                float yCoordinate = (float)y / length * scale + offsetY;
                float perlinHeightsValue = Mathf.PerlinNoise(xCoordinate, yCoordinate);

                if (useLerp)
                    heights[x, y] += subtract ? -Mathf.Lerp(minHeight, maxHeight, perlinHeightsValue) : Mathf.Lerp(minHeight, maxHeight, perlinHeightsValue);
                else
                    heights[x, y] += subtract ? -Mathf.Clamp(perlinHeightsValue, minHeight, maxHeight) : Mathf.Clamp(perlinHeightsValue, minHeight, maxHeight);
            }
        }
    }
}

[Serializable]
public class PerlinSettings
{
    public float MinHeight = 0;
    public float MaxHeight = 10;
    public float Scale = 20;
    public int MaxRandomness = 1000;
    public int Seed;
    public bool UseLerp = false;
    public bool Subtract = false;
}