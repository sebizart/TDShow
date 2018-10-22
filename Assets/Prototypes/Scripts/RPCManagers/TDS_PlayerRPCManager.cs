using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Photon;
using Random = UnityEngine.Random;

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
    public PhotonView PlayerRPCManagerPhotonView; 
    public List<TDS_Controller> AllPlayers = new List<TDS_Controller>();
    #endregion

    #region Methods
    #region OLD
    /// <summary>
    /// Check if there is something to project in the hit box of the player
    /// if there is something to project, tell everyone to hit this element
    /// </summary>
    /// <param name="_playerPunID"></param>
    [PunRPC]
    public void CheckHitBox(int _playerPunID)
    {
        if (!PhotonNetwork.isMasterClient) return; 
        TDS_Controller _player = GetControllerByID(_playerPunID);
        if (_player == null) return;

        //Check all verifications to throw the attack method
        // If the player can't attack, return
        if (!_player.CanAttack) return;

        // Raycast in the box of the attack
        TDS_Enemy[] _enemies = _player.AttackRaycast("Melee Box");
        
        if(_enemies.Length > 0)
        {
            int _enemyID; 
            int _damages;
            string _infosDamages = string.Empty;
            foreach (TDS_Enemy _e in _enemies)
            {
                if (!_e.GetComponent<PhotonView>()) break; 
                _enemyID = _e.GetComponent<PhotonView>().viewID; 
                _damages = Mathf.Clamp(Random.Range(1, _player.ComboValue + 1), _player.DamagesMin, _player.DamagesMax + 1);
                _infosDamages += $"{_enemyID},{_damages}|"; 
            }
            //Send Damages Informations to the enemyManager
            TDS_EnemyRPCManager.Instance.GetDamagesInformation(_infosDamages);
            PlayerRPCManagerPhotonView.RPC("SendInfosToPlayer", PhotonTargets.All, _playerPunID); 
        }
    }

    /// <summary>
    /// Check if there is something to project in the project box of the player
    /// if there is something to project, tell everyone to project this element
    /// 
    /// </summary>
    /// <param name="_playerPunID"></param>
    [PunRPC]
    public void CheckProjectionBox(int _playerPunID)
    {
        TDS_Controller _player = GetControllerByID(_playerPunID);
        if (_player == null) return;

        // If the player can't attack, return
        if (!_player.CanAttack) return;

        // Raycast in the box of the attack
        TDS_Enemy[] _enemies = _player.AttackRaycast("Project Box");

        if (_enemies.Length == 0) return;

        // Select the nearest interactable element and project it
        TDS_Enemy _nearestEnemy = _enemies.OrderBy(i => Vector3.Distance(transform.position, i.transform.position)).First();
        if (!_nearestEnemy.GetComponent<PhotonView>()) return ;
        int _enemyId = _nearestEnemy.GetComponent<PhotonView>().viewID;
        TDS_EnemyRPCManager.Instance.EnemyRPCManagerPhotonView.RPC("ProjectEnemyAndApplyLifeModification", PhotonTargets.All, _enemyId, _player.DamagesProjection, _playerPunID);
        PlayerRPCManagerPhotonView.RPC("SendProjectionInfosToPlayer", PhotonTargets.All, _playerPunID); 
    }

    /// <summary>
    /// Send the information to the player with the Id that he hit something
    /// </summary>
    /// <param name="_playerID"></param>
    [PunRPC]
    public void SendHitInfosToPlayer(int _playerID)
    {
        TDS_Controller _player = GetControllerByID(_playerID);
        if (!_player) return; 
        _player.HitVerified(); 
    }

    /// <summary>
    /// Send the information to the player with the Id that he projects something
    /// </summary>
    /// <param name="_playerID"></param>
    [PunRPC]
    public void SendProjectionInfosToPlayer(int _playerID)
    {
        TDS_Controller _player = GetControllerByID(_playerID);
        if (!_player) return;
        _player.ProjectVerified();
    }
    #endregion



    /// <summary>
    /// Search the player according to his PhotonView ID
    /// </summary>
    /// <param name="_playerPunID"></param>
    /// <returns></returns>
    private TDS_Controller GetControllerByID(int _playerPunID)
    {
        // Search the player with the _playerPunID
        if (!PhotonNetwork.isMasterClient) return null;
        PhotonView _playerPhotonView = PhotonView.Find(_playerPunID);
        if (!_playerPhotonView) return null;
        TDS_Controller _player = _playerPhotonView.GetComponent<TDS_Controller>();
        if (!_player) return null;
        else return _player;
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
