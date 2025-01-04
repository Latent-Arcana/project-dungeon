using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ExplorationData
{
    public int dungeonsFullyMapped;
    public int roomsMappedSuccessfully;
    public int dungeonsVisited;
    public int cartographersLost;
    public int enemiesKilled;
    public List<int> visitedDungeons;
    public ExplorationData(int _dungeonsFullyMapped, int _roomsMappedSuccessfully, int _dungeonsVisited, int _cartographersLost, int _enemiesKilled, List<int> _visitedDungeons)
    {
        dungeonsFullyMapped = _dungeonsFullyMapped;
        roomsMappedSuccessfully = _roomsMappedSuccessfully;
        dungeonsVisited = _dungeonsVisited;
        cartographersLost = _cartographersLost;
        enemiesKilled = _enemiesKilled;
        visitedDungeons = _visitedDungeons;
    }

    public ExplorationData(){
        dungeonsFullyMapped = 0;
        roomsMappedSuccessfully = 0;
        dungeonsVisited = 0;
        cartographersLost = 0;
        enemiesKilled = 0;
        visitedDungeons = new List<int>();
    }

    public void IncrementPlayerSavedData(ExplorationData currentRun){
        dungeonsFullyMapped += currentRun.dungeonsFullyMapped;
        roomsMappedSuccessfully += currentRun.roomsMappedSuccessfully;
        dungeonsVisited += currentRun.dungeonsVisited;
        cartographersLost += currentRun.cartographersLost;
        enemiesKilled += currentRun.enemiesKilled;

        foreach(int dungeon in currentRun.visitedDungeons){
            if(!visitedDungeons.Contains(dungeon)){
                visitedDungeons.Add(dungeon);
            }
        }
    }

}