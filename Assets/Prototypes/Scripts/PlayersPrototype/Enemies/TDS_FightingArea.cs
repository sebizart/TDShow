﻿using System; 
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

[RequireComponent(typeof(BoxCollider))][RequireComponent(typeof(PhotonView))]
public class TDS_FightingArea : PunBehaviour
{
    #region events
    public Action OnNextWave; 
    #endregion

    #region Fields and Properties
    [SerializeField]SpawnPointState detectionState = SpawnPointState.Disable; 
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
    public PhotonView AreaPhotonView { get { return areaPhotonView; } }

    #endregion

        #region UnityMethods
    private void Awake()
    {
        OnNextWave += Spawn;
    }
    void Start ()
    {
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
            Gizmos.DrawSphere(SpawnPoints[i].SpawnPosition, .5f); 
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
    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 200, 50, 200, 50), spawnedEnemies.Count.ToString()); 
    }
    #endregion

    #region Methods


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private TDS_SpawnPoint[] GetSpawnPoints()
    {
        if (isFirstWave)
        {
            return SpawnPoints.ToArray(); 
        }
        return SpawnPoints.FindAll(p => !p.IsOnFightingArea).ToArray(); ; 
    }
    
    /// <summary>
    /// 
    /// </summary>
    void Spawn()
    {
        if (!PhotonNetwork.isMasterClient) return;
        TDS_SpawnPoint[] _spawnablePoints = GetSpawnPoints();
        //TROUVER LE NOMBRE D'ENEMIES A FAIRE SPAWN A CHAQUE WAVE -> DANS UN GAME MANAGER qui calcule les dégats qu'ont infligé les joueurs?
        // Pour le moment, on en fera spawn autant qu'il y a de points
        for (int i = 0; i < _spawnablePoints.Length; i++)
        {
            TDS_Enemy _e = _spawnablePoints[i].GetEnemy();
            TDS_Enemy _enemy = PhotonNetwork.Instantiate(_e.PrefabName.ToString(), _spawnablePoints[i].SpawnPosition + Vector3.up, Quaternion.identity, 0).GetComponent<TDS_Enemy>();
            spawnedEnemies.Add(_enemy); 
        }
    }

    /// <summary>
    /// SpawnEnemies using Enemies Info 
    /// Spawn at a position, with a state, etc...
    /// </summary>
    /// <param name="_enemiesInfo"></param>
    public void SpawnEnemiesUsingInfos(List<TDS_EnemyInfo> _enemiesInfo)
    {
        foreach (TDS_EnemyInfo _info in _enemiesInfo)
        {
            TDS_Enemy _enemy = PhotonNetwork.Instantiate(((EnemyName)_info.EnemyType).ToString(), _info.EnemyPosition, Quaternion.identity, 0).GetComponent<TDS_Enemy>();
            spawnedEnemies.Add(_enemy);
        }
    }
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

#region Fighting Area Info

public class TDS_FightingAreaInfo
{
    public int FightingAreaID;
    // Add Complementary informations 

    public List<TDS_EnemyInfo> EnemiesInfos = new List<TDS_EnemyInfo>();

    public TDS_FightingAreaInfo(string _info)
    {
        FightingAreaID = int.Parse(_info.Split('|')[0]);
        for (int i = 1; i < _info.Split('|').Length; i++)
        {
            EnemiesInfos.Add(new TDS_EnemyInfo(_info.Split('|')[i]));
        }
    }
}

/// <summary>
/// string to convert into infomations: EnemyType#PosX#PosY#PosZ
/// </summary>
public class TDS_EnemyInfo
{
    public int EnemyId;
    public Vector3 EnemyPosition;
    public int EnemyType;
    //Add all transfered informations (animation state, life, etc...)

    public TDS_EnemyInfo(string _info)
    {
        EnemyType = int.Parse(_info.Split('#')[0]);
        float _xPos = float.Parse(_info.Split('#')[1]);
        float _yPos = float.Parse(_info.Split('#')[2]);
        float _zPos = float.Parse(_info.Split('#')[3]);
        EnemyPosition = new Vector3(_xPos, _yPos, _zPos);
    }

}
#endregion


