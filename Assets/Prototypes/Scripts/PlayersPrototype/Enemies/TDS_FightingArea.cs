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
    [SerializeField]SpawnPointState detectionState = SpawnPointState.Disable; 
    public SpawnPointState DetectionState { get { return detectionState; } set { detectionState = value; } }

    [SerializeField] int waveNumber = 0; 

    private bool isFirstWave = true;
    [SerializeField] bool isLooping = false; 
    public bool IsLooping { get { return isLooping; } set { isLooping = value; } }

    [SerializeField] TDS_DetectionArea detectionArea = new TDS_DetectionArea();
    public TDS_DetectionArea DetectionArea { get { return detectionArea; } }

    [SerializeField] TDS_PointsManager pointsManager; 
    public TDS_PointsManager PointsManager { get { return pointsManager; } }

    [SerializeField] List<TDS_Enemy> spawnedEnemies = new List<TDS_Enemy>();
    public List<TDS_Enemy> SpawnedEnemies { get { return spawnedEnemies; } }

    List<TDS_Enemy> destroyingEnemies = new List<TDS_Enemy>(); 

    [SerializeField] PhotonView areaPhotonView;
    public PhotonView AreaPhotonView { get { return areaPhotonView; } }

    [SerializeField] Canvas enemyCanvas;
    public Canvas EnemyCanvas { get { return enemyCanvas; } set { enemyCanvas = value; } }
    [SerializeField] string lifeBarName;
    public string LifeBarName { get { return lifeBarName; } set { lifeBarName = value; } }
    #endregion

    #region UnityMethods
    private void Awake()
    {
        OnNextWave += Spawn;
    }
    void Start ()
    {
        if (areaPhotonView == null)
        {
            areaPhotonView = GetComponent<PhotonView>();
        }
        if(!PhotonNetwork.isMasterClient)
        {
            StartCoroutine("GetEnemiesInformations");
        }
	}
    private void OnDrawGizmos()
    {
        Gizmos.color = DetectionArea.DebugColor;
        Gizmos.DrawWireCube(DetectionArea.CenterPosition, DetectionArea.ExtendingPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointsManager.Position, .1f);
        Gizmos.color = Color.blue;
        for (int i = 0; i < pointsManager.AllWavePoints.Count; i++)
        {
            Gizmos.DrawSphere(pointsManager.AllWavePoints[i].Position, .1f);
            Gizmos.DrawLine(pointsManager.AllWavePoints[i].Position, pointsManager.Position); 
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
    /// <summary>
    /// Destroy all dead enemies
    /// </summary>
    public void ClearDeadEnemies()
    {
        destroyingEnemies.ForEach(e => PhotonNetwork.Destroy(e.gameObject));
        destroyingEnemies.Clear();
    }

    /// <summary>
    /// Get an enemy in the spawned enemies list and remove it 
    /// Go on next wave when all the enemies are killed
    /// </summary>
    /// <param name="_enemy">enemy to remove</param>
    public void RemoveEnemy(TDS_Enemy _enemy)
    {
        int _index = spawnedEnemies.IndexOf(spawnedEnemies.Where(e => e.PhotonViewElementID == _enemy.PhotonViewElementID).FirstOrDefault());
        spawnedEnemies.RemoveAt(_index);
        destroyingEnemies.Add(_enemy);
        if (_enemy.LifeBar) PhotonNetwork.Destroy(_enemy.LifeBar.gameObject); 
        if(spawnedEnemies.Count == 0)
        {
            ClearDeadEnemies(); 
            if(!isLooping)
            {
                waveNumber++; 
            }
            OnNextWave?.Invoke();
        }
    }

    /// <summary>
    /// Get the points and spawn enemies in it 
    /// Then add the enemy to the spawned enemies list
    /// </summary>
    void Spawn()
    {
        if (!PhotonNetwork.isMasterClient) return;

        List<TDS_SpawningInformation> _spawnInformations = pointsManager.GetSpawningInformations(waveNumber);
        if (_spawnInformations.Count == 0)
        {
            waveNumber++;
            OnNextWave?.Invoke(); 
        }
        for (int i = 0; i < _spawnInformations.Count; i++)
        {
            TDS_Enemy _enemy = PhotonNetwork.Instantiate(((EnemyName)_spawnInformations[i].PrefabId).ToString(), _spawnInformations[i].SpawnPosition + Vector3.up, Quaternion.identity, 0).GetComponent<TDS_Enemy>();
            if(enemyCanvas && (lifeBarName != string.Empty))
            {
                Vector3 _pos = new Vector3(_enemy.transform.position.x, .1f, _enemy.transform.position.z);
                Quaternion _rotation = Quaternion.Euler(90, 0, 0); 
                TDS_FilledBar _bar = PhotonNetwork.Instantiate(lifeBarName, _pos, _rotation, 0).GetComponent<TDS_FilledBar>();
                _bar.SetCanvas(enemyCanvas); 
                _enemy.SetLifeBar(_bar); 
            }
            _enemy.SetOwner(this); 
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
            _enemy.SetOwner(this); 
            spawnedEnemies.Add(_enemy);
        }
    }

    IEnumerator GetEnemiesInformations()
    {
        while (!PhotonNetwork.inRoom)
        {
            yield return new WaitForEndOfFrame();
        }
        if (PhotonNetwork.isMasterClient) yield break;
        TDS_RPCManager.Instance.UpdateAreaInformations(areaPhotonView.viewID); 
    }
    #endregion

    #region FigthingAreaInformations
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_areaID"></param>
    /// <param name="_enemies"></param>
    /// <returns></returns>
    public string GetFightingAreaInfos()
    {
        string _info = $"{areaPhotonView.viewID}";
        foreach (TDS_Enemy _enemy in spawnedEnemies.ToList())
        {
            //PrefabName#PosX#PosY#PosZ
            _info += $"|{((int)_enemy.PrefabName)}#{_enemy.transform.position.x}#{_enemy.transform.position.y}#{_enemy.transform.position.z}#{_enemy.Health}#{_enemy.PhotonViewElementID}";
        }
        foreach (TDS_Enemy _enemy in destroyingEnemies)
        {
            _info += $"|{((int)_enemy.PrefabName)}#{_enemy.transform.position.x}#{_enemy.transform.position.y}#{_enemy.transform.position.z}#0#{_enemy.PhotonViewElementID}";
        }
        // THIS PART IS USED WHEN THE PLAYER DOES NOT LEAVE
        //_area.SpawnedEnemies.Clear();
        // END OF THE PART
        return _info;
    }
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
    public int EnemyHealth; 

    //Add all transfered informations (animation state, life, etc...)

    public TDS_EnemyInfo(string _info)
    {
        EnemyType = int.Parse(_info.Split('#')[0]);
        float _xPos = float.Parse(_info.Split('#')[1]);
        float _yPos = float.Parse(_info.Split('#')[2]);
        float _zPos = float.Parse(_info.Split('#')[3]);
        EnemyPosition = new Vector3(_xPos, _yPos, _zPos);
        EnemyHealth = int.Parse(_info.Split('#')[4]);
        EnemyId = int.Parse(_info.Split('#')[5]); 

    }

}
#endregion


