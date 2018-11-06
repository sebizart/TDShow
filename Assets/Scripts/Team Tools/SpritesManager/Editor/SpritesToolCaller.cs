using UnityEditor;
using UnityEngine;


/*
[Script Header] SpritesToolCaller Version 0.0.1
Created by:
Date: 
Description:

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/

public class SpritesToolCaller : MonoBehaviour 
{
    [MenuItem("Tools/SpritesManager/Call SpritesManager")]


    #region Methods
    public static void CallManager()
    {
        if (FindObjectOfType<SpritesManager>()) return;
        GameObject _manager = new GameObject("SpritesManager", typeof(SpritesManager));
        Selection.activeGameObject = _manager;
    }
    #endregion
}
