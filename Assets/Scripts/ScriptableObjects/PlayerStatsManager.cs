using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


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

    public event EventHandler<HP_Args> OnHealthChanged;

    public class HP_Args : EventArgs
    {
        public int newValue;
    }
    private void OnEnable()
    {
        _HP = _MAX_HP;
    }

    public void Initialize(){
        _HP = 10;
        _SPD = 2;
        _AGI = 1;
    }

    public void SetHP(int newValue)
    {
        _HP = newValue;
        OnHealthChanged.Invoke(this, new HP_Args { newValue = newValue });
    }

}
