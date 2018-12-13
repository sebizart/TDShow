using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

[Serializable]
public class TDS_CustomNavPath
{
    [SerializeField] List<TDS_NavPoint> navigationPoints = new List<TDS_NavPoint>(); 
    public List<TDS_NavPoint> NavigationPoints { get {return navigationPoints; }}

    [SerializeField] List<TDS_NavPoint> simplifiedPath = new List<TDS_NavPoint>();
    public List<TDS_NavPoint> SimplifiedPath { get { return simplifiedPath; } }

    /// <summary>
    /// Build a path using Astar resources
    /// Get the last point and get all its parent to build the path
    /// </summary>
    /// <param name="_pathToBuild">Astar resources</param>
    public void BuildPath(Dictionary<TDS_NavPoint, TDS_NavPoint> _pathToBuild)
    {
        TDS_NavPoint _currentPoint = _pathToBuild.Last().Key;
        while (_currentPoint != _pathToBuild.First().Key)
        {
            navigationPoints.Add(_currentPoint);
            _currentPoint = _pathToBuild[_currentPoint]; 
        }
        navigationPoints.Add(_currentPoint);
        navigationPoints.Reverse();
    }

    /// <summary>
    /// Clear the path datas
    /// </summary>
    public void ClearPath()
    {
        navigationPoints.Clear(); 
    }
   
}
