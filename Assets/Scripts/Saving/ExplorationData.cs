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

    public ExplorationData(){
        dungeonsFullyMapped = 0;
        roomsMappedSuccessfully = 0;
        dungeonsVisited = 0;
        cartographersLost = 0;
        enemiesKilled = 0;
    }

    public void IncrementPlayerSavedData(ExplorationData currentRun){
        dungeonsFullyMapped += currentRun.dungeonsFullyMapped;
        roomsMappedSuccessfully += currentRun.roomsMappedSuccessfully;
        dungeonsVisited += currentRun.dungeonsVisited;
        cartographersLost += currentRun.cartographersLost;
        enemiesKilled += currentRun.enemiesKilled;

    }

}