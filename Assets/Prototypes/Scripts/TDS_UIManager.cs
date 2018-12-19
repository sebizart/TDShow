using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TDS_UIManager : MonoBehaviour
{
    #region Events
    public event Action OnFailFireMiniGame = null;
    public event Action<FireMiniGameState> OnSetFireMiniGameState = null;
    #endregion

    #region Fields / Properties
    #region Player Selection Menu
    [Header("Menu")]
    // Indicates if the player is in the menu
    [SerializeField] private bool isInMenu = true;

    // Buttons to select a character in the menu
    [SerializeField] private Button beardLadySB, fatLadySB, fireEaterSB, jugglerSB = null;

    // Button to quit the application
    [SerializeField] private Button quittingButton; 

    // The player selection menu's parent game object
    [SerializeField] private GameObject playerSelectMenu = null;
    #endregion

    #region In-Game
    [Header("In-Game")]
    // The Ui animator
    [SerializeField] private Animator animator = null;

    // Dictionary containing in-game player's type associated with their health image
    [SerializeField]
    public Dictionary<PlayerCharacter, Image> OtherPlayersHealth = new Dictionary<PlayerCharacter, Image>();

    // Main player's health & portrait images
    [SerializeField] private Image mainPlayerH, mainPlayerP = null;

    // The prefab of the other player's UI health
    [SerializeField] private Image otherPlayerUIPrefab = null;

    // The canvas transform of the UI
    [SerializeField] private Transform canvasTransform = null;

    // The transform of the other player's health's parent
    [SerializeField] private Transform otherPlayersHParent = null;
    #endregion
    #endregion

    #region Singleton
    // The singleton instance of this class
    public static TDS_UIManager Instance = null;
    #endregion

    #region Methods
    #region Original Methods
    #region Menu
    /// <summary>
    /// Refreshes the character selection choice possibilities
    /// </summary>
    public void RefreshCharacterSelection()
    {
        bool _beardLadyEnabled = !TDS_GameManager.Instance.InGamePlayers[PlayerCharacter.BeardLady];
        bool _fatLadyEnabled = !TDS_GameManager.Instance.InGamePlayers[PlayerCharacter.FatLady];
        bool _fireEaterEnabled = !TDS_GameManager.Instance.InGamePlayers[PlayerCharacter.FireEater];
        bool _jugglerEnabled = !TDS_GameManager.Instance.InGamePlayers[PlayerCharacter.Juggler];

        beardLadySB.interactable = _beardLadyEnabled;
        fatLadySB.interactable = _fatLadyEnabled;
        fireEaterSB.interactable = _fireEaterEnabled;
        jugglerSB.interactable = _jugglerEnabled;
    }

    /// <summary>
    /// (Des)Activate the menu of character selection
    /// </summary>
    /// <param name="_isActive"></param>
    public void ActiveMenu(bool _isActive)
    {
        isInMenu = _isActive;
        playerSelectMenu.SetActive(_isActive);
    }
    #endregion

    #region In-Game
    /// <summary>
    /// Adds a player to the UI
    /// </summary>
    /// <param name="_player">Player type to add</param>
    public void AddPlayer(PlayerCharacter _player)
    {
        Color _playerColor = Color.white;

        switch (_player)
        {
            case PlayerCharacter.BeardLady:
                _playerColor = Color.blue;
                break;
            case PlayerCharacter.FatLady:
                _playerColor = Color.yellow;
                break;
            case PlayerCharacter.FireEater:
                _playerColor = Color.red;
                break;
            case PlayerCharacter.Juggler:
                _playerColor = Color.green;
                break;
            default:
                break;
        }

        Image _playerH = Instantiate(otherPlayerUIPrefab, otherPlayersHParent) as Image;
        _playerH.transform.GetChild(0).GetComponent<Image>().color = _playerColor;

        OtherPlayersHealth.Add(_player, _playerH);

        Debug.Log("Add => " + _player);

        RefreshCharacterSelection();
    }

    public void FailFireMiniGame() => OnFailFireMiniGame?.Invoke();

    /// <summary>
    /// Makes the main player leave the game (in UI)
    /// </summary>
    public void LeaveParty()
    {
        RefreshCharacterSelection();
        ActiveMenu(true);
    }

    /// <summary>
    /// Removes a player from the UI
    /// </summary>
    /// <param name="_player">Player type to remove</param>
    public void RemovePlayer(PlayerCharacter _player)
    {
        Debug.Log("Remove => " + _player);

        Destroy(OtherPlayersHealth[_player].gameObject);
        OtherPlayersHealth.Remove(_player);

        RefreshCharacterSelection();
    }

    public void SetFireMiniGame(bool _activate)
    {
        animator.SetBool("IsFireMiniGame", _activate);
    }

    public void SetFireMiniGameState(FireMiniGameState _state)
    {
        OnSetFireMiniGameState?.Invoke(_state);
    }

    /// <summary>
    /// Set the main player (for the UI)
    /// </summary>
    /// <param name="_player">Player type to set as main</param>
    public void SetMainPlayer(PlayerCharacter _player)
    {
        Color _playerColor = Color.white;

        switch (_player)
        {
            case PlayerCharacter.BeardLady:
                _playerColor = Color.blue;
                break;
            case PlayerCharacter.FatLady:
                _playerColor = Color.yellow;
                break;
            case PlayerCharacter.FireEater:
                _playerColor = Color.red;
                break;
            case PlayerCharacter.Juggler:
                _playerColor = Color.green;
                break;
            default:
                break;
        }
        mainPlayerP.color = _playerColor;
        mainPlayerH.fillAmount = 1;
    }

    /// <summary>
    /// Updates a player's UI health
    /// </summary>
    /// <param name="_player">Player to update life</param>
    /// <param name="_healthPerCent">Health of the player per cent</param>
    public void UpdatePlayerHealth(PlayerCharacter _player, float _healthPerCent)
    {
        OtherPlayersHealth[_player].fillAmount = _healthPerCent;
    }
    #endregion
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);

        if (!beardLadySB || !fatLadySB || !fireEaterSB || !jugglerSB)
        {
            TDS_CustomDebug.CustomDebugLogError("Missing player's selection button ! Self-destruction of the UIManager");
            Destroy(this);
        }

        if (!mainPlayerH || !mainPlayerP || !otherPlayersHParent)
        {
            TDS_CustomDebug.CustomDebugLogError("Missing player's health refrence ! Self-destruction of the UIManager");
            Destroy(this);
        }

        beardLadySB.onClick.AddListener(() => TDS_GameManager.Instance.Spawn(PlayerCharacter.BeardLady));
        fatLadySB.onClick.AddListener(() => TDS_GameManager.Instance.Spawn(PlayerCharacter.FatLady));
        fireEaterSB.onClick.AddListener(() => TDS_GameManager.Instance.Spawn(PlayerCharacter.FireEater));
        jugglerSB.onClick.AddListener(() => TDS_GameManager.Instance.Spawn(PlayerCharacter.Juggler));

        beardLadySB.interactable = false;
        fatLadySB.interactable = false;
        fireEaterSB.interactable = false;
        jugglerSB.interactable = false;

        if (!animator) animator = GetComponent<Animator>();

        if(quittingButton && TDS_GameManager.Instance)
        {
            quittingButton.onClick.AddListener(() => Application.Quit());
        }
    }

    private void FixedUpdate()
    {
    }

    private void Start()
    {
    }
    #endregion
    #endregion
}
