using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using UnityEngine;
using UnityEngine.SceneManagement;

/*
[Script Header] TDS_NavMeshManager Version 0.0.1
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
public class TDS_NavMeshManager : MonoBehaviour
{
    #region Fields and properties
    public static TDS_NavMeshManager Instance;

    [SerializeField] List<TDS_Triangle> triangles = new List<TDS_Triangle>();
    public List<TDS_Triangle> Triangles { get { return triangles; }  }

    private Dictionary<int, TDS_NavPoint> navPoints = new Dictionary<int, TDS_NavPoint>();
    public Dictionary<int, TDS_NavPoint> NavPoints { get { return navPoints; } }

    private bool isCalculating = false; 
    public bool IsCalculating { get { return IsCalculating; } } 
    #endregion

    #region Methods
    #region bool
    /// <summary>
    /// Return if the position is inside of the triangle
    /// </summary>
    /// <param name="_position"></param>
    /// <returns></returns>
    bool IsInTriangle(Vector3 _position, TDS_Triangle _triangle)
    {
        Barycentric _barycentric = new Barycentric(navPoints[_triangle.VerticesIndex[0]].Position, navPoints[_triangle.VerticesIndex[1]].Position, navPoints[_triangle.VerticesIndex[2]].Position, _position);
        return _barycentric.IsInside;
    }

    /// <summary>
    /// Calculate path from an origin to a destination 
    /// Set the path when it can be calculated 
    /// </summary>
    /// <param name="_origin">The Origin of the path </param>
    /// <param name="_destination">The Destination of the path</param>
    /// <param name="_path">The path to set</param>
    /// <returns>Return if the path can be calculated</returns>
    public bool CalculatePath(Vector3 _origin, Vector3 _destination, TDS_CustomNavPath _path)
    {
        isCalculating = true; 
        // GET TRIANGLES
        // Get the origin triangle and the destination triangle
        TDS_Triangle _currentTriangle = GetTriangleContainingPosition(_origin);
        TDS_Triangle _targetedTriangle = GetTriangleContainingPosition(_destination);

        // CREATE POINTS
        TDS_NavPoint _currentPoint = null;
        //Create a point on the origin position
        TDS_NavPoint _originPoint = new TDS_NavPoint(_origin);
        //Get the linked triangles for the origin point
        _originPoint.LinkedTriangles.Add(_currentTriangle);
        //Create a point on the destination position
        TDS_NavPoint _destinationPoint = new TDS_NavPoint(_destination);

        //ARRAY 
        TDS_NavPoint[] _linkedPoints = null;

        //LISTS AND DICO
        List<TDS_NavPoint> _openList = new List<TDS_NavPoint>();
        Dictionary<TDS_NavPoint, TDS_NavPoint> _cameFrom = new Dictionary<TDS_NavPoint, TDS_NavPoint>();

        /* ASTAR: Algorithm*/
        // Add the origin point to the open and close List
        //Set its heuristic cost and its selection state
        _openList.Add(_originPoint);
        _originPoint.HeuristicCostFromStart = 0; 
        _originPoint.HasBeenSelected = true;
        _cameFrom.Add(_originPoint, _originPoint); 
        while (_openList.Count > 0)
        {
            //Get the point with the best heuristic cost
            _currentPoint = GetBestPoint(_openList);
            //If this point is in the targeted triangle, 
            if (GetTrianglesFromPoint(_currentPoint).Contains(_targetedTriangle))
            {
                //add the destination point to the close list and set the previous point to the current point 
                _cameFrom.Add(_destinationPoint, _currentPoint);
                //Build the path
                _path.BuildPath(_cameFrom);
                //Clear all points selection state
                foreach (KeyValuePair<int, TDS_NavPoint> pair in navPoints)
                {
                    pair.Value.HasBeenSelected = false; 
                }
                return true;
            }
            //Get all linked points from the current point
            _linkedPoints = GetLinkedPoints(_currentPoint);
            for (int i = 0; i < _linkedPoints.Length; i++)
            {
                TDS_NavPoint _linkedPoint = _linkedPoints[i];
                TDS_NavPoint _parentPoint; 
                // If the linked points is not selected yet
                if (!_linkedPoint.HasBeenSelected)
                {
                    // Calculate the heuristic cost from start of the linked point
                    float _cost = _currentPoint.HeuristicCostFromStart + HeuristicCost(_currentPoint, _linkedPoint);
                    _linkedPoint.HeuristicCostFromStart = _cost; 
                    if (!_openList.Contains(_linkedPoint) || _cost < _linkedPoint.HeuristicCostFromStart)
                    {
                        if(IsInLineOfSight(_cameFrom[_currentPoint], _linkedPoint))
                        {
                            _cost = HeuristicCost(_cameFrom[_currentPoint], _linkedPoint);
                            _parentPoint = _cameFrom[_currentPoint]; 
                        }
                        else
                        {
                            _parentPoint = _currentPoint; 
                        }
                        // Set the heuristic cost from start for the linked point
                        _linkedPoint.HeuristicCostFromStart = _cost;
                        //Its heuristic cost is equal to its cost from start plus the heuristic cost between the point and the destination
                        _linkedPoint.HeuristicPriority = HeuristicCost(_linkedPoint, _destinationPoint) + _cost;
                        //Set the point selected and add it to the open and closed list
                        _linkedPoint.HasBeenSelected = true;
                        _openList.Add(_linkedPoint);
                        _cameFrom.Add(_linkedPoint, _parentPoint);
                    }
                }
            }
        }
        
        return false;
    }

    /// <summary>
    /// Check the cost 
    /// GEt if there is a triangle in the direction from the next point to the parent point
    /// </summary>
    /// <param name="_previousPoint">Parent Point</param>
    /// <param name="_nextPoint">Linked Point</param>
    /// <returns>if the link between the previous and the next point can be set</returns>
    bool IsInLineOfSight(TDS_NavPoint _previousPoint, TDS_NavPoint _nextPoint)
    {
        //A AMELIORER
        //AUGMENTER LA PRECISION DU CHECK DES TRIANGLES
        float _cost = _previousPoint.HeuristicCostFromStart + HeuristicCost(_previousPoint, _nextPoint);
        if (_cost > _nextPoint.HeuristicCostFromStart) return false;
        Vector3 _dir = (_nextPoint.Position - _previousPoint.Position).normalized;
        _dir.y = 0;
        return AnyTriangleContainingPointExists(_nextPoint.Position + _dir); 
    }

    /// <summary>
    /// Get if the position is contained in any triangle
    /// </summary>
    /// <param name="_position">position</param>
    /// <returns>Any triangle containing the position</returns>
    bool AnyTriangleContainingPointExists(Vector3 _position)
    {
        return triangles.Any(t => IsInTriangle(_position, t)); 
    }
    #endregion

    #region float 
    /// <summary>
    /// Return the heuristic cost between 2 points
    /// Heuristic cost is the distance between 2 points
    /// => Can add a multiplier to change the cost of the movement depending on the point 
    /// </summary>
    /// <param name="_a">First Point</param>
    /// <param name="_b">Second Point</param>
    /// <returns>Heuristic Cost between 2 points</returns>
    float HeuristicCost(TDS_NavPoint _a, TDS_NavPoint _b)
    {
        return Vector3.Distance(_a.Position, _b.Position); 
    }
    #endregion 

    #region Triangle
    /// <summary>
    /// Get the triangle where the position is contained 
    /// Position is right under the selected Position
    /// If triangle can't be found, get the closest triangle
    /// </summary>
    /// <param name="_position">Position</param>
    /// <returns>Triangle where the position is contained</returns>
    public TDS_Triangle GetTriangleContainingPosition(Vector3 _position)
    {
        RaycastHit _hit;
        if (Physics.Raycast(_position, Vector3.down, out _hit, 5))
        {
            Vector3 _onGroundPosition = _hit.point;
            foreach (TDS_Triangle triangle in triangles)
            {
                if (IsInTriangle(_onGroundPosition, triangle))
                {
                    return triangle;
                }
            }
        }
        return triangles.OrderBy(t => Vector3.Distance(t.CenterPosition, _position)).FirstOrDefault(); 
    }

    /// <summary>
    /// Get all triangles that contains the point P
    /// </summary>
    /// <param name="_point">Point P</param>
    /// <returns>List of all linked triangles of the point P</returns>
    TDS_Triangle[] GetTrianglesFromPoint(TDS_NavPoint _point)
    {
        List<TDS_Triangle> _containingTriangles = new List<TDS_Triangle>();
        foreach (TDS_Triangle triangle in triangles)
        {
            if (IsInTriangle(_point.Position, triangle))
            {
                _containingTriangles.Add(triangle);
            }
        }
        return _containingTriangles.ToArray();
    }
    #endregion
    #region NavPoints
    /// <summary>
    /// Get all linked points from a selected point
    /// </summary>
    /// <param name="_point">Point</param>
    /// <returns>Array containing the linked points of the selected point</returns>
    TDS_NavPoint[] GetLinkedPoints(TDS_NavPoint _point)
    {
        List<int> _indexes = new List<int>();
        //For each triangle Linked
        for (int i = 0; i < _point.LinkedTriangles.Count; i++)
        {
            //Get vertices indexes
            _indexes.AddRange(_point.LinkedTriangles[i].VerticesIndex);
            //Get Middle segment indexes 
            _indexes.AddRange(_point.LinkedTriangles[i].MiddleSegmentIndex);
        }
        TDS_NavPoint[] _points = new TDS_NavPoint[_indexes.Count];
        for (int i = 0; i < _indexes.Count; i++)
        {
            //Get the points with the indexes
            _points[i] = navPoints[_indexes[i]]; 
        }
        return _points; 
    }

    /// <summary>
    /// Get the point with the best heuristic cost from a list 
    /// Remove this point from the list and return it
    /// </summary>
    /// <param name="_points">list where the points are</param>
    /// <returns>point with the best heuristic cost</returns>
    TDS_NavPoint GetBestPoint(List<TDS_NavPoint> _points)
    {
        int bestIndex = 0;
        for (int i = 0; i < _points.Count; i++)
        {
            if (_points[i].HeuristicPriority < _points[bestIndex].HeuristicPriority)
            {
                bestIndex = i;
            }
        }

        TDS_NavPoint _bestNavPoint = _points[bestIndex];
        _points.RemoveAt(bestIndex);
        return _bestNavPoint;
    }
    #endregion

    #region void
    /// <summary>
    /// Get the datas from the dataPath folder to get the navpoints and the triangles
    /// </summary>
    void LoadDatas()
    {
        TDS_NavDataSaver<CustomNavData> _loader = new TDS_NavDataSaver<CustomNavData>();
        string _sceneName = SceneManager.GetActiveScene().name;
        string _directoryName = Path.Combine(Application.dataPath, "TDS_NavDatas");
        CustomNavData _datas = _loader.LoadFile(_directoryName, _sceneName);
        navPoints = _datas.NavPointsInfo;
        triangles = _datas.TrianglesInfos;
    }
    #endregion

    #endregion
    #region Unity Methods
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this); 
        }
    }

    void Start ()
    {
        LoadDatas(); 
    }

    private void OnDrawGizmos()
    {
        if (triangles.Count == 0 || navPoints.Count == 0) return;
        Gizmos.color = Color.green;
        foreach (KeyValuePair<int, TDS_NavPoint> pair in navPoints)
        {
            int[] _linkedIndexes = pair.Value.GetAllNeighborsIndexes();
            for (int i = 0; i < _linkedIndexes.Length; i++)
            {
                Gizmos.DrawLine(pair.Value.Position, navPoints[_linkedIndexes[i]].Position);
            }
        }
        for (int i = 0; i < triangles.Count; i++)
        {
            TDS_Triangle _t = triangles[i];
            for (int j = 0; j < 3; j++)
            {
                if (!navPoints.ContainsKey(_t.VerticesIndex[j])) continue;
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
    }
    #endregion

}