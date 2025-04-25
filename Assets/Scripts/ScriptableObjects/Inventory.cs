using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory")]
public class Inventory : ScriptableObject
{
    public List<Item> items;
    public List<int> currentDurability;
    public int equippedArmor;
    public int equippedWeapon;

    public int selected_pack;


    [SerializeField]
    [Header("DEBUG")]
    [Tooltip("Reset the inventory on scene load?")]
    bool DEBUG_RESET = false;


    public void OnEnable()
    {
        if (DEBUG_RESET)
        {
            Reset();
        }

    }

    public void Reset()
    {        
        items.Clear();
        currentDurability.Clear();

        equippedArmor = -1;
        equippedWeapon = -1;
    }

    public void SetPack(int packId){
        selected_pack = packId;
    }
}