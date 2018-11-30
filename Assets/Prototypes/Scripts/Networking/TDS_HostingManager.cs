using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Photon;

/*
[Script Header] TDS_HostingManager Version 0.0.1
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
[RequireComponent(typeof(PhotonView))]
public class TDS_HostingManager : PunBehaviour
{
    #region Field and properties
    public static TDS_HostingManager Instance;
    [SerializeField] PhotonView hostingManagerPhotonView;

    private bool canLeave = false;

    [Header("Transfered Datas")]
    [SerializeField] List<TDS_FightingArea> allAreas = new List<TDS_FightingArea>(); 
    //PROPS
    #endregion

    #region UnityMethods
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this; 
        }
        else
        {
            Destroy(this);
            return; 
        }
    }
    private void Start()
    {
        if (hostingManagerPhotonView == null) hostingManagerPhotonView = GetComponent<PhotonView>();
        if (allAreas.Count == 0) allAreas = FindObjectsOfType<TDS_FightingArea>().ToList();
    }
    private void OnGUI()
    {
        if (!PhotonNetwork.isMasterClient) return;
        if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 50, 200, 50), "ChangeOwner"))
        {
            SetNewMasterClient();
        }
    }
    #endregion

    #region Methods

    public void GetMigrationFeedBack()
    {
        canLeave = true;
        Application.Quit(); 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_areaID"></param>
    /// <param name="_enemies"></param>
    /// <returns></returns>
    private string SetFightingAreaInfos(TDS_FightingArea _area)
    {
        string _info = $"{_area.AreaPhotonView.viewID}";
        foreach (TDS_Enemy _enemy in _area.SpawnedEnemies.ToList())
        {
            //PrefabName#PosX#PosY#PosZ
            Debug.Log(_enemy.transform.position); 
            _info += $"|{((int)_enemy.PrefabName)}#{_enemy.transform.position.x}#{_enemy.transform.position.y}#{_enemy.transform.position.z}";
        }
        // THIS PART IS USED WHEN THE PLAYER DOES NOT LEAVE
        //_area.SpawnedEnemies.Clear();
        // END OF THE PART
        return _info;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private string SetPropsInformations()
    {
        // ADD PROPS INFOS
        // SEPARATE WITH @ AND ADD IT INTO _info
        return string.Empty; 
    }

    /// <summary>
    /// Migrate informations while using RPC Requests #TRY#
    /// </summary>
    private void MigrateInformations()
    {
        //MIGRATE ALL DATAS FOREACH ENEMY AND PROPS
        //Start informations with the id of the previous master
        string _info = $"{ hostingManagerPhotonView.owner.ID }@";
        for (int i = 0; i < allAreas.Count; i++)
        {
            TDS_FightingArea _area = allAreas[i];
            _area.ClearDeadEnemies(); 
            if (_area.DetectionState == SpawnPointState.Enable)
            {
                // SEPARATE WITH & AND ADD OTHER INFOMATIONS
                if (i >= 1) _info += '&';
                _info += SetFightingAreaInfos(_area);
            }
        }
        //ADD PROPS INFORMATIONS + SEPARATE WITH @

        //SEND INFORMATIONS TO THE NEW MASTER
        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("ReceiveMigrationsInformations", PhotonTargets.MasterClient, _info);
        hostingManagerPhotonView.TransferOwnership(PhotonNetwork.masterClient);
    }

    /// <summary>
    /// Set the next client on the list as the masterclient
    /// </summary>
    public void SetNewMasterClient()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            canLeave = true; 
            return;
        }
        //Set a new Master if there is more than one player
        if (PhotonNetwork.otherPlayers.Length == 0)
        {
            canLeave = true;
            return;
        }
        PhotonNetwork.SetMasterClient(PhotonNetwork.otherPlayers[0]);
        //CHANGING OWNERSHIP CAN'T WORK SO WE HAVE TO RE-INSTANCIATE THE ENEMIES
        StartCoroutine(WaitNewMaster());
    }

    /// <summary>
    /// while the master didn't change wait
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitNewMaster()
    {
        while (hostingManagerPhotonView.owner.IsMasterClient)
        {
            yield return new WaitForEndOfFrame();
        }
        MigrateInformations();
        yield break;
    }
    #endregion

    #region Pun Methods
    public override void OnJoinedRoom()
    {
        if (hostingManagerPhotonView.owner == null) hostingManagerPhotonView.TransferOwnership(PhotonNetwork.masterClient);
    }
    #endregion

}
