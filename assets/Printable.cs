using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Printable : MonoBehaviour {

	public static void printDicInt(Dictionary<int,int>  d)
    {
        foreach(int key in d.Keys)
        {
            Debug.Log(key + " -- " + d[key]);
        }
    }
}
