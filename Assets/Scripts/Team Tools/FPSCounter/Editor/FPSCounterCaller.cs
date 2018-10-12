using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
[Script Header] FPSCounterCaller Version 0.0.1
Created by: Will
Date: 10/10/2018
Description: Instantiate FPSCounter UI in the scene

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/

public class FPSCounterCaller : MonoBehaviour 
{
    [MenuItem("Tools/FPSCounter/Call FPSCounter")]
     
    #region Methods
    public static void CallFPSCounter()
    {
        if (FindObjectOfType<Canvas>())
        {
            Canvas _existingCanva = FindObjectOfType<Canvas>();
            GameObject _go = Instantiate(Resources.Load("FPSText"), Vector3.zero, Quaternion.identity) as GameObject;
            _go.transform.parent = _existingCanva.transform;
            _go.transform.position = Vector3.zero;
        }

        else
            Instantiate(Resources.Load("FPSCounterPref"), Vector3.zero, Quaternion.identity);
    }
    #endregion
}
