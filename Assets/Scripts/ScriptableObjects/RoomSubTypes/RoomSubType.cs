using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

//[CreateAssetMenu(menuName = "Room Subtype")]
public class RoomSubType : ScriptableObject
{
    // [SerializeField]
    // public abstract Dictionary<Enums.ObjectType, int> maxAllowed;

    // public Enums.ObjectType[] RequiredObjects;

    // public Enums.ObjectType[] DecorObjects;

    private List<Enums.ObjectType> _requiredObjects;
    private List<Enums.ObjectType> _decorObjects;

    private Dictionary<Enums.ObjectType, int> _maxAllowed;


    public virtual List<Enums.ObjectType> RequiredObjects
    {
        get { return _requiredObjects; }
        set { _requiredObjects = value; }
    }

    public virtual List<Enums.ObjectType> DecorObjects
    {
        get { return _decorObjects; }
        set { _decorObjects = value; }
    }

    public virtual Dictionary<Enums.ObjectType, int> MaxAllowed
    {
        get { return _maxAllowed; }
        set { _maxAllowed = value; }
    }

}
