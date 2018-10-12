using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object; 

/*
[Script Header] LogSaverMenu Version 0.0.1
Created by: Alexis Thiebaut
Date: 04/10/2018
Description: Initialise the LogSaver Gameobject
            If it's not in the scene, create it
            If it's not complete, add the missing scripts

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/

public class TDS_LogSaverMenu 
{

    #region Fields/Properties

    #endregion

    #region Methods
    [MenuItem("Tools/LogSaver/InitLogSaver")]
    static void InitLogSaver()
    {
        GameObject _obj;
        if (Object.FindObjectOfType<TDS_LogSaver>())
        {
            _obj = Object.FindObjectOfType<TDS_LogSaver>().gameObject;
            if (!_obj.GetComponent<TDS_LoggerManager>())
            {
                _obj.AddComponent<TDS_LoggerManager>();
            }
        }
        else if(Object.FindObjectOfType<TDS_LoggerManager>())
        {
            _obj = Object.FindObjectOfType<TDS_LoggerManager>().gameObject;
            _obj.AddComponent<TDS_LogSaver>();
        }
        else
        {
            _obj = new GameObject();
            _obj.AddComponent<TDS_LoggerManager>();
            _obj.AddComponent<TDS_LogSaver>();
        }
        _obj.name = "LogSaverManager";
        Selection.activeGameObject = _obj;
    }
    #endregion


}
