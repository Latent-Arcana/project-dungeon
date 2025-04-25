
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class StorePack : MonoBehaviour
{

    private Dictionary<int, Pack> staticPacks;

    private Dictionary<int, Pack> randomPacks;


    //don't destroy on load
    public static StorePack Store_Pack { get; set; }

    void Awake()
    {
        // SINGLETON CHECK
        if (Store_Pack == null)
        {
            Store_Pack = this;
        }
        else if (Store_Pack != this)
        {
            Destroy(this);
        }

        //DontDestroyOnLoad(Store_Pack);


        //Default Packs that always show in the store
        //I'm referencing these by ID in the functions, so make sure to update in both spots
        //We are also referencing these by ID in ContainerGeneration to determin what itmes to give the player
        staticPacks = new()
        {
            {0, new Pack(0,"No Pack", "Default", 0)},
            {1, new Pack(1,"Basic Weapon Pack", "Default", 20)},
            {2, new Pack(2,"Basic Armor Pack", "Default", 30)},
            {3, new Pack(3,"Advanced Weapon Pack", "Default", 40)},
            {4, new Pack(4,"Advanced Armor Pack", "Default", 50)},
        };


        //initialize list of packs that can be grabbed randomly to fill out the rest of the store slots
        //We are referencing these by ID in ContainerGeneration to determin what itmes to give the player
        randomPacks = new()
        {
            //basic
            {10, new Pack(10,"Basic Alchemist Pack", "Roles", 101)},
            {11, new Pack(11,"Basic Healer Pack", "Roles", 11)},
            {12, new Pack(12,"Basic Strength Pack", "Stats", 22)},
            {13, new Pack(13,"Basic Agility Pack", "Stats", 33)},
            {14, new Pack(14,"Basic Speed Pack", "Stats", 44)},
            {15, new Pack(15,"Basic HP Pack", "Stats", 15)},
            {16, new Pack(16,"Basic AP Pack", "Stats", 166)},

            //advanced
            {20, new Pack(20,"Advanced Alchemist Pack", "Roles", 100)},
            {21, new Pack(21,"Advanced Healer Pack", "Roles", 123)},
            {22, new Pack(22,"Advanced Strength Pack", "Stats", 740)},
            {23, new Pack(23,"Advanced Agility Pack", "Stats", 702)},
            {19, new Pack(19,"Advanced Speed Pack", "Stats", 703)},
            {18, new Pack(18,"Advanced HP Pack", "Stats", 707)},
            {17, new Pack(17,"Advanced AP Pack", "Stats", 780)},
        };

    }


    public Pack GetNoPack()
    {
        return staticPacks[0];
    }

    public Pack GetBasicWeaponPack()
    {
        return staticPacks[1];
    }

    public Pack GetBasicArmorPack()
    {
        return staticPacks[2];
    }

    public Pack GetAdvnacedWeaponPack()
    {
        return staticPacks[3];
    }

    public Pack GetAdvancedArmorPack()
    {
        return staticPacks[4];
    }


    public List<Pack> GetRandomPacks(int numberOfPacks = 4)
    {
        List<Pack> results = new();

        while (results.Count < numberOfPacks)
        {

            //find a random number within the size of the available random packs
            int randomIndexInDictionary = UnityEngine.Random.Range(0, randomPacks.Count);

            //first get the key at the selected index
            int randomKey = randomPacks.ElementAt(randomIndexInDictionary).Key;

            //then get the pack at that key
            Pack tempPack = randomPacks[randomKey];

            if (!results.Contains(tempPack))
            {
                results.Add(tempPack);
            }
            // else, we already picked that pack, 
            // so continue in the WHILE loop until we pick enough distinct packs

        }

        return results;
    }




    // //helper functions for 
    // public Pack GetNoPack(List<int> excludeIdsList)
    // {
    //get a random Pack, but not one in the excludeIdsList
    // }




    //object to hold info about each pack
    public class Pack
    {

        private int _price, _packId;
        private string _name, _type;


        public int price
        {
            get { return _price; }
            set { _price = value; }
        }

        public int packId
        {
            get { return _packId; }
            set { _packId = value; }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string type
        {
            get { return _type; }
            set { _type = value; }
        }


        public Pack(int setPackId, string setName, string setType, int setPrice)
        {
            this._packId = setPackId;
            this._price = setPrice;
            this._name = setName;
            this._type = setType;
        }

    }





}