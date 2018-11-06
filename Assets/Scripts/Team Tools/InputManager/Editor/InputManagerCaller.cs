using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InputManagerCaller : Editor
{
    #region InputManager
    [MenuItem("Tools/InputManager/Call InputManager")]    
    public static void CallManager()
    {
        if (FindObjectOfType<InputsManager>()) return;
        GameObject _manager = new GameObject("InputManager", typeof(InputsManager));
        Selection.activeGameObject = _manager;
    }
    #endregion

    #region SwitchControllerManager
    [MenuItem("Tools/InputManager/Call Switch Controller Manager")]
    public static void CallSwitchManager()
    {
        if (!FindObjectOfType<InputsManager>() || FindObjectOfType<SwitchControllerManager>())
        {
            Debug.Log("You have to Call InputManager first!");
            return;
        }
        var _inputManager = FindObjectOfType<InputsManager>();
        GameObject _switchmanager = new GameObject("SwitchControllerManager", typeof(SwitchControllerManager));
        _switchmanager.transform.parent = _inputManager.transform;
        Debug.Log("Switch controller manager added to InputManager!");

        /*        
        Selection.activeGameObject = _manager;
        */
    }
    #endregion
}
