using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Test Event")]
public class TestEvent : ScriptableObject
{
    private List<TestEventListener> listeners = new List<TestEventListener>();


    public void TriggerEvent()
    {
        for (int i = listeners.Count -1; i >= 0; i--)
        {
            listeners[i].OnEventTriggered();
        }
    }

    public void AddListener(TestEventListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(TestEventListener listener)
    {
        listeners.Remove(listener);
    }
}
