using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SplashScreenUIBehavior : MonoBehaviour
{

    public UIDocument main_document;
    public VisualElement anyKeyText;

    void Awake()
    {
        main_document = GetComponent<UIDocument>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
