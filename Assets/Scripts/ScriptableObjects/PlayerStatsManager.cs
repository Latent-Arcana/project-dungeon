using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Stats Manager")]
public class PlayerStatsManager : ScriptableObject
{
    public int _MAX_HP = 10;
    public int _HP = 10;
    public int _SPD = 2;
    public int _AGI = 1;
    
    public UnityEvent<int> healthChangeEvent;

    private void OnEnable(){
        _HP = _MAX_HP;

        if(healthChangeEvent == null){
            healthChangeEvent = new UnityEvent<int>();
        }
    }

    public void DecreaseHealth(int amount){
        _HP -= amount;
        Debug.Log("decreasing the player's HP by " + amount);
        healthChangeEvent.Invoke(_HP);
    }

}
