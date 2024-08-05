using System;

public static class RoomPopulatorFactory {
    
    public static IRoomPopulator GetPopulator(){

        int subCategory = UnityEngine.Random.Range(0, 2);


        switch (subCategory)
        {
            case 0:
                return new LibraryPopulator();
            case 1:
                return new TreasurePopulator();

            // case 2:
            //     return new LibraryPopulator();

            // case 3:
            //     return new TreasurePopulator();
            // Add cases for other room types
            default:
                throw new ArgumentException("Invalid room type");
        }

    }
}