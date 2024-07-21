using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TileType firstActiveTileType;
    
    private GestionalGridGenerator gridGenerator;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        firstActiveTileType = TileType.road;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GestionalTile tile = hit.collider.GetComponent<GestionalTile>();
                if (tile != null && !tile.occupied)
                {
                    //Debug.Log("Hai premuto su un tile: " + tile.gameObject.name);

                    tile.TriggerTile(firstActiveTileType);

                    List<GestionalTile> occupiedTiles = gridGenerator.GetOccupied(tile.GetSurroundingTiles());

                    tile.SetOccupied(occupiedTiles);

                    tile.gameObject.GetComponentInChildren<Road>().TriggerRoad();

                    gridGenerator.UpdateTiles(occupiedTiles);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GestionalTile tile = hit.collider.GetComponent<GestionalTile>();
                if (tile != null && !tile.occupied)
                {
                    House house = tile.gameObject.AddComponent<House>();
                    house.Initialize(tile);
                    house.StartSpread(gridGenerator.houseSpreadDuration);
                }
            }
        }
    }

    public void SetCamera(GestionalGridGenerator gridGenerator)
    {
        this.gridGenerator = gridGenerator;
        Camera mainCamera = Camera.main;

        mainCamera.gameObject.transform.position = new Vector3(gridGenerator.gridWidth / 2, gridGenerator.gridHeight, gridGenerator.gridHeight / 2);

        mainCamera.gameObject.transform.LookAt(new Vector3(gridGenerator.gridWidth / 2, 0, gridGenerator.gridHeight / 2));

    }

}
