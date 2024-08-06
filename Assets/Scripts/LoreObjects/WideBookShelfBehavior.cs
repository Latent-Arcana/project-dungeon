using UnityEngine;

public class WideBookShelfBehavior : LoreObjectBehavior
{
    public override Enums.LoreRoomSubType SubType => Enums.LoreRoomSubType.Library;

    public override Enums.ObjectType ObjectType => Enums.ObjectType.Wide;

    public override int Width => 2;

    public override int Height => 1;

    public override bool IsWallSpawn => true;
    public override int MaximumNumberAllowed => 4;


    public override GameObject Interact()
    {
        return null;
    }
}
