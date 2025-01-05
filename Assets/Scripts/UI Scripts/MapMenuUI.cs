using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapMenuUI : MonoBehaviour
{
    private InputController input;

    private UIDocument main_document;
    private VisualElement parentContainer;

    //Buttons
    private Button ButtonMarkerSafe;
    private Button ButtonMarkerLore;
    private Button ButtonMarkerDanger;

    public event EventHandler<MarkerArgs> OnMarkerChange;

    public class MarkerArgs : EventArgs
    {
        public int markerId;
        // 0 = Safe
        // 1 = Lore
        // 2 = Danger
    }

    void Awake()
    {
        input = GameObject.Find("InputController").GetComponent<InputController>();

        main_document = this.GetComponent<UIDocument>();

        parentContainer = main_document.rootVisualElement.Q("Container");

        ButtonMarkerSafe = main_document.rootVisualElement.Q("MapMarkerSafe") as Button;
        ButtonMarkerLore = main_document.rootVisualElement.Q("MapMarkerLore") as Button;
        ButtonMarkerDanger = main_document.rootVisualElement.Q("MapMarkerDanger") as Button;

        //init just in case
        parentContainer.style.display = DisplayStyle.None;

        //set button behavior
        ButtonMarkerSafe.clicked += () => SelectButton(ButtonMarkerSafe, "map-button-background-safe", 0);
        ButtonMarkerLore.clicked += () => SelectButton(ButtonMarkerLore, "map-button-background-lore", 1);
        ButtonMarkerDanger.clicked += () => SelectButton(ButtonMarkerDanger, "map-button-background-danger", 2);
    }

    private void OnEnable()
    {
        input.OnMapEnter += Event_OnMapEnter;
    }

    private void OnDisable()
    {
        input.OnMapEnter -= Event_OnMapEnter;
    }

    public void SelectButton(Button selectedButton, string buttonClass, int id)
    {

        ButtonMarkerSafe.RemoveFromClassList("selected");
        ButtonMarkerLore.RemoveFromClassList("selected");
        ButtonMarkerDanger.RemoveFromClassList("selected");

        selectedButton.AddToClassList("selected");

        OnMarkerChange.Invoke(this, new MarkerArgs { markerId = id});


    }

    public void Event_OnMapEnter(object sender, EventArgs e)
    {
        parentContainer.style.display = (parentContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
    }

}
