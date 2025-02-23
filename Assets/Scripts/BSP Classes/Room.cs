using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static BSPGeneration;

[System.Serializable]
public class Room : MonoBehaviour
{
    [SerializeField]
    private int _x, _y, _width, _height, _originX, _originY, _roomId;

    GameObject _roomObject;

    public Partition partition;
 
    [SerializeField]
    Enums.RoomType _roomType;

    [SerializeField]
    Enums.RoomSubType _roomSubType;

    [SerializeField]
    public bool _discovered;

    public int x
    {
        get { return _x; }
        set { _x = value; }
    }

    public int y
    {
        get { return _y; }
        set { _y = value; }
    }

    public int width
    {
        get { return _width; }
        set { _width = value; }
    }

    public int height
    {
        get { return _height; }
        set { _height = value; }
    }

    public int originX
    {
        get { return _originX; }
        set { _originX = value; }
    }

    public int originY
    {
        get { return _originY; }
        set { _originY = value; }
    }

    public int roomId
    {
        get { return _roomId; }
        set { _roomId = value; }
    }

    public GameObject roomObject
    {
        get { return _roomObject; }
        set { _roomObject = value; }
    }

    public Enums.RoomType roomType
    {
        get { return _roomType; }
        set { _roomType = value; }
    }

    public Enums.RoomSubType roomSubType
    {
        get { return _roomSubType; }
        set { _roomSubType = value; }
    }

    public bool discovered{
        get { return _discovered; }
        set { _discovered = value; }
    }

    public void SetupRoom(int x, int y, int width, int height, int originX, int originY, int roomId, ref BSPGeneration.Partition partitionIn, bool discoveredIn = false)
    {
        _x = x;
        _y = y;
        _width = width;
        _height = height;
        _originX = originX;
        _originY = originY;
        _roomId = roomId;

        partition = partitionIn;

        //GameObject roomObj = new GameObject("Room_" +  roomId);

        //_roomObject = roomObj;

        _roomType = Enums.RoomType.Unassigned;
        _roomSubType = Enums.RoomSubType.Unassigned;
        _discovered = discoveredIn;
    }

    internal void SetupRoom(int x, int y, int width, int height, int originX, int originY, int roomId, ref BSP_Demo.Partition partitionIn, bool discoveredIn = false)
    {
         _x = x;
        _y = y;
        _width = width;
        _height = height;
        _originX = originX;
        _originY = originY;
        _roomId = roomId;

        partition = partitionIn;

        //GameObject roomObj = new GameObject("Room_" +  roomId);

        //_roomObject = roomObj;

        _roomType = Enums.RoomType.Unassigned;
        _roomSubType = Enums.RoomSubType.Unassigned;
        _discovered = discoveredIn;
    }
}
