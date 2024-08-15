using UnityEngine;
using static Enums;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : Item
{
    [Header("Weapon Properties")]
    public string useVerb;
}
