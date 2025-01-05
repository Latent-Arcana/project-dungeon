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
    public List<int> mappedDungeons;
    public ExplorationData(int _dungeonsFullyMapped, int _roomsMappedSuccessfully, int _dungeonsVisited, int _cartographersLost, int _enemiesKilled, List<int> _visitedDungeons, List<int> _mappedDungeons)
    {
        dungeonsFullyMapped = _dungeonsFullyMapped;
        roomsMappedSuccessfully = _roomsMappedSuccessfully;
        dungeonsVisited = _dungeonsVisited;
        cartographersLost = _cartographersLost;
        enemiesKilled = _enemiesKilled;
        visitedDungeons = _visitedDungeons;
        mappedDungeons = _mappedDungeons;
    }

    public ExplorationData()
    {
        dungeonsFullyMapped = 0;
        roomsMappedSuccessfully = 0;
        dungeonsVisited = 0;
        cartographersLost = 0;
        enemiesKilled = 0;
        visitedDungeons = new List<int>();
        mappedDungeons = new List<int>();
    }

    public void IncrementPlayerSavedData(ExplorationData currentRun)
    {
        dungeonsFullyMapped += currentRun.dungeonsFullyMapped;
        roomsMappedSuccessfully += currentRun.roomsMappedSuccessfully;
        dungeonsVisited += currentRun.dungeonsVisited;
        cartographersLost += currentRun.cartographersLost;
        enemiesKilled += currentRun.enemiesKilled;

        foreach (int dungeon in currentRun.visitedDungeons)
        {
            if (!visitedDungeons.Contains(dungeon))
            {
                visitedDungeons.Add(dungeon);
            }
        }

        foreach (int dungeon in currentRun.mappedDungeons)
        {
            if (!mappedDungeons.Contains(dungeon))
            {
                mappedDungeons.Add(dungeon);
            }
        }
    }

    public void DEBUG_AddDungeonsToList(int numberOfDungeons)
    {

        for (int i = 0; i < numberOfDungeons; ++i)
        {

            int randValue = UnityEngine.Random.Range(0, 10000);
            if(!visitedDungeons.Contains(randValue)){
                visitedDungeons.Add(randValue);
            }

        }

        for (int i = 0; i < numberOfDungeons; ++i)
        {

            int randValue = UnityEngine.Random.Range(0, 10000);
            if(!mappedDungeons.Contains(randValue)){
                mappedDungeons.Add(randValue);
            }

        }


    }

}