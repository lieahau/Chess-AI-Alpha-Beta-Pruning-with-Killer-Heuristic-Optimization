using System;
using System.Collections.Generic;

public static class Shuffle
{
    private static Random rng = new Random();
    public static void ShuffleList<T>(this IList<T> list)
    {
        for(var i=list.Count; i > 0; i--)
            list.Swap(0, rng.Next(0, i));
    }

    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
}
