using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DungeonNarrator : MonoBehaviour
{
 
    // Defining the narrator as a singleton
    public static DungeonNarrator Dungeon_Narrator { get; private set; }


    private UIDocument narrator_doc;
    private TextElement text_narrator;

    private void Awake()
    {
        if (Dungeon_Narrator != null && Dungeon_Narrator != this)
        {
            Destroy(this);
        }
        else
        {
            Dungeon_Narrator = this;
        }



        narrator_doc = this.GetComponent<UIDocument>();
        text_narrator = narrator_doc.rootVisualElement.Q("DungeonNarratorText") as TextElement;
        ClearDungeonNarratorText();

    }

    public void SetDungeonNarratorText(string message)
    {
        text_narrator.text = message;
    }

    public void ClearDungeonNarratorText()
    {
        SetDungeonNarratorText("");
    }

    public void AddDungeonNarratorText(string message)
    {

        if(text_narrator.text.Length > 2500)
        {
            text_narrator.text = text_narrator.text.Remove(2000);
        }
        //TODO: This should eventually be truncated so it doesn't overflow the string variable
        text_narrator.text = message + "\n\n" + text_narrator.text; //substring text_narrator.text to  anumber of lines or something
    }


}
