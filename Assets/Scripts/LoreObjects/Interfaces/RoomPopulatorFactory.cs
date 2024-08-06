using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class RoomPopulatorFactory
{

    public static IRoomPopulator GetPopulator(GameObject[] loreObjects)
    {

        int subCategory = UnityEngine.Random.Range(0, loreObjects.Length);

        List<GameObject> roomObjects = new List<GameObject>();


        switch (subCategory)
        {
            case 0:

                Debug.Log("room was selected as library");
                roomObjects = loreObjects.Where(x => x.GetComponent<LoreObjectBehavior>().SubType == Enums.LoreRoomSubType.Library).ToList();

                return new LibraryPopulator(roomObjects);
            case 1:

                Debug.Log("room was selected as treasure");
                roomObjects = loreObjects.Where(x => x.GetComponent<LoreObjectBehavior>().SubType == Enums.LoreRoomSubType.Treasure).ToList();

                return new TreasurePopulator(roomObjects);

            // case 2:
            //     return new LibraryPopulator();

            // case 3:
            //     return new TreasurePopulator();
            // Add cases for other room types
            default:
                throw new ArgumentException("Invalid room type");
        }

    }
}