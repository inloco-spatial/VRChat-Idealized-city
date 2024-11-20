using System;
using UnityEngine;

namespace Stasok.Utils
{
    public static class ArrayUtils
    {
        public static void Resize<T>(ref T[] array, int newSize)
        {
            if (array.Length >= newSize)
                return;

            T[] newArray = new T[newSize];
            Array.Copy(array, newArray, array.Length);
            array = newArray;
        }

        public static void IncreaseSize<T>(ref T[] array, int addSize)
        {
            addSize += array.Length;
            if (array.Length >= addSize)
                return;

            T[] newArray = new T[addSize];
            Array.Copy(array, newArray, array.Length);
            array = newArray;
        }

        public static void SetActive(GameObject[] toggles, bool state)
        {
            if (!ValidateArray(toggles))
                return;

            foreach (var toggle in toggles)
                if (toggle)
                    toggle.SetActive(state);
        }

        public static bool ValidateArray<T>(T[] arr)
        {
            if (arr == null || arr.Length == 0)
                return false;
            return true;
        }

        public static void AddToNext<T>(ref T[] arr, T newElement, ref int freeIndex, int stepSize)
        {
            if (!ValidateArray(arr) || freeIndex >= arr.Length)
                IncreaseSize(ref arr, stepSize);

            arr[freeIndex++] = newElement;
        }
    }
}