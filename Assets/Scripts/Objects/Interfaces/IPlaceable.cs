using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IPlaceable
{
   List<Enums.RoomType> RoomTypes
   {
      get;
   }

   List<Enums.RoomSubType> RoomSubTypes{
      get;
   }

   Enums.PlacementType PlacementType{
      get;
   }

   int Width
   {
      get;
   }

   int Height{
      get;
   }

   int MaximumNumberAllowed{
      get;
   }

}
