using System;


///
/// This class will be used to set up our loot tables. This will allow us to manage a roll of 1-100, and then determine what item gets chosen
/// The range is [minValue, maxValue]
///
[Serializable]
public class LootItem
{
    public string itemID;
    public string itemName;
    public int minValue;
    public int maxValue;

    public LootItem(string _itemID, string _itemName, int _minValue, int _maxValue){
        itemID = _itemID;
        itemName = _itemName;
        minValue = _minValue;
        maxValue = _maxValue;
    }
}

[Serializable]
public class LootItemData
{
    public string itemID;
    public string itemName;
    public int DropChance;

}



