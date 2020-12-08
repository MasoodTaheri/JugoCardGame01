using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class SequentialsTest : MonoBehaviour
{
    public List<string> classNames;
    public List<int> numbers1;
    public List<string> clasName;
    //public int[] numbers;
    public List<int> myList;

    void Start()
    {
        //MissinNum();
        SortWithMyNum();
        //numbers = new int[56];
        //for (int i = 0; i < numbers.Length; i++)
        //{
        //    numbers[i] = i;
        //}
        //clasName = new List<string>(classNames.Count);
    }

    void SortWithMyNum()
    {
        //clasName = new List<string>(classNames.Count);
        Debug.LogError("classNames = " + classNames.Count);
        Debug.LogError("numbers1 = " + numbers1.Count);
        for (int i = 0; i < classNames.Count; i++)
        {
            Debug.LogError("classNames[numbers1[i]] = " + classNames[numbers1[i] - 1]);
            Debug.LogError("numbers1[i] = " + numbers1[i]);
            clasName.Add(classNames[numbers1[i] - 1]);
        }
    }

    void MissinNum()
    {
        myList.Sort();
        Debug.LogError("Numbers... ");
        foreach (int val in myList)
        {
            Debug.LogError(val);
        }

        int a, b;
        List<int> myList2 = new List<int>();
        List<int> remaining = new List<int>();
        for (int i = 0; i < myList.Count; i++)
        {
            a = myList[i];
            if (i < myList.Count - 1) {
                b = myList[i + 1];
            }
            else {
                break;
            }          

            myList2 = Enumerable.Range(a, b - a + 1).ToList();
            remaining = myList2.Except(myList).ToList();
            Debug.LogError("remaining.Count = " + remaining.Count);
            if (remaining.Count <= 1)
            {
                Debug.LogError("First Remaing Num = " + remaining[0]);
            }
            foreach (int res in remaining)
            {
                Debug.LogError(res);
            }
        }
    }
}