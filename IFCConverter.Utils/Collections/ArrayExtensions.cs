using System.Collections.Generic;

namespace IFCConverter.Utils.Collections
{
    public static class ArrayExtensions
    {
        public static T[] Flatten<T>(this T[][] array)
        {
            List<T> result = new List<T>();

            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    result.Add(array[i][j]);
                }
            }

            return result.ToArray();
        }
    }
}