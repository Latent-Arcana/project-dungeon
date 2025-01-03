using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{

    ///Saves the stats for the entire run, to display on the Game Over screen


    public PlayerStatsManager Player_Stats; // scriptable object (persistent, until game over)
    public Inventory Player_Inventory; // scriptable object (persistent, until game over)

    [SerializeField]
    private int Numerator;
    [SerializeField]
    private int Denominator;

    private int RoomsVisited;

    private string DeathText;


    ExplorationData currentRunData;


    void Awake()
    {
        NewGame();
    }

    public void InitializeCurrentRunData()
    {
        currentRunData = new ExplorationData();
    }


    /// <summary>
    /// Sets score to the given values
    /// </summary>
    /// <param name="numerator">Total number of correct room tags</param>
    /// <param name="denominator">Total number of rooms</param>
    public void SetScore(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;

    }

    public void AddToScore(int numerator, int denominator)
    {
        Numerator += numerator;
        Denominator += denominator;
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

        Debug.Log("Numerator is: " + GetNumerator());
        Debug.Log("Denominator is: " + GetDenominator());
        // get numerator and denominator data so we can update the current run
        if (GetNumerator() == GetDenominator()) // TODO: REVISIT
        {
            IncrementFullyMappedDungeons();
        }

        IncrementDungeonsVisited(); // we know we're going to increment this any time we swap portals (DO THIS ON DEATH TOO)

        SetRoomsSuccessfullyMapped(GetNumerator()); // set how many room we've visited

        Debug.Log("We just set the rooms succesfully mapped to: " + currentRunData.roomsMappedSuccessfully + " with numerator: " + GetNumerator());
    }


    public void SaveStats()
    {
        Debug.Log("SAVING PLAYER DATA");
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
        currentRunData.roomsMappedSuccessfully = roomsCount;
    }

    public void IncrementCartographersLost()
    {
        currentRunData.cartographersLost++;
    }

    public void IncrementEnemiesKilled()
    {
        currentRunData.enemiesKilled++;
    }


}