using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics; 
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement; 
using Random = UnityEngine.Random;

/*
[Script Header] TDS_CustomNavMesh Version 0.0.1
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

public class TDS_NavMeshBuilder : MonoBehaviour 
{

    #region Fields/Properties

    public bool ShowVertices = false;
    public bool ShowMiddle = false;
    public bool ShowSegments = false;
    public bool ShowLinks = false;

    [SerializeField] List<TDS_Triangle> triangles = new List<TDS_Triangle>();

    [SerializeField] Dictionary<int, TDS_NavPoint> navPoints = new Dictionary<int, TDS_NavPoint>();
    public Dictionary<int, TDS_NavPoint> NavPoints { get { return navPoints;  } }

    public string SavingDirectory { get { return Application.dataPath + "/Resources/TDS_NavDatas";  } }
    #endregion

    #region Methods

    #region List<TDS_NavPoints>
    /// <summary>
    /// Compare triangles
    /// if the triangles have more than 1 vertices in common return true
    /// </summary>
    /// <param name="_triangle1">First triangle to compare</param>
    /// <param name="_triangle2">Second triangle to compare</param>
    /// <returns>If the triangles have more than 1 vertex.ices in common</returns>
    int[] GetVerticesInCommon(TDS_Triangle _triangle1, TDS_Triangle _triangle2)
    {
        List<int> _verticesIndex = new List<int>();
        for (int i = 0; i < _triangle1.VerticesIndex.Length; i++)
        {
            for (int j = 0; j < _triangle2.VerticesIndex.Length; j++)
            {
                if (_triangle1.VerticesIndex[i] == _triangle2.VerticesIndex[j])
                    _verticesIndex.Add(_triangle1.VerticesIndex[i]);
            }
        }
        return _verticesIndex.ToArray();
    }
    #endregion

    #region void
    public void ClearNavDatas()
    {
        navPoints.Clear(); 
        triangles.Clear();
    }

    /// <summary>
    /// CENTER IS EQUAL TO THE ARITHMETIQUE MEDIUM OF THE 3 POINTS 
    /// CREATE CENTER POINT OF A TRIANGLE AND ADD IT TO THE NAV POINTS DICO
    /// </summary>
    void GetCenterPosition(TDS_Triangle _triangle)
    {
        int[] _indexPositions = new int[3] { _triangle.VerticesIndex[0], _triangle.VerticesIndex[1], _triangle.VerticesIndex[2] };
        TDS_NavPoint[] _points = new TDS_NavPoint[3] { navPoints[_indexPositions[0]], navPoints[_indexPositions[1]], navPoints[_indexPositions[2]] };
        Vector3 _pos = (_points[0].Position + _points[1].Position + _points[2].Position) / 3;
        _triangle.CenterPosition = _pos;
    }

    /// <summary>
    /// Get all nav mesh surfaces
    /// Update all navmesh datas for each NavMeshSurfaces
    /// Calculate triangulation 
    /// Add each triangle in the triangulation to a list of triangles
    /// Link Triangles with its neighbors
    /// </summary>
    public void GetNavPointsFromNavSurfaces()
    {
        triangles.Clear();
        navPoints.Clear();
        List<NavMeshSurface> _surfaces = NavMeshSurface.activeSurfaces;
        foreach (NavMeshSurface surface in _surfaces)
        {
            surface.BuildNavMesh();
        }
        NavMeshTriangulation _tr = NavMesh.CalculateTriangulation();
        Vector3[] _vertices = _tr.vertices;
        List<int> _modifiedIndices = _tr.indices.ToList();
        //GET ALL NAV POINTS
        int _previousIndex = 0;
        for (int i = 0; i < _vertices.Length; i++)
        {
            ////CREATE A POINT AT POSITION
            Vector3 _pos = _vertices[i];
            // CHECK IF NAV POINT ALREADY EXISTS
            if (navPoints.Any(p => p.Value.Position == _pos))
            {
                ////GET THE SAME POINT
                KeyValuePair<int, TDS_NavPoint> _existingPoint = navPoints.Select(n => n).Where(p => p.Value.Position == _pos).First();
                //// ORDER THE INDEX 
                for (int j = 0; j < _modifiedIndices.Count; j++)
                {
                    // REPLACE INDEX
                    if (_modifiedIndices[j] == _previousIndex)
                    {
                        _modifiedIndices[j] = _existingPoint.Key;
                    }
                    // IF INDEX IS GREATER THAN THE REPLACED INDEX REMOVE ONE FROM THEM
                    if (_modifiedIndices[j] > _previousIndex)
                    {
                        _modifiedIndices[j]--;
                    }
                }
            }
            // IF THE NAV POINT DOESN'T EXISTS ADD IT TO THE DICO
            else
            {
                navPoints.Add(navPoints.Count, new TDS_NavPoint(_pos));
                _previousIndex++;
            }
        }
        //GET ALL TRIANGLES
        for (int i = 0; i < _modifiedIndices.Count; i += 3)
        {
            int[] _pointsIndex = new int[3] { _modifiedIndices[i], _modifiedIndices[i + 1], _modifiedIndices[i + 2] };
            TDS_Triangle _triangle = new TDS_Triangle(_pointsIndex);
            triangles.Add(_triangle);
            GetCenterPosition(_triangle);
        }
        for (int i = 0; i < triangles.Count; i++)
        {
            TDS_Triangle _t = triangles[i];
            LinkTriangles(_t);
        }
    }

    /// <summary>
    /// For the triangle, link each point to the other points of the triangle
    /// </summary>
    /// <param name="_triangle">triangle that contains the points</param>
    public void LinkPoints(TDS_Triangle _triangle)
    {
        List<int> _allIndexes = new List<int>();
        _allIndexes.AddRange(_triangle.VerticesIndex);
        _allIndexes.AddRange(_triangle.MiddleSegmentIndex);
        for (int i = 0; i < _allIndexes.Count; i++)
        {
            TDS_NavPoint _point = navPoints[_allIndexes[i]];
            _point.LinkedTriangles.Add(_triangle); 
        }
    }

    /// <summary>
    /// Get all triangles' neighbors and add them to the list of Neighbors
    /// </summary>
    void LinkTriangles(TDS_Triangle _triangle)
    {
        //COMPARE CURRENT TRIANGLE WITH EVERY OTHER TRIANGLE
        for (int i = 0; i < triangles.Count; i++)
        {
            // IF TRIANGLES ARE THE SAME, DON'T COMPARE THEM
            if (triangles[i] != _triangle && !triangles[i].HasBeenLinked)
            {
                // GET THE VERTICES IN COMMON
                int[] _verticesInCommon = GetVerticesInCommon(_triangle, triangles[i]);
                // CHECK IF THERE IS THE RIGHT AMMOUNT OF VERTICES IN COMMON
                if (_verticesInCommon.Length > 0)
                {
                    _triangle.LinkedTriangles.Add(triangles[i]);
                }
                // IF THERE IS 2 TWO VERTICES IN COMMON, THE 2 TRIANGLES HAD 1 SIDE IN COMMON
                if (_verticesInCommon.Length == 2)
                {
                    Vector3 _pos = (navPoints[_verticesInCommon[0]].Position + navPoints[_verticesInCommon[1]].Position) / 2;
                    TDS_NavPoint _middlePoint = new TDS_NavPoint(_pos);
                    navPoints.Add(navPoints.Count, _middlePoint);
                    _triangle.MiddleSegmentIndex.Add(navPoints.Count - 1);
                    triangles[i].MiddleSegmentIndex.Add(navPoints.Count - 1);
                }
            }
        }
        LinkPoints(_triangle); 
        _triangle.HasBeenLinked = true;
    }

    /// <summary>
    /// Open the resources folder where the datas are saved
    /// </summary>
    public void OpenDirectory()
    {
        if (!Directory.Exists(SavingDirectory)) Directory.CreateDirectory(SavingDirectory);
        Process.Start(SavingDirectory);
    }

    /// <summary>
    /// Get the nav points and save them in binary in a selected directory
    /// 
    /// Sauvegarder les datas dans le dossier resources
    /// Au démarrage du jeu, si les datas ne sont pas dans le data path, on va les chercher dans le dosser resources pour les mettre dans le pdpath
    /// </summary>
    public void SaveDatas()
    {
        if (!Directory.Exists(SavingDirectory)) Directory.CreateDirectory(SavingDirectory);
        TDS_NavDataSaver<CustomNavData> _navDataSaver = new TDS_NavDataSaver<CustomNavData>();
        CustomNavData _dataSaved = new CustomNavData();
        _dataSaved.NavPointsInfo = navPoints;
        _dataSaved.TrianglesInfos = triangles;
        _navDataSaver.SaveFile(SavingDirectory, SceneManager.GetActiveScene().name, _dataSaved, ".txt");
    }
    
    #endregion
    #endregion

    #region UnityMethods
    private void OnDrawGizmos()
    {
        if (triangles.Count == 0) return;
        if (ShowLinks)
        {
            Gizmos.color = Color.green;
            foreach (KeyValuePair<int, TDS_NavPoint> pair in navPoints)
            {
                int[] _linkedIndexes = pair.Value.GetAllNeighborsIndexes();
                for (int i = 0; i < _linkedIndexes.Length; i++)
                {
                    Gizmos.DrawLine(pair.Value.Position, navPoints[_linkedIndexes[i]].Position);
                }
            }
        }
        for (int i = 0; i < triangles.Count; i++)
        {
            TDS_Triangle _t = triangles[i]; 
            for (int j = 0; j < 3; j++)
            {
                if (!navPoints.ContainsKey(_t.VerticesIndex[j])) continue;
                if (ShowVertices)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(navPoints[_t.VerticesIndex[j]].Position, .1f);
                }
                if (ShowSegments)
                {
                    Gizmos.color = Color.blue;
                    if (j < 2)
                    {
                        Gizmos.DrawLine(navPoints[_t.VerticesIndex[j]].Position, navPoints[_t.VerticesIndex[j + 1]].Position);
                    }
                    else
                    {
                        Gizmos.DrawLine(navPoints[_t.VerticesIndex[j]].Position, navPoints[_t.VerticesIndex[0]].Position);
                    }
                }
            }
            if (ShowMiddle)
            {
                Gizmos.color = Color.cyan;
                for (int j = 0; j < _t.MiddleSegmentIndex.Count; j++)
                {
                    if(navPoints.ContainsKey(_t.MiddleSegmentIndex[j]))
                        Gizmos.DrawSphere(navPoints[_t.MiddleSegmentIndex[j]].Position, .1f);
                }
            }         
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_t.CenterPosition, .1f);
        }
    }
    #endregion
}

[Serializable]
public struct CustomNavData
{
    public Dictionary<int, TDS_NavPoint> NavPointsInfo;
    public List<TDS_Triangle> TrianglesInfos; 
}