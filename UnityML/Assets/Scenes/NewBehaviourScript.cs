using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {
        var list = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        foreach (var v in list.Skip(1).Take(2)) 
        {
            Debug.Log(v);
        }
    }
}
