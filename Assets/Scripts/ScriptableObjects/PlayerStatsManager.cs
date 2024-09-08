using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Stats Manager")]
public class PlayerStatsManager : ScriptableObject
{
    private int _MAX_HP = 10, _HP = 10, _SPD = 2, _AGI = 1;

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

    public event EventHandler OnHealthChanged;

    private void OnEnable()
    {
        _HP = _MAX_HP;
    }

    public void ModifyHP(int amount)
    {
        _HP += amount;
        OnHealthChanged.Invoke(this, EventArgs.Empty);
    }

}
