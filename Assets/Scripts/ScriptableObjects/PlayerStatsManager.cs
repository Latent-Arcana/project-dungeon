using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


[CreateAssetMenu(menuName = "Stats Manager")]
public class PlayerStatsManager : ScriptableObject
{
    // PLAYER BASE STATS
    public int BASE_MAX_HP = 10;
    public int BASE_AGI = 1;
    public int BASE_SPD = 2;
    public int BASE_STR = 1;
    public int BASE_AP = 0;

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
        public int oldValue;
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
        int temp = _HP;
        _HP = newValue;
        OnHealthChanged.Invoke(this, new Stats_Args { newValue = newValue, oldValue = temp });
    }

    public void SetAGI(int newValue)
    {
        int temp = _AGI;
        _AGI = newValue;
        OnAgilityChanged.Invoke(this, new Stats_Args { newValue = newValue, oldValue = temp });
    }

    public void SetSTR(int newValue)
    {
        int temp = _STR;
        _STR = newValue;
        OnStrengthChanged.Invoke(this, new Stats_Args { newValue = newValue, oldValue = temp });
    }

    public void SetSPD(int newValue)
    {
        int temp = _SPD;
        _SPD = newValue;
        OnSpeedChanged.Invoke(this, new Stats_Args { newValue = newValue, oldValue = temp });
    }

    public void SetAP(int newValue)
    {
        int temp = _AP;
        _AP = newValue;
        OnArmorPointsChanged.Invoke(this, new Stats_Args { newValue = newValue, oldValue = temp });
    }

    public void SetMaxHP(int newValue)
    {
        int temp = _MAX_HP;
        _MAX_HP = newValue;
        OnMaxHealthChanged.Invoke(this, new Stats_Args { newValue = newValue, oldValue = temp });
    }

}
