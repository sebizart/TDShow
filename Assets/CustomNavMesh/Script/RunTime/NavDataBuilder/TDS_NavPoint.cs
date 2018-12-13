using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] TDS_NavPoint Version 0.0.1
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
public class TDS_NavPoint
{
    #region Fields and Properties
    public Vector3 Position
    {
        get
        {
            return new Vector3(xPos, yPos, zPos);
        }
    }

    [SerializeField] float xPos, yPos, zPos; 

    public bool HasBeenSelected { get; set; }

    [SerializeField] List<TDS_Triangle> linkedTriangles = new List<TDS_Triangle>(); 
    public List<TDS_Triangle> LinkedTriangles { get { return linkedTriangles;  } }

    private float heuristicPriority; 
    public float HeuristicPriority
    {
        get
        {
            return heuristicPriority = HeuristicCostFromStart + HeuristicCostToDestination;
        }
        set
        {
            heuristicPriority = value;
            HeuristicCostToDestination = heuristicPriority - HeuristicCostFromStart; 
        }
    } 

    public float HeuristicCostFromStart { get; set; }
    public float HeuristicCostToDestination { get; set; }
    #endregion

    #region Constructor
    public TDS_NavPoint(Vector3 _pos)
    {
        xPos = _pos.x;
        yPos = _pos.y;
        zPos = _pos.z; 
    }
    #endregion

    #region Methods
    public int[] GetAllNeighborsIndexes()
    {
        List<int> _indexes = new List<int>();
        for (int i = 0; i < linkedTriangles.Count; i++)
        {
            TDS_Triangle _t = linkedTriangles[i];
            _indexes.AddRange(_t.VerticesIndex);
            _indexes.AddRange(_t.MiddleSegmentIndex); 
        }
        return _indexes.ToArray(); 
    }
    #endregion
}