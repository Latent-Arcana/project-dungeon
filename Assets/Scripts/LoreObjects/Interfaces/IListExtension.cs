using System.Collections.Generic;

public static class IListExtension {


    /// <summary>
    /// Fisher-Yates shuffle is a simple algorithm for shuffling a list. 
    /// See more <see href="https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle">here</see>
    /// </summary>
    public static void Shuffle<T>(this IList<T> ls){
        
        int count = ls.Count;
        
        for(int i = 0; i < ls.Count; ++i){

            int rand = UnityEngine.Random.Range(i, count);

            var temp = ls[i];
            ls[i] = ls[rand];
            ls[rand] = temp;

        }

    }
}