using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Enums;


public class LoreObjectBehavior : MonoBehaviour, IInteractable
{
    public ScriptableObject loreObject;
    public virtual GameObject Interact()
    {
        throw new System.NotImplementedException();
    }
}
