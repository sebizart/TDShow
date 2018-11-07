using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_GameManager : MonoBehaviour
{
    #region Events

    #endregion

    #region Fields / Properties
    // All available player characters associated with a bool indicating if their are already in game
    public Dictionary<PlayerCharacter, bool> InGamePlayers = new Dictionary<PlayerCharacter, bool>();
    #endregion

    #region Singleton
    // The singleton instance of this class
    public static TDS_GameManager Instance = null;
    #endregion

    #region Methods
    #region Original Methods
    public void LeftParty(PlayerCharacter _player)
    {
        InGamePlayers[_player] = false;

        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("RemovePlayer", PhotonTargets.Others, (int)_player);
        TDS_UIManager.Instance.LeftParty();
    }

    public void Spawn(PlayerCharacter _player)
    {
        InGamePlayers[_player] = true;
        TDS_UIManager.Instance.ActiveMenu(false);
        TDS_UIManager.Instance.SetMainPlayer(_player);

        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("AddPlayer", PhotonTargets.Others, (int)_player);
        TDS_Networking.Instance.Spawn(_player);

    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);

        InGamePlayers.Add(PlayerCharacter.BeardLady, false);
        InGamePlayers.Add(PlayerCharacter.FatLady, false);
        InGamePlayers.Add(PlayerCharacter.FireEater, false);
        InGamePlayers.Add(PlayerCharacter.Juggler, false);
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    #endregion
    #endregion
}
