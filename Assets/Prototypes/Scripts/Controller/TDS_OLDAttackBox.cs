using System;
using UnityEngine;

/*
[Script Header] TDS_AttackBox Version 0.0.1
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

[Serializable]
public class TDS_OLDAttackBox 
{
    #region Fields/Properties
    // The name of this box
    [SerializeField] private string name = "Attack Detection Box";
    public string Name
    {
        get { return name; }
    }

    // Should this box be drawn by gizmo
    public bool isVisible = false;

    // The color used to draw this detection box
    [SerializeField] private Color color = Color.green;
    public Color Color
    {
        get { return color; }
    }

    // The center position of the detection box
    [SerializeField] private Vector3 centerPosition = Vector3.zero;
    public Vector3 CenterPosition
    {
        get { return centerPosition; }
    }

    // The half of the size of the detection box
    [SerializeField] private Vector3 halfExtents = Vector3.one;
    public Vector3 HalfExtents
    {
        get { return halfExtents; }
    }
    #endregion
}
