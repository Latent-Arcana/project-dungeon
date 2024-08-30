using System;
using System.Buffers.Text;
using UnityEngine;
using static Enums;

[Serializable]
public abstract class Item : ScriptableObject
{
    [Header("Base Item Information")]
    public string itemName;
    public string itemDescription;
    public ItemType type;
    public string image;


    [Header("Stat Buffs and Debuffs")]
    public int STR;
    public int SPD;
    public int AGI;
    public int HP;
}

