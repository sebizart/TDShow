using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

#region DETECTION AREA
//HEADER
// A zone from a center position with a specified size which detect every damageableElement into its range
[Serializable]
public class TDS_DetectionArea
{
    public Vector3 CenterPosition { get; set; }

    [SerializeField]float extendedX = 1;
    public float ExtendedX { get { return extendedX; } }
    [SerializeField] float extendedY = 1;
    public float ExtendedY { get { return extendedY; } }
    [SerializeField] float extendedZ = 1;
    public float ExtendedZ { get { return extendedZ; } }


    public Vector3 ExtendingPosition { get { return new Vector3(extendedX, extendedY, extendedZ); } }


    [SerializeField] Color debugColor = Color.red; 
    public Color DebugColor { get { return debugColor; } }

    public TDS_DamageableElement[] GetDetectedElements()
    {
        return Physics.OverlapBox(CenterPosition, ExtendingPosition).Select(c => c.GetComponent<TDS_DamageableElement>()).ToArray();
    }

    /// <summary>
    /// Update the Area scale with 3 float
    /// </summary>
    /// <param name="_x">scale.x</param>
    /// <param name="_y">scale.y</param>
    /// <param name="_z">scale.z</param>
    public void UpdateAreaScale(float _x, float _y, float _z)
    {
        extendedX = _x;
        extendedY = _y;
        extendedZ = _z; 
    }
}
#endregion