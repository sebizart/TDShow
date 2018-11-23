﻿using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] TDS_Triangle Version 0.0.1
Created by: Alexis Thiébaut
Date: 21/11/2018
Description: 

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/
[Serializable]
public class TDS_Triangle
{
    #region Fields and properties
    [SerializeField] int[] verticesIndex = new int[3];
    public int[] VerticesIndex { get { return verticesIndex; } }

    [SerializeField] float xPos, yPos, zPos;
    public Vector3 CenterPosition
    {
        get
        {
            return new Vector3(xPos, yPos, zPos);
        }
        set
        {
            xPos = value.x;
            yPos = value.y;
            zPos = value.z;

        }
    }

    [SerializeField] List<int> middleSegmentIndex = new List<int>();
    public List<int> MiddleSegmentIndex { get { return middleSegmentIndex; } }

    [SerializeField] List<TDS_Triangle> linkedTriangles = new List<TDS_Triangle>(); 
    public List<TDS_Triangle> LinkedTriangles { get { return linkedTriangles; } }

    public bool HasBeenLinked = false; 
    
    #endregion

    #region Constructor
    public TDS_Triangle(int[] _navPoints)
    {
        for (int i = 0; i < verticesIndex.Length; i++)
        {
            verticesIndex[i] =_navPoints[i];
        }
    }
    #endregion

    #region Methods
    #endregion
}

public struct Barycentric
{
    public float u;
    public float v;
    public float w; 

    /// <summary>
    /// Return if u, v and w are greater or equal than 0 and less or equal than 1
    /// That means the point is inside of the triangle
    /// </summary>
    public bool IsInside
    {
        get
        {
            return (u >= 0.0f) && (u <= 1.0f) && (v >= 0.0f) && (v <= 1.0f) && (w >= 0.0f); //(w <= 1.0f)
        }
    }

    /// <summary>
    /// Calculate 3 value u, v, w as:
    /// aP = u*aV1 + v*aV2 + w*aV3
    /// if u, v and w are greater than 0, that means that the point is in the triangle aV1 aV2 aV3.
    /// </summary>
    /// <param name="aV1">First point of the triangle</param>
    /// <param name="aV2">Second point of the triangle</param>
    /// <param name="aV3">Third point of the triangle</param>
    /// <param name="aP">Point to get the Barycentric coordinates</param>
    public Barycentric(Vector3 aV1, Vector3 aV2, Vector3 aV3, Vector3 aP)
    {
        Vector3 a = aV2 - aV3;
        Vector3 b = aV1 - aV3;
        Vector3 c = aP - aV3;
        float aLen = a.x * a.x + a.y * a.y + a.z * a.z;
        float bLen = b.x * b.x + b.y * b.y + b.z * b.z;
        float ab = a.x * b.x + a.y * b.y + a.z * b.z;
        float ac = a.x * c.x + a.y * c.y + a.z * c.z;
        float bc = b.x * c.x + b.y * c.y + b.z * c.z;
        float d = aLen * bLen - ab * ab;
        u = (aLen * bc - ab * ac) / d;
        v = (bLen * ac - ab * bc) / d;
        w = 1.0f - u - v;
    }


}




