using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


[CreateAssetMenu(menuName = "Stats Manager")]
public class PlayerStatsManager : ScriptableObject
{
    // PLAYER BASE STATS
    int BASE_MAX_HP = 10;
    int BASE_AGI = 1;
    int BASE_SPD = 2;
    int BASE_STR = 1;
    int BASE_AP = 0;

    private int _MAX_HP = 10, _HP = 10, _SPD = 2, _AGI = 1, _STR = 1, _AP = 0;

    public int MAX_HP
    {
        get { return _MAX_HP; }
    }

    public int HP
    {
        get { return _HP; }
    }

    public int SPD
    {
        get { return _SPD; }
    }

    public int AGI
    {
        get { return _AGI; }
    }

    public int STR
    {
        get { return _STR; }
    }

    public int AP
    {
        get { return _AP; }
    }

    public event EventHandler<Stats_Args> OnHealthChanged;
    public event EventHandler<Stats_Args> OnAgilityChanged;
    public event EventHandler<Stats_Args> OnSpeedChanged;
    public event EventHandler<Stats_Args> OnStrengthChanged;
    public event EventHandler<Stats_Args> OnArmorPointsChanged;
    public event EventHandler<Stats_Args> OnMaxHealthChanged;

    public class Stats_Args : EventArgs
    {
        public int newValue;
    }
    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        _MAX_HP = BASE_MAX_HP;
        _HP = BASE_MAX_HP;
        _SPD = BASE_SPD;
        _AGI = BASE_AGI;
        _STR = BASE_STR;
        _AP = BASE_AP;
    }

    public void SetHP(int newValue)
    {
        _HP = newValue;
        OnHealthChanged.Invoke(this, new Stats_Args { newValue = newValue });
    }

    public void SetAGI(int newValue)
    {
        _AGI = newValue;
        OnAgilityChanged.Invoke(this, new Stats_Args { newValue = newValue });
    }

    public void SetSTR(int newValue)
    {
        _STR = newValue;
        OnStrengthChanged.Invoke(this, new Stats_Args { newValue = newValue });
    }

    public void SetSPD(int newValue)
    {
        _SPD = newValue;
        OnSpeedChanged.Invoke(this, new Stats_Args { newValue = newValue });
    }

    public void SetAP(int newValue)
    {
        _AP = newValue;
        OnArmorPointsChanged.Invoke(this, new Stats_Args { newValue = newValue });
    }

    public void SetMaxHP(int newValue)
    {
        _MAX_HP = newValue;
        OnMaxHealthChanged.Invoke(this, new Stats_Args { newValue = newValue });
    }

}
