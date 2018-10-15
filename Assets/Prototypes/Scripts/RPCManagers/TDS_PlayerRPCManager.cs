using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon; 

/*
[Script Header] TDS_PlayerRPCManager Version 0.0.1
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

public class TDS_PlayerRPCManager : PunBehaviour
{
    #region Fields/Properties
    public static TDS_PlayerRPCManager Instance;
    public PhotonView photonID; 
    public List<TDS_Controller> AllPlayers = new List<TDS_Controller>();
    #endregion

    #region Methods
    [PunRPC]
    public void AddPlayer(int _playerID, bool _sendToOthers)
    {
        if (_sendToOthers)
        {
            photonID.RPC("AddPlayer", PhotonTargets.Others, _playerID, false);
        }
        else
        {
            AllPlayers.Add(PhotonView.Find(_playerID).GetComponent<TDS_Controller>());
        }
    }

    [PunRPC]
    public void Hit(string _enemiesAndDamages)
    {
        if (PhotonNetwork.isMasterClient)
        {
            photonID.RPC("Hit", PhotonTargets.Others, _enemiesAndDamages);
        }
        else
        {
            TDS_EnemyRPCManager.Instance.HitEnemies(_enemiesAndDamages);
        }
    }

    [PunRPC]
    public void HitCheck(int _playerID)
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonView _playerPV = PhotonView.Find(_playerID);

            TDS_Controller _player = AllPlayers.Where(p => p.photonView.viewID == _playerID).FirstOrDefault();
            if (!_player) return;
            _player.HitCheck();
        }
        else
        {
            photonID.RPC("HitCheck", PhotonTargets.MasterClient, _playerID);
        }
    }

    [PunRPC]
    public void Project(string _enemyDamagesAndTransform)
    {
        if (PhotonNetwork.isMasterClient)
        {
            photonID.RPC("Project", PhotonTargets.Others, _enemyDamagesAndTransform);
        }
        else
        {
            string[] _separed = _enemyDamagesAndTransform.Split(',');
            int _enemyId = 0;
            int _damages = 0;
            int.TryParse(_separed[0], out _enemyId);
            int.TryParse(_separed[1], out _damages);
            TDS_Controller _player = AllPlayers.Where(p => p.photonView.viewID.ToString() == _separed[2]).FirstOrDefault();

            if (!_player) return;

            TDS_EnemyRPCManager.Instance.ProjectEnemy(_enemyId, _damages, _player.transform);
        }
    }

    [PunRPC]
    public void ProjectCheck(int _playerID)
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_Controller _player = AllPlayers.Where(p => p.photonView.viewID == _playerID).FirstOrDefault();
            if (!_player) return;
            _player.ProjectCheck();
        }
        else
        {
            photonID.RPC("ProjectCheck", PhotonTargets.MasterClient, _playerID);
        }
    }
    #endregion

    #region UnityMethods
    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this; 
    }
    #endregion
}
