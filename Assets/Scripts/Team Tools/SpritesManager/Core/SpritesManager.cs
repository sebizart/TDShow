using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] SpritesManager Version 0.0.1
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

public class SpritesManager : MonoBehaviour 
{
    #region Fields/Properties
    public List<Transform> AllSpritesToMove = new List<Transform>();
    #endregion

    #region Methods
    public void AddObject(Transform _transforms)
    {
        if (!_transforms) return;
        AllSpritesToMove.Add(_transforms);
    }

    public void RemoveAllSprites() => AllSpritesToMove.Clear();

    public void RemoveSpriteAt(int _s) => AllSpritesToMove.RemoveAt(_s);
    #endregion
}
