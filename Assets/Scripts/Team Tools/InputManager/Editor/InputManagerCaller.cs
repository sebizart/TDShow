using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InputManagerCaller : Editor
{
    [MenuItem("Tools/InputManager/Call InputManager")]
    #region Meth
    public static void CallManager()
    {
        if (FindObjectOfType<InputsManager>()) return;
        GameObject _manager = new GameObject("InputManager", typeof(InputsManager));
        Selection.activeGameObject = _manager;
    }
    #endregion
}
