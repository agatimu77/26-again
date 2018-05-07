using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {

	public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prg = new System.Random(seed);
        for (int i = 0; i < array.Length-1; i++)
        {
            int randomIndex = prg.Next(i, array.Length);
            T temporalItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = temporalItem;
            
        }
        return array;
    }
}
