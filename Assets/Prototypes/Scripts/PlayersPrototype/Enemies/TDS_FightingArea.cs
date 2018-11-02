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

[RequireComponent(typeof(BoxCollider))][RequireComponent(typeof(PhotonView))]
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
        TDS_SpawnPoint[] _spawnablePoints = GetSpawnPoints();
        //TROUVER LE NOMBRE D'ENEMIES A FAIRE SPAWN A CHAQUE WAVE -> DANS UN GAME MANAGER qui calcule les dégats qu'ont infligé les joueurs?
        // Pour le moment, on en fera spawn autant qu'il y a de points
        for (int i = 0; i < _spawnablePoints.Length; i++)
        {
            spawnedEnemies.Add(_spawnablePoints[i].SpawnEnemy()); 
        }
    }

    private TDS_SpawnPoint[] GetSpawnPoints()
    {
        if (isFirstWave)
        {
            return SpawnPoints.ToArray(); 
        }
            return SpawnPoints.FindAll(p => !p.IsOnFightingArea).ToArray(); ; 
    }

    private string SetFightingAreaInfos(int _areaID, TDS_Enemy[] _enemiesID)
    {
        string _info = $"{_areaID}";
        foreach (TDS_Enemy _enemy in _enemiesID.ToList())
        {
            _info += $"|{_enemy.PhotonViewElementID}#{_enemy.transform.position.x.ToString("0.0")}#{_enemy.transform.position.y.ToString("0.0")}#{_enemy.transform.position.z.ToString("0.0")}#{(int)_enemy.PrefabName}";
        }
        return _info;
    }

    /// <summary>
    /// Change the owner of the photonView #TRY#
    /// </summary>
    private void ChangeOwnerShip()
    {
        areaPhotonView.TransferOwnership(PhotonNetwork.masterClient.ID); 
    }
    
    /// <summary>
    /// Migrate informations while using RPC Requests #TRY#
    /// </summary>
    private void MigrateInformations()
    {
        //Migrate Area datas  + enemies datas
        string _info = SetFightingAreaInfos(areaPhotonView.viewID, spawnedEnemies.ToArray());
        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("ApplyAreaInformations", PhotonTargets.MasterClient, _info);
    }

    #region PUN Networking
    //FAIRE QUITTER LA ROOM AVANT DE TOUT FERMER
    public override void OnLeftRoom()
    {
        if (!PhotonNetwork.isMasterClient) return;
        //Set a new Master if there is more than one player
        if (PhotonNetwork.otherPlayers.Length == 0) return;
        PhotonNetwork.SetMasterClient(PhotonNetwork.otherPlayers[0]);
        Debug.Log("NEW MASTER SET"); 
        //MigrateInformations();
        //OR
        ChangeOwnerShip(); 
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
/// string to convert into infomations: EnemyId#PosX#PosY#PosZ#EnemyType
/// </summary>
public class TDS_EnemyInfo
{
    public int EnemyId;
    public Vector3 EnemyPosition;
    public int EnemyType;
    //Add all transfered informations (animation state, life, etc...)

    public TDS_EnemyInfo(string _info)
    {
        EnemyId = int.Parse(_info.Split('#')[0]);
        float _xPos = int.Parse(_info.Split('#')[1]);
        float _yPos = int.Parse(_info.Split('#')[2]);
        float _zPos = int.Parse(_info.Split('#')[3]);
        EnemyPosition = new Vector3(_xPos, _yPos, _zPos);
        EnemyType = int.Parse(_info.Split('#')[4]);

    }

}
#endregion


