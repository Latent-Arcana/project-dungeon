using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    [SerializeField]
    private int Numerator;
    [SerializeField]
    private int Denominator;

    private int RoomsVisited;

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

    /// <summary>
    /// Sets the total number of rooms viisted to be displayed on the Game Over screen
    /// </summary>
    public void SetRoomsVisited(int roomsVisited)
    {
        RoomsVisited = roomsVisited;
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


}