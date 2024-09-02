using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DEBUG_All_Item_Data : MonoBehaviour
{

    private UIDocument main_document;
    private VisualElement parentContainer;

    [SerializeField]
    public VisualTreeAsset template;

    private ItemLoader itemLoader;

    private List<Item> items;


    void Awake()
    {
        main_document = this.GetComponent<UIDocument>();
        parentContainer = main_document.rootVisualElement.Q("Container_Scroll");

        //player = GameObject.Find("Player");
        itemLoader = GameObject.Find("DungeonGenerator").GetComponent<ItemLoader>();

        items = itemLoader.GetItemsDatabase();

        itemLoader.LoadItemsFromJson();

        foreach (Item item in items)
        {

            var tempElement = template.Instantiate();
            //tempElement.text = "Some Text" + i.ToString();

            Sprite sprt = Resources.Load<Sprite>(item.image);
            tempElement.Q("Image").style.backgroundImage = new StyleBackground(sprt);


            TextElement tempText = tempElement.Q("Name") as TextElement;
            tempText.text = "Name:" + item.itemName;

            tempText = tempElement.Q("Stats") as TextElement;
            tempText.text = "Stats:" + item.statsText;

            tempText = tempElement.Q("Description") as TextElement;
            tempText.text = "Description:" + item.itemDescription;

            parentContainer.Add(tempElement);




        }




    }


    // Update is called once per frame
    void Update()
    {

    }
}
