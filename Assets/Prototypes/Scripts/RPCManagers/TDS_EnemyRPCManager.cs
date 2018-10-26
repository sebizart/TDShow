using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon; 

/*
[Script Header] CATA_EnemyRPCManager Version 0.0.1
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

public class TDS_EnemyRPCManager : PunBehaviour 
{
    #region Fields/Properties
    public static TDS_EnemyRPCManager Instance;
    public PhotonView EnemyRPCManagerPhotonView;
    public List<TDS_EnemyOLD> AllEnemies = new List<TDS_EnemyOLD>();
    #endregion


    #region Methods
    public void AddNewEnemy(TDS_EnemyOLD _enemy) => AllEnemies.Add(_enemy);

    /// <summary>
    /// Get all damages informations for each touched enemies and apply the damages
    /// </summary>
    /// <param name="_damagesInformations"></param>
    public void GetDamagesInformation(string _damagesInformations)
    {
        if (!PhotonNetwork.isMasterClient) return;
        string[] _enemiesDamages = SplitInformations(_damagesInformations);
        int _enemyId;
        int _damages; 
        for (int i = 0; i < _enemiesDamages.Length; i++)
        {
            string[] _enemyDamages = _enemiesDamages[i].Split(',');
            if (int.TryParse(_enemyDamages[0], out _enemyId))
            {
                if (int.TryParse(_enemyDamages[1], out _damages))
                {
                    EnemyRPCManagerPhotonView.RPC("ApplyLifeModification", PhotonTargets.All, _enemyId, _damages); 
                }
                else break; 
            }
            else break; 
        }
    }

    /// <summary>
    /// Split the damages informations
    /// </summary>
    /// <param name="_damagesInfo"></param>
    /// <returns></returns>
    private string[] SplitInformations(string _damagesInfo)
    {
        return _damagesInfo.Split('|'); 
    }

    #region RPC
    /// <summary>
    /// Call this Methods when a Enemy is instanciated to add it to the list of all enemies
    /// </summary>
    /// <param name="_enemyViewID"></param>
    [PunRPC]
    public void UpdateEnemyList(int _enemyViewID)
    {
        PhotonView _enemyPhotonView = PhotonView.Find(_enemyViewID);
        if (!_enemyPhotonView)
        {
            return;
        }
        TDS_EnemyOLD _enemy = _enemyPhotonView.GetComponent<TDS_EnemyOLD>();
        if (!_enemy)
        {
            return;
        }
        AllEnemies.Add(_enemy);
    }

    /// <summary>
    /// Apply life modification to an enemy with the selected PunID
    /// </summary>
    /// <param name="_punID"></param>
    /// <param name="_newValue"></param>
    [PunRPC]
    public void ApplyLifeModification(int _punID, int _newValue)
    {
        PhotonView.Find(_punID).GetComponent<TDS_EnemyOLD>().ApplyDamages(_newValue);
    }

    /// <summary>
    /// Project an enemy and apply damages to him with the selected PunID 
    /// </summary>
    /// <param name="_punID"></param>
    /// <param name="_dmgValue"></param>
    /// <param name="_owner"></param>
    [PunRPC]
    public void ProjectEnemyAndApplyLifeModification(int _punID, int _dmgValue, int _owner)
    {
        PhotonView _user = PhotonView.Find(_owner);
        if (!_user)
        {
            return;
        }
        PhotonView.Find(_punID).GetComponent<TDS_EnemyOLD>().BeingProjected(_dmgValue, _user.transform);
    }

    /// <summary>
    /// Fully regenerate the life of an enemy with the selected PunID 
    /// </summary>
    /// <param name="_punID"></param>
    [PunRPC]
    public void RegenerateLife(int _punID)
    {
        foreach (TDS_EnemyOLD _e in AllEnemies)
        {
            if (_e.EnemyPhotonView.viewID == _punID)
            {
                _e.ApplyRegeneration();
            }
        }
    }

    #endregion

    #endregion

    #region UnityMethods
    private void Awake()
    {
        if(Instance)
        {
            Destroy(this);
            return; 
        }
        Instance = this; 
    }
	#endregion
}
