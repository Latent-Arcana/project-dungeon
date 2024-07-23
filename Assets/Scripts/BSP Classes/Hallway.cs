using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hallway : MonoBehaviour
{

    private Vector2Int _horizontalStart, _verticalStart, _horizontalEnd, _verticalEnd;
    private int _room1Id, _room2Id;

    public int room1Id
    {
        get { return _room1Id; }
        set { _room1Id = value; }
    }

    public int room2Id
    {
        get { return _room2Id; }
        set { _room2Id = value; }
    }

    public Vector2Int horizontalStart
    {
        get { return _horizontalStart; }
        set { _horizontalStart = value; }
    }

    public Vector2Int horizontalEnd
    {
        get { return _horizontalEnd; }
        set { _horizontalEnd = value; }
    }

    public Vector2Int verticalStart
    {
        get { return _verticalStart; }
        set { _verticalStart = value; }
    }

    public Vector2Int verticalEnd
    {
        get { return _verticalEnd; }
        set { _verticalEnd = value; }
    }

    public Hallway(int horizontalXStart, int horizontalXEnd, int horizontalY, int verticalYStart, int verticalYEnd, int verticalX)
    {

        //Two coordinates of the vertical line
        _horizontalStart = new Vector2Int(horizontalXStart, horizontalY);
        _horizontalEnd = new Vector2Int(horizontalXEnd, horizontalY);

        //Two coordinates of the horizontal line
        _verticalStart = new Vector2Int(verticalYStart, verticalX);
        _verticalEnd = new Vector2Int(verticalYEnd, verticalX);

    }

    public Hallway(Vector2Int horizontalStart, Vector2Int horizontalEnd, Vector2Int verticalStart, Vector2Int verticalEnd)
    {
        _horizontalStart = horizontalStart;
        _horizontalEnd = horizontalEnd;

        //Two coordinates of the horizontal line
        _verticalStart = verticalStart;
        _verticalEnd = verticalEnd;
    }

}
