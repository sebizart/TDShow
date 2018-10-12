﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Photon; 

/*
[Script Header] CATA_Networking Version 0.0.1
Created by: Thiebaut Alexis
Date: 05/10/2018
Description: Connect the player to a room and instantiate the prefab for each player 

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/

public class TDS_Networking : PunBehaviour 
{
    #region Fields/Properties
    public static TDS_Networking Instance; 
    public int OwnerID { get; private set; }

    //Spawn positions of every character
    [SerializeField] CATA_PlayerSpawnPoint[] spawnPoints;

    //Player name in local
    [SerializeField] string playerName = "Player";

    private bool isHost = false; 
    public bool IsHost { get { return isHost; } }

    public PhotonView networkId; 
    #endregion

    #region Methods
    void InitConnection()
    {
        //Connect the player to Photon
        PhotonNetwork.ConnectUsingSettings("1.0"); 
    }

    /// <summary>
    /// Create a room with several options or join it if this room already exists
    /// </summary>
    void JoinRoom()
    {
        // Create options for a room: it is visible and limited to 4 players
        RoomOptions _options = new RoomOptions()
        {
            IsVisible = true,
            MaxPlayers = 4,
        };
        // Connect the player to a room named (here "CATA_EPIIC") with the options created before and to LobbyType
        PhotonNetwork.JoinOrCreateRoom("CATA_EPIIC", _options, null); 
    }

    
    #endregion

    #region UnityMethods
    private void Awake()
    {
        if (Instance == null) Instance = this; 
    }
    void Start () 
	{
        InitConnection();
        networkId = GetComponent<PhotonView>(); 
	}
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Gizmos.DrawSphere(spawnPoints[i].SpawnPosition, .2f); 
        }
    }
    private void OnGUI()
    {
        GUILayout.Box(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Box(PhotonNetwork.isMasterClient.ToString()); 
    }
    #endregion

    #region PhotonMethods
    /// <summary>
    /// When the player is connected to master, he joins the room
    /// </summary>
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        JoinRoom(); 

    }
    
    /// <summary>
    /// When the player joins the room, instantiate a prefab for the player and set its name with the player name
    /// </summary>
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonView _playerId = PhotonNetwork.Instantiate("Beard Leady - Socle", Vector3.zero + Vector3.up, Quaternion.identity,0).GetComponent<PhotonView>();
        if (_playerId.isMine)
        {
            OwnerID = _playerId.viewID;
        }
        List<TDS_Controller> _players = FindObjectsOfType<TDS_Controller>().ToList();
        TDS_PlayerRPCManager.Instance.AllPlayers = _players; 
    }

    /// <summary>
    /// When the player create a room, he's the host of the game
    /// </summary>
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        isHost = true; 
    }
    #endregion
}

[Serializable]
public class CATA_PlayerSpawnPoint
{
    [SerializeField] Vector3 spawnPosition;
    public Vector3 SpawnPosition { get { return spawnPosition; } }
    [SerializeField] string playerPrefabName = "Character";
    public string PlayerPrefabName { get { return playerPrefabName; } }
}

                                           
                                            
                                            