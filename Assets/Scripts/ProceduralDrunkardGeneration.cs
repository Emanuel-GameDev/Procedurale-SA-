using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralDrunkardGeneration : MonoBehaviour
{
    [SerializeField] List<GameObject> roomPrefabs;
    [SerializeField] int roomCount;
    [SerializeField] float minRoomDistance;
    [SerializeField] Transform startPivot;
    
    public bool clearRoomsOnNewGeneration;

    [SerializeField] private List<GameObject> _spawnedRooms;
    private Vector3 _currentPosition;
    [SerializeField] private List<Vector3> validPositions;

    private void Awake()
    {
        _currentPosition = startPivot.position;
        validPositions = new List<Vector3>();
    }

    private void Start()
    {
        if (clearRoomsOnNewGeneration && _spawnedRooms.Count > 0)
        {
            ClearRoomsSpawned();
            GenerateRooms();

        }

    }

    private void ClearRoomsSpawned()
    {
        foreach (var item in _spawnedRooms)
        {
            Destroy(item);
        }

        _spawnedRooms.Clear();
        validPositions.Clear();
        _currentPosition = startPivot.position;
    }

    private void GenerateRooms()
    {
        if (_spawnedRooms == null)
            _spawnedRooms = new List<GameObject>();
        else if (clearRoomsOnNewGeneration)
            _spawnedRooms.Clear();

        for (int i = 0; i < roomCount; i++)
        {
            Vector3 validPosition = GetValidPosition();
            GameObject newRoom = Instantiate(roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Count)], validPosition, Quaternion.identity);
            _spawnedRooms.Add(newRoom);
            validPositions.Add(validPosition);
        }
    }

    public void PopulateRooms()
    {
        _spawnedRooms.Select(x => x.GetComponent<RoomHandler>()).ToList().ForEach(r => r.Initialize());
    }

    public void FulfillRooms()
    {
        _spawnedRooms.Select(x => x.GetComponent<RoomHandler>()).ToList().ForEach(r => r.StartSpawningObjects());
    }

    private Vector3 GetValidPosition()
    {
        do
        {

            _currentPosition = GetDrunkardPosition(_currentPosition, (int)minRoomDistance);

        } while (!isPositionvalid(_currentPosition));

        return _currentPosition;
    }

    private static Vector3 GetDrunkardPosition(Vector3 currentPos, int minDistance)
    {
        Vector3 direction;
        int distance;

        do
        {
            direction = new Vector3(UnityEngine.Random.Range(-1, 2), 0, UnityEngine.Random.Range(-1, 2));
            distance = UnityEngine.Random.Range((int)-minDistance, (int)minDistance);

        } while (direction.magnitude == 0);

        currentPos += direction * distance;

        return currentPos;
    }

    private bool isPositionvalid(Vector3 testPosition)
    {
        if (validPositions.Contains(testPosition)) return false;

        foreach (Vector3 pos in validPositions)
        {
            if (Vector3.Distance(pos, testPosition) < minRoomDistance)
                return false;
        }

        return true;
    }

    private void ClearRoomsSpawnedImmediate()
    {
        foreach (var item in _spawnedRooms)
        {
            DestroyImmediate(item);
        }

        _spawnedRooms.Clear();
        validPositions.Clear();
        _currentPosition = startPivot.position;
    }

#if UNITY_EDITOR

    public void GenerateRoomsFromEditor()
    {
        if (clearRoomsOnNewGeneration)
        {
            ClearRoomsSpawnedImmediate();
        }

        GenerateRooms();
    }

#endif
}
