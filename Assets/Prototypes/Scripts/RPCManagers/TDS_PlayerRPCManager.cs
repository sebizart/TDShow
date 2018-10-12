using System;
using System.Collections;
using System.Collections.Generic;
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
    public PhotonView PlayerRPCManagerPhotonView; 
    public List<TDS_Controller> AllPlayers = new List<TDS_Controller>();
    #endregion

    #region Methods

    [PunRPC]
    public void CheckHitBox(int _playerPunID)
    {
        if (!PhotonNetwork.isMasterClient) return; 
        
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
