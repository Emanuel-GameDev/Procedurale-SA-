using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Road : MonoBehaviour
{
    private GestionalTile associatedTile;

    private List<GameObject> lines = new List<GameObject>();
    private List<GameObject> guardrails = new List<GameObject>();

    public void InitializeRoad(GestionalTile tile)
    {
        associatedTile = tile;

        InitializeLines();
        InitializeGuardrails();
    }

    private void InitializeLines()
    {
        for (int i = 1; i <= 4; i++)
        {
            lines.Add(gameObject.transform.GetChild(i).gameObject);
        }

        TriggerList(lines, false, false, false, false);
    }

    private void InitializeGuardrails()
    {
        for (int i = 5; i <= 8; i++)
        {
            guardrails.Add(gameObject.transform.GetChild(i).gameObject);
        }

        TriggerList(guardrails, false, false, false, false);
    }

    private void TriggerList(List<GameObject> list, bool north, bool south, bool east, bool west)
    {
        list[0].SetActive(north);
        list[1].SetActive(south);
        list[2].SetActive(east);
        list[3].SetActive(west);
    }

    internal void TriggerRoad()
    {
        switch (associatedTile.tileType)
        {
            case TileType.road:

                if (associatedTile.northOccupied || associatedTile.southOccupied)
                {
                    TriggerList(lines, true, true, false, false);
                    TriggerList(guardrails, false, false, true, true);
                }
                else if (associatedTile.eastOccupied || associatedTile.westOccupied)
                {
                    TriggerList(lines, false, false, true, true);
                    TriggerList(guardrails, true, true, false, false);
                }

                break;

            case TileType.turn:

                if (associatedTile.northOccupied && associatedTile.eastOccupied)
                {
                    TriggerList(lines, true, false, true, false);
                    TriggerList(guardrails, false, true, false, true);
                }
                else if (associatedTile.eastOccupied && associatedTile.southOccupied)
                {
                    TriggerList(lines, false, true, true, false);
                    TriggerList(guardrails, true, false, false, true);
                }
                else if (associatedTile.southOccupied && associatedTile.westOccupied)
                {
                    TriggerList(lines, false, true, false, true);
                    TriggerList(guardrails, true, false, true, false);
                }
                else if (associatedTile.westOccupied && associatedTile.northOccupied)
                {
                    TriggerList(lines, true, false, false, true);
                    TriggerList(guardrails, false, true, true, false);
                }

                break;

            case TileType.crossroad_T:

                if (associatedTile.northOccupied && associatedTile.eastOccupied && associatedTile.southOccupied)
                {
                    TriggerList(lines, true, true, true, false);
                    TriggerList(guardrails, false, false, false, true);
                }
                else if (associatedTile.eastOccupied && associatedTile.southOccupied && associatedTile.westOccupied)
                {
                    TriggerList(lines, false, true, true, true);
                    TriggerList(guardrails, true, false, false, false);
                }
                else if (associatedTile.northOccupied && associatedTile.southOccupied && associatedTile.westOccupied)
                {
                    TriggerList(lines, true, true, false, true);
                    TriggerList(guardrails, false, false, true, false);
                }
                else if (associatedTile.northOccupied && associatedTile.eastOccupied && associatedTile.westOccupied)
                {
                    TriggerList(lines, true, false, true, true);
                    TriggerList(guardrails, false, true, false, false);
                }

                break;

            case TileType.crossroad_4:

                TriggerList(lines, true, true, true, true);
                TriggerList(guardrails, false, false, false, false);

                List<GestionalTile> diagonalTiles = associatedTile.GetDiagonalTiles();
                List<int> rotations = new List<int>() { 180, 90, 0, -90 };
                List<Vector3> positions = new List<Vector3>() { new Vector3(associatedTile.squareDimension, 0, 0),
                                                                new Vector3(associatedTile.squareDimension, 0, associatedTile.squareDimension),
                                                                new Vector3(0, 0, associatedTile.squareDimension),
                                                                new Vector3(0, 0, 0)};

                for (int i = 0; i < diagonalTiles.Count; i++)
                {
                    GameObject light = Instantiate(associatedTile.gridGenerator.stopLightPrefab, diagonalTiles[i].gameObject.transform);
                    light.transform.localPosition = positions[i];
                    light.transform.localRotation = Quaternion.Euler(0, rotations[i], 0);
                }

                break;

            case TileType.highway:

                if (associatedTile.northWestOccupied && associatedTile.westOccupied && associatedTile.northOccupied ||
                    associatedTile.southWestOccupied && associatedTile.westOccupied && associatedTile.southOccupied)
                {
                    TriggerList(lines, true, true, false, false);
                    TriggerList(guardrails, false, false, true, false);
                }
                else if (associatedTile.northEastOccupied && associatedTile.eastOccupied && associatedTile.northOccupied ||
                    associatedTile.southEastOccupied && associatedTile.eastOccupied && associatedTile.southOccupied)
                {
                    TriggerList(lines, true, true, false, false);
                    TriggerList(guardrails, false, false, false, true);
                }

                break;

            case TileType.highway_T:

                if (associatedTile.northWestOccupied && associatedTile.westOccupied && associatedTile.northOccupied &&
                    associatedTile.eastOccupied && associatedTile.southWestOccupied && associatedTile.southOccupied)
                {
                    TriggerList(lines, true, true, true, false);
                    TriggerList(guardrails, false, false, false, false);
                }
                else if (associatedTile.northEastOccupied && associatedTile.eastOccupied && associatedTile.northOccupied &&
                         associatedTile.westOccupied && associatedTile.southEastOccupied && associatedTile.southOccupied)
                {
                    TriggerList(lines, true, true, false, true);
                    TriggerList(guardrails, false, false, false, false);
                }

                break;

            default:
                break;
        }
    }
}
