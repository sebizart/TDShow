using System; 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon;
using Random = System.Random; 
/*
[Script Header] TDS_FightingArea Version 0.0.1
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

[RequireComponent(typeof(BoxCollider))]
public class TDS_FightingArea : PunBehaviour
{
    #region events
    public Action OnNextWave; 
    #endregion

    #region Fields and Properties
    private SpawnPointState detectionState = SpawnPointState.Disable; 
    public SpawnPointState DetectionState { get { return detectionState; } }

    [SerializeField, Range(1, 10)] int remainingWaves=1;
    public int RemainingWaves { get { return remainingWaves; } }

    private bool isFirstWave = true;

    [SerializeField] TDS_DetectionArea detectionArea = new TDS_DetectionArea();
    public TDS_DetectionArea DetectionArea { get { return detectionArea; } }
    [SerializeField] List<TDS_SpawnPoint> spawnPoints = new List<TDS_SpawnPoint>();
    public List<TDS_SpawnPoint> SpawnPoints { get { return spawnPoints; } }
    [SerializeField] List<TDS_Enemy> spawnedEnemies = new List<TDS_Enemy>();
    public List<TDS_Enemy> SpawnedEnemies { get { return spawnedEnemies; } }
    [SerializeField] PhotonView areaPhotonView; 
    #endregion

    #region UnityMethods
    void Start ()
    {
        OnNextWave += Spawn;
	}
	void Update ()
    {
	}
    private void OnDrawGizmos()
    {
        Gizmos.color = DetectionArea.DebugColor;
        Gizmos.DrawWireCube(DetectionArea.CenterPosition, DetectionArea.ExtendingPosition);
        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            Gizmos.color = SpawnPoints[i].SpawnPointColor;
            Gizmos.DrawSphere(SpawnPoints[i].SpawnPosition, .1f); 
        }
    }
    private void OnTriggerEnter(Collider _collider)
    {
        if (detectionState != SpawnPointState.Disable) return; 
        if (_collider.GetComponent<TDS_Player>())
        {
            detectionState = SpawnPointState.Enable;
            isFirstWave = true; 
            OnNextWave?.Invoke();
        }

    }
    #endregion

    #region Methods
    void Spawn()
    {
        if (!PhotonNetwork.isMasterClient) return;
        Debug.Log("OKAY"); 
        TDS_SpawnPoint[] _spawnablePoints = GetSpawnPoints();
        //TROUVER LE NOMBRE D'ENEMIES A FAIRE SPAWN A CHAQUE WAVE -> DANS UN GAME MANAGER qui calcule les dégats qu'ont infligé les joueurs?
        // Pour le moment, on en fera spawn autant qu'il y a de points
    }

    private TDS_SpawnPoint[] GetSpawnPoints()
    {
        if (isFirstWave)
        {
            return SpawnPoints.ToArray(); 
        }
            return SpawnPoints.FindAll(p => !p.IsOnFightingArea).ToArray(); ; 
    }

    #region Networking
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (!PhotonNetwork.isMasterClient) return; 
        //Migrate spawn point datas  + enemies datas
    }
    #endregion


    #endregion

    #region EditorMethods
    public void AddSpawnPoint(int _i) => SpawnPoints.Add(new TDS_SpawnPoint(_i));
    public void RemovePointAt(int _i) => SpawnPoints.RemoveAt(_i);  
    #endregion
}

public enum SpawnPointState
{
    Disable, 
    Enable, 
    Completed
}


//
//
//
//
//
//
//

//
//
//
//
//
//


//


//
//

//
//
//
//

//
//
//
//
//
//
//
//
//
//
//
//
//
//

