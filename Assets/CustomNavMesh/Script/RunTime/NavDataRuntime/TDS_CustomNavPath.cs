using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

[Serializable]
public class TDS_CustomNavPath
{
    [SerializeField] Vector3 startPosition; 
    public Vector3 StartPosition { get { return startPosition; } }

    [SerializeField] Vector3 endPosition; 
    public Vector3 EndPosition { get { return endPosition; } }

    [SerializeField] List<TDS_NavPoint> navigationPoints = new List<TDS_NavPoint>(); 
    public List<TDS_NavPoint> NavigationPoints
    {
        get
        {
            return navigationPoints;
        }
        set
        {
            navigationPoints = value;
        }
    }

    public void BuildPath(Dictionary<TDS_NavPoint, TDS_NavPoint> _pathToBuild)
    {
        startPosition = _pathToBuild.First().Key.Position;
        endPosition = _pathToBuild.Last().Value.Position;
        TDS_NavPoint _currentPoint = _pathToBuild.Last().Key;
        while (_currentPoint != _pathToBuild.First().Key)
        {
            navigationPoints.Add(_currentPoint);
            _currentPoint = _pathToBuild[_currentPoint]; 
        }
        navigationPoints.Add(_currentPoint);
        navigationPoints.Reverse(); 
    }

}
