using System;
using System.Collections;
using System.Collections.Generic;
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
    /// Set the info damages into a string
    /// </summary>
    /// <param name="_dico">Dictionary of damageableElements and linked damages</param>
    /// <param name="_charID">PhotonView ID of the attacking character</param>
    /// <param name="_attackID">ID of the attack</param>
    /// <returns></returns>
    private string SetInfoDamages(Dictionary<int, int> _dico, int _charID, int _attackID)
    {
        string _info = $"{_charID}#{_attackID}";
        foreach (KeyValuePair<int, int> _pair in _dico)
        {
            _info += $"|{_pair.Key}#{_pair.Value}";
        }
        return _info;
    }

    #region RPC Requests
    #region Actions
    /// <summary>
    /// Launch an action on a character by its ID
    /// </summary>
    /// <param name="_characterID">Id of the character</param>
    /// <param name="_actionID">ID of the action</param>
    [PunRPC]
    public void LaunchAction(int _characterID, int _actionID)
    {
        TDS_Character _character = GetCharacterByID(_characterID);
        if (_character == null)
        {
            TDS_CustomDebug.CustomDebugLog($"No Character found with the ID {_characterID}");
            return;
        }
        _character.Action(_actionID);
    }
    #endregion

    #region Attacks
    /// <summary>
    /// Launch the attack with the character by the ID
    /// Get this character
    /// Check its attackBox
    /// And Apply Info damages on every PhotonTarget
    /// </summary>
    /// <param name="_characterID"></param>
    /// <param name="_attackID"></param>
    [PunRPC]
    public void LaunchAttack(int _characterID, int _attackID)
    {
        if (!PhotonNetwork.isMasterClient) return;
        TDS_Character _char = GetCharacterByID(_characterID);
        if (_char == null)
        {
            TDS_CustomDebug.CustomDebugLog($"No Character found with the ID {_characterID}");
            return;
        }
        Dictionary<int, int> _allChars = _char.CheckHit(_attackID);
        RPCManagerPhotonView.RPC("ApplyInfoDamages", PhotonTargets.All, SetInfoDamages(_allChars, _characterID, _attackID));
    }

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

    #region Grab & Throw Object
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

    #region SpawnPoints
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
    public int AttackId;
    public List<TDS_AttackSubInfo> AllSubInfos = new List<TDS_AttackSubInfo>();
    
    public TDS_AttackInfo(string _info)
    {
        AttackerId = int.Parse(_info.Split('|')[0].Split('#')[0]); 
        AttackId = int.Parse(_info.Split('|')[0].Split('#')[1]);
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
