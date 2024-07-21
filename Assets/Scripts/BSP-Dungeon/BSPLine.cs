using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BSPOrientation
{
    horinzontal = 0,
    vertical = 1
}

public class BSPLine : MonoBehaviour
{
    BSPOrientation orientation;
    Vector2Int coordinates;

    public BSPOrientation Orientation => orientation;
    public Vector2Int Coordinates => coordinates;

    public BSPLine(BSPOrientation orientation, Vector2Int coordinates)
    {
        this.coordinates = coordinates;
        this.orientation = orientation;
    }
}
