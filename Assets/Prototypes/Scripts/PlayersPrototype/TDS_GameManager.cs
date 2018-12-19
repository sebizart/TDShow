using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TDS_GameManager : MonoBehaviour
{
    #region Events

    #endregion

    #region Fields / Properties
    private const string SURVEY_LINK = "https://goo.gl/forms/7T8vu0IHRgpol3HK2";
    // All available player characters associated with a bool indicating if their are already in game
    public Dictionary<PlayerCharacter, bool> InGamePlayers = new Dictionary<PlayerCharacter, bool>();

    private bool isReadyToQuit = true;
    #endregion

    #region Singleton
    // The singleton instance of this class
    public static TDS_GameManager Instance = null;
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// Makes the main player leave the game
    /// </summary>
    /// <param name="_player">Player type who leave</param>
    public void LeaveParty(PlayerCharacter _player)
    {
        InGamePlayers[_player] = false;

        TDS_UIManager.Instance.LeaveParty();

        if (isReadyToQuit)
        {
            TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("RemovePlayer", PhotonTargets.Others, (int)_player);
        }
        else
        {
            StartCoroutine(Quit());
        }
    }

    /// <summary>
    /// Spawns the main player in the game
    /// </summary>
    /// <param name="_player">Player type to spawn</param>
    public void Spawn(PlayerCharacter _player)
    {
        InGamePlayers[_player] = true;
        TDS_UIManager.Instance.ActiveMenu(false);
        TDS_UIManager.Instance.SetMainPlayer(_player);

        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("AddPlayer", PhotonTargets.Others, (int)_player);
        TDS_Networking.Instance.Spawn(_player);

    }

    private bool QuitSystem()
    {
        if (isReadyToQuit && FindObjectsOfType<TDS_Player>().Where(p => p.PhotonViewElement.isMine).FirstOrDefault() != null)
        {
            isReadyToQuit = false;

            TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("RemovePlayer", PhotonTargets.All, (int)FindObjectsOfType<TDS_Player>().Where(p => p.PhotonViewElement.isMine).First().Character);

            return false;
        }

        if (isReadyToQuit)
        {
            isReadyToQuit = false;

            // DEBUG
            //System.IO.File.AppendAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Debug.txt", new string[] { "Debug|" });

            Application.OpenURL(SURVEY_LINK);

            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator Quit()
    {
        yield return new WaitForEndOfFrame();
        isReadyToQuit = true;
        Application.Quit();
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);

        if (!Application.isEditor)
        {
            Application.wantsToQuit += QuitSystem;
        }

        InGamePlayers.Add(PlayerCharacter.BeardLady, false);
        InGamePlayers.Add(PlayerCharacter.FatLady, false);
        InGamePlayers.Add(PlayerCharacter.FireEater, false);
        InGamePlayers.Add(PlayerCharacter.Juggler, false);
    }

    // Use this for initialization
    void Start ()
    {
        isReadyToQuit = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
    }
    #endregion
    #endregion
}
