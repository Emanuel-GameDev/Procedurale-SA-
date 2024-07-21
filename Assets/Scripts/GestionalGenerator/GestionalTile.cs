using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileType
{
    road,
    house,
    turn,
    crossroad_T,
    crossroad_4,
    highway,
    highway_T
}

public class GestionalTile : MonoBehaviour, IGestionalSelectable
{
    public float squareDimension;

    public GestionalGridGenerator gridGenerator;
    public TileType tileType;
    public bool occupied = false;
    public bool isStatic = false;

    public bool northOccupied = false;
    public bool southOccupied = false;
    public bool eastOccupied = false;
    public bool westOccupied = false;
    public bool northEastOccupied = false;
    public bool northWestOccupied = false;
    public bool southEastOccupied = false;
    public bool southWestOccupied = false;

    public void OnDeselect(Action action)
    {
        action.Invoke();
    }

    public void OnSelect(Action action)
    {
        action.Invoke();
    }

    public void TriggerTile(TileType type)
    {
        tileType = type;

        switch (tileType)
        {
            case TileType.road:

                GameObject road = Instantiate(gridGenerator.roadPrefab, transform);
                road.transform.localScale = new Vector3(squareDimension, squareDimension, squareDimension);
                road.GetComponent<Road>().InitializeRoad(this);
                occupied = true;

                break;


        }
    }


    public List<GestionalTile> GetSurroundingTiles()
    {
        List<GestionalTile> surroundingTiles = new List<GestionalTile>();

        Vector3 center = transform.position + new Vector3(squareDimension / 2, 0, squareDimension / 2);

        float radius = squareDimension;

        Collider[] colliders = Physics.OverlapSphere(center, radius);

        foreach (Collider collider in colliders)
        {
            GestionalTile neighborTile = collider.GetComponent<GestionalTile>();
            if (neighborTile != null && neighborTile != this)
            {
                surroundingTiles.Add(neighborTile);
            }
        }

        return surroundingTiles;
    }


    internal List<GestionalTile> GetDiagonalTiles()
    {
        List<GestionalTile> surroundingTiles = GetSurroundingTiles();
        List<GestionalTile> diagonalTiles = new List<GestionalTile>();

        Vector3 currentPosition = transform.position + new Vector3(squareDimension / 2, 0, squareDimension / 2);

        // Definisci le direzioni relative da verificare
        Vector3[] diagonalDirections = new Vector3[]
        {
        new Vector3(-squareDimension, 0, squareDimension),  // Nord-Ovest
        new Vector3(-squareDimension, 0, -squareDimension), // Sud-Ovest
        new Vector3(squareDimension, 0, -squareDimension),  // Sud-Est
        new Vector3(squareDimension, 0, squareDimension)   // Nord-Est
        };

        foreach (Vector3 direction in diagonalDirections)
        {
            Vector3 targetPosition = currentPosition + direction;

            // Trova il tile corrispondente alla posizione target
            GestionalTile targetTile = surroundingTiles.FirstOrDefault(tile =>
                tile.gameObject.transform.position + new Vector3(tile.squareDimension / 2, 0, tile.squareDimension / 2) == targetPosition);

            if (targetTile != null)
            {
                diagonalTiles.Add(targetTile);
            }
        }

        return diagonalTiles;
    }

    internal void SetOccupied(List<GestionalTile> occupiedTiles)
    {
        Vector3 currentPosition = transform.position + new Vector3(squareDimension / 2, 0, squareDimension / 2);

        foreach (GestionalTile tile in occupiedTiles)
        {
            Vector3 tilePosition = tile.gameObject.transform.position + new Vector3(tile.squareDimension / 2, 0, tile.squareDimension / 2);

            if (tilePosition == currentPosition + new Vector3(0, 0, squareDimension))
                northOccupied = true;
            else if (tilePosition == currentPosition - new Vector3(0, 0, squareDimension))
                southOccupied = true;
            else if (tilePosition == currentPosition + new Vector3(squareDimension, 0, 0))
                eastOccupied = true;
            else if (tilePosition == currentPosition - new Vector3(squareDimension, 0, 0))
                westOccupied = true;
            else if (tilePosition == currentPosition + new Vector3(squareDimension, 0, squareDimension))
                northEastOccupied = true;
            else if (tilePosition == currentPosition - new Vector3(squareDimension, 0, squareDimension))
                southWestOccupied = true;
            else if (tilePosition == currentPosition + new Vector3(squareDimension, 0, -squareDimension))
                southEastOccupied = true;
            else if (tilePosition == currentPosition - new Vector3(squareDimension, 0, -squareDimension))
                northWestOccupied = true;
        }

        DetermineRoadType(occupiedTiles);
    }

    public void DetermineRoadType(List<GestionalTile> occupiedTiles)
    {
        // Determina il tipo di strada in base ai tile occupati vicini
        if (northOccupied && eastOccupied || northOccupied && westOccupied || southOccupied && eastOccupied || southOccupied && westOccupied)
        {
            tileType = TileType.turn;
        }
        if (northOccupied && eastOccupied && southOccupied || eastOccupied && southOccupied && westOccupied ||
                 northOccupied && westOccupied && southOccupied || westOccupied && northOccupied && eastOccupied)
        {
            tileType = TileType.crossroad_T;
        }
        if (southOccupied && eastOccupied && westOccupied && northOccupied)
        {
            tileType = TileType.crossroad_4;
        }
        if (northWestOccupied && westOccupied && northOccupied || northEastOccupied && eastOccupied && northOccupied ||
            southWestOccupied && westOccupied && southOccupied || southEastOccupied && eastOccupied && southOccupied)
        {
            tileType = TileType.highway;
        }
        if (northWestOccupied && westOccupied && northOccupied && eastOccupied && southWestOccupied && southOccupied ||
            southEastOccupied && southOccupied && eastOccupied && northEastOccupied && northOccupied && westOccupied)
        {
            tileType = TileType.highway_T;
        }
    }
}
