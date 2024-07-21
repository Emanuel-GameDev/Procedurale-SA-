using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public float spreadDuration; 

    private GestionalTile currentTile;

    public void Initialize(GestionalTile tile)
    {
        currentTile = tile;
    }

    public void StartSpread(float duration)
    {
        spreadDuration = duration;

        StartCoroutine(SpreadCoroutine());
    }

    private IEnumerator SpreadCoroutine()
    {
        float endTime = Time.time + spreadDuration;

        while (Time.time < endTime)
        {
            if (currentTile.gridGenerator.usePerling)
            {
                SpreadWithPerlinNoise();
            }
            else
            {
                SpreadRandomly();
            }

            yield return null;
        }
    }

    private void SpreadRandomly()
    {
        List<GestionalTile> surroundings = currentTile.GetSurroundingTiles();

        List<GestionalTile> freeTiles = surroundings.FindAll(tile => !tile.occupied);

        if (freeTiles.Count > 0)
        {
            GestionalTile selectedTile = freeTiles[Random.Range(0, freeTiles.Count)];

            GenerateHouseInTile(selectedTile);

            currentTile = selectedTile;
            currentTile.occupied = true;
            currentTile.tileType = TileType.house;
        }
    }

    private void SpreadWithPerlinNoise()
    {
        List<GestionalTile> surroundings = currentTile.GetSurroundingTiles();
        List<GestionalTile> freeTiles = surroundings.FindAll(tile => !tile.occupied);

        if (freeTiles.Count > 0)
        {
            freeTiles.Sort((tileA, tileB) =>
            {
                float noiseA = Mathf.PerlinNoise(tileA.transform.position.x * 0.1f, tileA.transform.position.z * 0.1f);
                float noiseB = Mathf.PerlinNoise(tileB.transform.position.x * 0.1f, tileB.transform.position.z * 0.1f);

                return noiseA.CompareTo(noiseB);
            });

            GestionalTile selectedTile = freeTiles[0];

            GenerateHouseInTile(selectedTile);

            currentTile = selectedTile;
            currentTile.occupied = true;
            currentTile.tileType = TileType.house;

        }
    }

    private void GenerateHouseInTile(GestionalTile tile)
    {
        GameObject housePrefab = currentTile.gridGenerator.housePrefab;

        if (housePrefab != null)
        {
            float houseWidth = Random.Range(0.1f, 1);
            float houseDepth = Random.Range(0.1f, 1);
            float houseHeight = Random.Range(0.1f, 1);

            GameObject house = Instantiate(housePrefab, tile.gameObject.transform);
            house.transform.position = tile.transform.position + new Vector3(tile.squareDimension / 2, 0, tile.squareDimension / 2);

            house.transform.localScale = new Vector3(houseWidth, houseHeight, houseDepth);

        }
    }
}
