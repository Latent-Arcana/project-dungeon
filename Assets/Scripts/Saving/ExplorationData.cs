using System;
using UnityEngine;

[System.Serializable]
public class ExplorationData
{
    public int dungeonsFullyMapped;
    public int roomsMappedSuccessfully;
    public int dungeonsVisited;
    public int cartographersLost;
    public int enemiesKilled;

    public ExplorationData(int _dungeonsFullyMapped, int _roomsMappedSuccessfully, int _dungeonsVisited, int _cartographersLost, int _enemiesKilled)
    {
        dungeonsFullyMapped = _dungeonsFullyMapped;
        roomsMappedSuccessfully = _roomsMappedSuccessfully;
        dungeonsVisited = _dungeonsVisited;
        cartographersLost = _cartographersLost;
        enemiesKilled = _enemiesKilled;
    }

}