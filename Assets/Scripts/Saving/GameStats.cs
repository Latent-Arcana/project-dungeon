using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{

    ///Saves the stats for the entire run, to display on the Game Over screen


    public PlayerStatsManager Player_Stats;
    public Inventory Player_Inventory;

    [SerializeField]
    private int Numerator;
    [SerializeField]
    private int Denominator;

    private int RoomsVisited;

    private string DeathText;


    ExplorationData currentRunData;


    void Awake()
    {
        InitializeCurrentRunData();
    }

    public void InitializeCurrentRunData()
    {
        currentRunData = new ExplorationData();
    }


    /// <summary>
    /// Sets the final game score to be displayed on the Game Over screen
    /// </summary>
    /// <param name="numerator">Total number of correct room tags</param>
    /// <param name="denominator">Total number of rooms</param>
    public void SetScore(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;

    }

    public void AddToScore(int numerator)
    {
        Numerator += numerator;
    }

    /// <summary>
    /// Sets the total number of rooms viisted to be displayed on the Game Over screen
    /// </summary>
    public void SetRoomsVisited(int roomsVisited)
    {
        RoomsVisited = roomsVisited;
    }

    public void NewGame()
    {
        SetScore(0, 0);
        SetRoomsVisited(0);
        InitializePlayer(); // TODO: Continue to extend
        InitializeCurrentRunData();
    }

    public void InitializePlayer()
    {
        Player_Stats.Initialize();
        Player_Inventory.Reset();
    }

    /// <summary>
    /// Increments the total number of rooms viisted by 1. To be displayed on the Game Over screen.
    /// </summary>
    public void IncrementRoomsVisited()
    {
        RoomsVisited++;
    }

    public int GetRoomsVisited()
    {
        return RoomsVisited;
    }

    public int GetNumerator()
    {
        return Numerator;
    }

    public int GetDenominator()
    {
        return Denominator;
    }

    public void SetDeathText(string deathText)
    {
        DeathText = deathText;
    }

    public string GetDeathText()
    {
        return DeathText;
    }

    public void UpdateCurrentRoomAndDungeonData()
    {
        // get numerator and denominator data so we can update the current run
        if (GetNumerator() == GetDenominator())
        {
            IncrementFullyMappedDungeons();
        }

        IncrementDungeonsVisited(); // we know we're going to increment this any time we swap portals

        SetRoomsSuccessfullyMapped(GetNumerator()); // set how many room we've visited
    }


    public void SaveStats()
    {
        ExplorationData loadedData = SaveSystem.LoadPlayerSaveData();

        if (loadedData != null)
        {
            loadedData.IncrementPlayerSavedData(currentRunData);
            SaveSystem.SaveExplorationData(loadedData);
        }

        else{
            SaveSystem.SaveExplorationData(currentRunData);
        }


    }


    public void IncrementFullyMappedDungeons()
    {
        currentRunData.dungeonsFullyMapped++;
    }

    public void IncrementDungeonsVisited()
    {
        currentRunData.dungeonsVisited++;
    }

    public void SetRoomsSuccessfullyMapped(int roomsCount)
    {
        currentRunData.roomsMappedSuccessfully += roomsCount;
    }

    public void IncrementCartographersLost()
    {
        currentRunData.cartographersLost++;
    }

    public void IncrementEnemiesKilled()
    {
        currentRunData.enemiesKilled++;
        Debug.Log(currentRunData.enemiesKilled);
    }


}