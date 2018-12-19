using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon; 

/*
[Script Header] TDS_RPCManager Version 0.0.1
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

public class TDS_RPCManager : PunBehaviour 
{
    #region Fields/Properties
    public static TDS_RPCManager Instance;
    public PhotonView RPCManagerPhotonView;
    #endregion

    #region Methods
    // TRY TO GLOBALIZE THE METHODS IN ONE 

    /// <summary>
    /// Get the character with its photonView ID
    /// </summary>
    /// <param name="_characterID">photonview ID</param>
    /// <returns></returns>
    private TDS_Character GetCharacterByID(int _characterID)
    {
        // Search a character with the _characterID
        PhotonView _playerPhotonView = PhotonView.Find(_characterID);
        if (!_playerPhotonView) return null;
        TDS_Character _char = _playerPhotonView.GetComponent<TDS_Character>();
        return _char;
    }

    /// <summary>
    /// Get the DamageableElement with its PhotonView ID
    /// </summary>
    /// <param name="_elementID"></param>
    /// <returns></returns>
    private TDS_DamageableElement GetDamageableElementByID(int _elementID)
    {

        PhotonView _playerPhotonView = PhotonView.Find(_elementID);
        if (!_playerPhotonView) return null;
        TDS_DamageableElement _elem = _playerPhotonView.GetComponent<TDS_DamageableElement>();
        return _elem;
    }

    /// <summary>
    /// Get the Fighting Area with its PhotonView ID
    /// </summary>
    /// <param name="_areaID"></param>
    /// <returns></returns>
    private TDS_FightingArea GetFightingAreaByID(int _areaID)
    {
        PhotonView _areaPhotonView = PhotonView.Find(_areaID);
        if (!_areaPhotonView) return null;
        TDS_FightingArea _elem = _areaPhotonView.GetComponent<TDS_FightingArea>();
        return _elem;
    }

    // END GLOBALIZATION

    /// <summary>
    /// Set the info damages into a string
    /// </summary>
    /// <param name="_dico">Dictionary of damageableElements and linked damages</param>
    /// <param name="_charID">PhotonView ID of the attacking character</param>
    /// <param name="_attackID">ID of the attack</param>
    /// <returns></returns>
    public string SetInfoDamages(Dictionary<int, int> _dico, int _charID)
    {
        string _info = $"{_charID}";

        if (_dico != null)
        {
            foreach (KeyValuePair<int, int> _pair in _dico)
            {
                _info += $"|{_pair.Key}#{_pair.Value}";
            }
        }

        return _info;
    }

    #region RPC Requests
    #region Actions
    /// <summary>
    /// Launch an action with the character by the ID
    /// </summary>
    /// <param name="_characterID">ID of the character</param>
    /// <param name="_actionID">ID of the action</param>
    [PunRPC]
    public void LaunchAction(int _characterID, string _actionID)
    {
        TDS_Character _char = GetCharacterByID(_characterID);
        if (_char == null)
        {
            TDS_CustomDebug.CustomDebugLog($"No Character found with the ID {_characterID}");
            return;
        }
        _char.ExecuteAction(_actionID);
    }
    #endregion

    #region Attacks
    /// <summary>
    /// Create a AttackInfo object which contains all the infos in the string
    /// </summary>
    /// <param name="_infoDamages">info damages stocked into a string</param>
    [PunRPC]
    private void ApplyInfoDamages(string _infoDamages)
    {
        TDS_AttackInfo _infos = new TDS_AttackInfo(_infoDamages);
        //TDS_Character _character = GetCharacterByID(_infos.AttackerId);
        _infos.AllSubInfos.ForEach(i => (GetDamageableElementByID(i.TargetId)).TakeDamage(i.Damages));
    }
    #endregion

    #region FightingAreaInformations
    [PunRPC]
    public void ApplyAreaInformations(string _areaInfo)
    {
        if (!PhotonNetwork.isMasterClient) return;
        TDS_FightingAreaInfo _infos = new TDS_FightingAreaInfo(_areaInfo);
        TDS_FightingArea _area = GetFightingAreaByID(_infos.FightingAreaID);
        if (_area == null) return;
        _area.SpawnEnemiesUsingInfos(_infos.EnemiesInfos);
    }
    #endregion

    #region Grab & Throw Object
    /// <summary>
    /// Makes a character drop an object
    /// </summary>
    /// <param name="_characterID">ID of the character</param>
    [PunRPC]
    public void DropObject(int _characterID)
    {
        TDS_Character _char = GetCharacterByID(_characterID);
        if (_char == null)
        {
            TDS_CustomDebug.CustomDebugLog($"No Character found with the ID {_characterID}");
            return;
        }
        _char.DropObject();
    }

    /// <summary>
    /// Makes a character grab an object
    /// </summary>
    /// <param name="_characterAndObject">String containing the ID of the character & the Id of the object, separated by a "|"</param>
    [PunRPC]
    public void GrabObject(string _characterAndObject)
    {
        string[] _split = _characterAndObject.Split('|');
        int _characterID = int.Parse(_split[0]);
        TDS_Character _char = GetCharacterByID(_characterID);
        if (_char == null)
        {
            TDS_CustomDebug.CustomDebugLog($"No Character found with the ID {_characterID}");
            return;
        }
        _char.GrabObject(int.Parse(_split[1]));
    }

    /// <summary>
    /// Makes a character throw its weired object
    /// </summary>
    /// <param name="_characterID">ID of the character throwing the object</param>
    [PunRPC]
    public void ThrowObject(int _characterID)
    {
        TDS_Character _char = GetCharacterByID(_characterID);
        if (_char == null)
        {
            TDS_CustomDebug.CustomDebugLog($"No Character found with the ID {_characterID}");
            return;
        }
        _char.ThrowObject();
    }
    /// <summary>
    /// Makes the juggler throw an object
    /// </summary>
    /// <param name="_characterID">ID of the juggler</param>
    /// <param name="_velocity">Velocity to give to the object</param>
    [PunRPC]
    public void ThrowObject(int _characterID, Vector3 _velocity)
    {
        TDS_Character _char = GetCharacterByID(_characterID);
        if (_char == null)
        {
            TDS_CustomDebug.CustomDebugLog($"No Character found with the ID {_characterID}");
            return;
        }
        TDS_Juggler _juggler = _char.GetComponent<TDS_Juggler>();

        if (_juggler)
        {
            _juggler.ThrowObject(_velocity);
        }
    }

    /// <summary>
    /// Makes the juggler throw a mystery ball
    /// </summary>
    /// <param name="_characterID">ID of the juggler</param>
    /// <param name="_velocity">Velocity of the mystery ball</param>
    [PunRPC]
    public void ThrowMysteryBall(int _characterID, Vector3 _velocity)
    {
        TDS_Character _char = GetCharacterByID(_characterID);
        if (_char == null)
        {
            TDS_CustomDebug.CustomDebugLog($"No Character found with the ID {_characterID}");
            return;
        }
        TDS_Juggler _juggler = _char.GetComponent<TDS_Juggler>();

        if (_juggler)
        {
            _juggler.ThrowMysteryBall(_velocity);
        }
    }

    /// <summary>
    /// Calculate the first object a character can grab, and if found an object grab it by RPC
    /// </summary>
    /// <param name="_characterID">ID of the character trying to grab an object</param>
    [PunRPC]
    public void TryToGrabObject(int _characterID)
    {
        if (!PhotonNetwork.isMasterClient) return;
        TDS_Character _char = GetCharacterByID(_characterID);
        TDS_Throwable _throwable = null;
        if (_char == null)
        {
            TDS_CustomDebug.CustomDebugLog($"No Character found with the ID {_characterID}");
            return;
        }
        if (_throwable = _char.TryToGrabObject())
        {
            string _params = $"{_characterID}|{_throwable.PhotonID}";
            RPCManagerPhotonView.RPC("GrabObject", PhotonTargets.All, _params);
        }
        else
        {
            TDS_CustomDebug.CustomDebugLog("No object found");
        }
    }
    #endregion

    #region HostMigration
    [PunRPC]
    public void ReceiveMigrationsInformations(string _migrationInfos)
    {
        if (!PhotonNetwork.isMasterClient) return;
        //Debug.Log(_migrationInfos); 
        //SPLIT AT '@' TO GET EXMASTER ID AT 0, FIGHTING AREA INFO AT [1] AND PROPS INFORMATIONS AT [2]
        string _areasInfos = _migrationInfos.Split('@')[1];
        //string _propsInfos = _migrationInfos.Split('@')[2]; 
        if (_areasInfos != string.Empty)
        {
            //SPLIT AGAIN AT '&' TO GET EVERY ACTIVE ZONE (NORMALLY JUST ONE BUT JUST IN CASE)
            string[] _allAreasInfo = _areasInfos.Split('&');
            for (int i = 0; i < _allAreasInfo.Length; i++)
            {
                TDS_FightingAreaInfo _infos = new TDS_FightingAreaInfo(_allAreasInfo[i]);
                TDS_FightingArea _area = GetFightingAreaByID(_infos.FightingAreaID);
                if (_area == null) return;
                _area.SpawnEnemiesUsingInfos(_infos.EnemiesInfos);
            }
        }
        // if(_propsInfos != string.Empty)
        // {
        //     // ADD PROPS PART
        // }
        int _exMasterID = int.Parse(_migrationInfos.Split('@')[0]);
        PhotonPlayer _exMaster = PhotonPlayer.Find(_exMasterID);
        RPCManagerPhotonView.RPC("ReceptionFeeback", _exMaster);
    }

    [PunRPC]
    public void ReceptionFeeback()
    {
        TDS_HostingManager.Instance.GetMigrationFeedBack();
    }
    #endregion

    #region Spawn
    /// <summary>
    /// Adds an other player in game settings
    /// </summary>
    /// <param name="_playerCharacter">Player type to add</param>
    [PunRPC]
    public void AddPlayer(int _playerCharacter)
    {
        TDS_GameManager.Instance.InGamePlayers[(PlayerCharacter)_playerCharacter] = true;
        TDS_UIManager.Instance.AddPlayer((PlayerCharacter)_playerCharacter);
    }

    /// <summary>
    /// Receives informations about in-game players
    /// </summary>
    /// <param name="_players">String containing all in-game player types as int</param>
    [PunRPC]
    public void ReceiveInGamePlayers(string _players)
    {
        if (_players == string.Empty) return;

        string[] _split = _players.Split('|');
        foreach (string _inGamePlayer in _split)
        {
            int _character = int.Parse(_inGamePlayer);
            if (TDS_GameManager.Instance.InGamePlayers[(PlayerCharacter)_character] == false)
            {
                TDS_GameManager.Instance.InGamePlayers[(PlayerCharacter)_character] = true;
                TDS_UIManager.Instance.AddPlayer((PlayerCharacter)_character);
            }
        }

        TDS_UIManager.Instance.RefreshCharacterSelection();
    }

    /// <summary>
    /// Removes a player from game's settings
    /// </summary>
    /// <param name="_playerCharacter">Player type to remove</param>
    [PunRPC]
    public void RemovePlayer(int _playerCharacter)
    {
        TDS_Player _this = FindObjectsOfType<TDS_Player>().Where(p => p.PhotonViewElement.isMine).FirstOrDefault();

        if (_this && _this.Character == (PlayerCharacter)_playerCharacter)
        {
            _this.DestroyCharacter();

            return;
        }

        TDS_GameManager.Instance.InGamePlayers[(PlayerCharacter)_playerCharacter] = false;
        TDS_UIManager.Instance.RemovePlayer((PlayerCharacter)_playerCharacter);
    }

    /// <summary>
    /// Send in-game player's informations to all other clients
    /// </summary>
    [PunRPC]
    public void SendInGamePlayers()
    {
        if (!PhotonNetwork.isMasterClient) return;

        string _toSend = string.Empty;

        foreach (KeyValuePair<PlayerCharacter, bool> _player in TDS_GameManager.Instance.InGamePlayers)
        {
            if (_player.Value == true)
            {
                if (_toSend != string.Empty) _toSend += '|';
                int _type = (int)_player.Key;
                _toSend += _type;
            }
        }

        RPCManagerPhotonView.RPC("ReceiveInGamePlayers", PhotonTargets.Others, _toSend);
    }
    #endregion
    #endregion
    #endregion

    #region UnityMethods
    private void Awake()
    {
        if (!Instance) Instance = this;  
    }

    void Start () 
	{
	}

    void Update () 
	{
    }
    #endregion
}

public class TDS_AttackInfo
{
    public int AttackerId;
    public List<TDS_AttackSubInfo> AllSubInfos = new List<TDS_AttackSubInfo>();
    
    public TDS_AttackInfo(string _info)
    {
        AttackerId = int.Parse(_info.Split('|')[0]); 
        for (int i = 1; i < _info.Split('|').Length; i++)
        {
            AllSubInfos.Add(new TDS_AttackSubInfo(_info.Split('|')[i])); 
        }
    }
}
public class TDS_AttackSubInfo
{
    public int TargetId;
    public int Damages; 

    public TDS_AttackSubInfo(string _subInfo)
    {
        TargetId = int.Parse(_subInfo.Split('#')[0]);
        Damages = int.Parse(_subInfo.Split('#')[1]); 
    }
}
