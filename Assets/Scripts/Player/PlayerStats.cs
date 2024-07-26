using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static DungeonNarrator;

public class PlayerStats : MonoBehaviour
{

    //private int _HP_MAX = 10;
    public int _HP = 10;
    public int _SPD = 2;
    public int _AGI = 1;


    private UIDocument narrator_doc;

    private TextElement healthText;


    // Start is called before the first frame update
    void Start()
    {
       //healthText = narrator_doc.rootVisualElement.Q("HealthText") as TextElement;

       //testEvent.TriggerEvent();

    }

    public void SetHealth (int health){
        //_HP = health;
        //healthText.text = "HP: " + health.ToString();
    }

    public int GetCurrentHealth(){
        return _HP;
    }

    //TODO: Do the rest of the stats

}
