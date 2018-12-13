using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public static class LineRendererExtension
{
    /// <summary>
    /// Draws a trajectory passing by an array of positions
    /// </summary>
    /// <param name="_value">LineRenderer to use</param>
    /// <param name="_positions">Positions to pass by</param>
    public static void DrawTrajectory(this LineRenderer _value, Vector3[] _positions)
    {
        _value.positionCount = _positions.Length;
        _value.SetPositions(_positions);
    }
}
