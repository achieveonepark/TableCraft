using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;

public class StringToListConverter
{
    public static List<T> ConvertStringToList<T>(string input)
    {
        List<T> result = new List<T>();

        if (!string.IsNullOrEmpty(input))
        {
            string[] parts = input.Split(',');

            foreach (string part in parts)
            {
                T value = (T)Convert.ChangeType(part.Trim(), typeof(T));
                result.Add(value);
            }
        }

        return result;
    }

    public static T[] ConvertStringToArray<T>(string input)
    {
        List<T> result = new List<T>();

        try
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            string[] parts = input.Split(',');

            foreach (string part in parts)
            {
                T value = (T)Convert.ChangeType(part.Trim(), typeof(T));
                result.Add(value);
            }
        }
        catch
        {

        }

        return result.ToArray();
    }

    public static string ToString<T>(T[] array)
    {
        if (array == null || array.Length == 0)
            return string.Empty;

        return string.Join(",", array);
    }

    public static string ToString<T>(List<T> array)
    {
        if (array == null || array.Count == 0)
            return string.Empty;

        return string.Join(",", array);
    }
}