using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TDS_UIManager : MonoBehaviour
{
    #region Events

    #endregion

    #region Fields / Properties
    #region Player Selection Menu
    [Header("Menu")]
    // Indicates if the player is in the menu
    [SerializeField] private bool isInMenu = true;

    // Buttons to select a character in the menu
    [SerializeField] private Button beardLadySB, fatLadySB, fireEaterSB, jugglerSB = null;

    // The player selection menu's parent game object
    [SerializeField] private GameObject playerSelectMenu = null;
    #endregion

    #region In-Game
    [Header("In-Game")]
    // Dictionary containing in-game player's type associated with their health image
    [SerializeField] private Dictionary<PlayerCharacter, Image> playersHealth = new Dictionary<PlayerCharacter, Image>();

    // All player's health images
    [SerializeField] private Image mainPlayerH, secondPlayerH, thirdPlayerH, fourthPlayerH = null;

    // All player's portrait
    [SerializeField] private Image mainPlayerP, secondPlayerP, thirdPlayerP, fourthPlayerP = null;

    // The canvas transform of the UI
    [SerializeField] private Transform canvasTransform = null;
    #endregion
    #endregion

    #region Singleton
    // The singleton instance of this class
    public static TDS_UIManager Instance = null;
    #endregion

    #region Methods
    #region Original Methods
    #region Menu
    private void RefreshCharacterSelection()
    {
        bool _beardLadyEnabled = !TDS_GameManager.Instance.InGamePlayers[PlayerCharacter.BeardLady];
        bool _fatLadyEnabled = !TDS_GameManager.Instance.InGamePlayers[PlayerCharacter.FatLady];
        bool _fireEaterEnabled = !TDS_GameManager.Instance.InGamePlayers[PlayerCharacter.FireEater];
        bool _jugglerEnabled = !TDS_GameManager.Instance.InGamePlayers[PlayerCharacter.Juggler];

        beardLadySB.enabled = _beardLadyEnabled;
        fatLadySB.enabled = _fatLadyEnabled;
        fireEaterSB.enabled = _fireEaterEnabled;
        jugglerSB.enabled = _jugglerEnabled;
    }

    public void Spawn(PlayerCharacter _player)
    {
        isInMenu = false;
        playerSelectMenu.SetActive(false);
        TDS_GameManager.Instance.InGamePlayers[_player] = true;
    }
    #endregion

    #region In-Game
    /// <summary>
    /// Adds a player to the UI
    /// </summary>
    /// <param name="_player">Player to add</param>
    public void AddPlayer(PlayerCharacter _player)
    {
        Color _playerColor = Color.white;
        Image _playerH = null;
        Image _playerP = null;

       
        if (!playersHealth.ContainsValue(secondPlayerH))
        {
            _playerH = secondPlayerH;
            _playerP = secondPlayerP;
        }
        else if (!playersHealth.ContainsValue(thirdPlayerH))
        {
            _playerH = thirdPlayerH;
            _playerP = thirdPlayerP;
        }
        else if (!playersHealth.ContainsValue(fourthPlayerH))
        {
            _playerH = fourthPlayerH;
            _playerP = fourthPlayerP;
        }

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

        _playerH.gameObject.SetActive(true);
        _playerP.color = _playerColor;
        playersHealth.Add(_player, _playerH);
    }

    /// <summary>
    /// Enables the player selection menu
    /// </summary>
    public void LeftParty(PlayerCharacter _player)
    {
        playersHealth.Remove(_player);
        playerSelectMenu.SetActive(true);
        isInMenu = true;
    }

    /// <summary>
    /// Removes a player from the UI
    /// </summary>
    /// <param name="_player">Player to remove</param>
    public void RemovePlayer(PlayerCharacter _player)
    {
        if (playersHealth[_player] == secondPlayerH && playersHealth.Count > 2)
        {
            secondPlayerH.fillAmount = thirdPlayerH.fillAmount;
            secondPlayerP.color = thirdPlayerP.color;

            if (playersHealth.Count > 3)
            {
                thirdPlayerH.fillAmount = fourthPlayerH.fillAmount;
                thirdPlayerP.color = fourthPlayerP.color;
                fourthPlayerH.gameObject.SetActive(false);
            }
            else
            {
                thirdPlayerH.gameObject.SetActive(false);
            }
        }
        else if (playersHealth[_player] == thirdPlayerH && playersHealth.Count > 3)
        {
            thirdPlayerH.fillAmount = fourthPlayerH.fillAmount;
            thirdPlayerP.color = fourthPlayerP.color;
            fourthPlayerH.gameObject.SetActive(false);
        }
        else
        {
            playersHealth[_player].gameObject.SetActive(false);
            playersHealth.Remove(_player);
        }
    }

    /// <summary>
    /// Set the main player for the UI
    /// </summary>
    /// <param name="_player">Player to set as main</param>
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
        playersHealth.Add(_player, mainPlayerH);
    }

    /// <summary>
    /// Updates a player's UI health
    /// </summary>
    /// <param name="_player">Player to update life</param>
    /// <param name="_healthPerCent">Health of the player per cent</param>
    public void UpdatePlayerHealth(PlayerCharacter _player, float _healthPerCent)
    {
        playersHealth[_player].fillAmount = _healthPerCent;
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

        if (!mainPlayerH || !secondPlayerH || !thirdPlayerH || !fourthPlayerH)
        {
            TDS_CustomDebug.CustomDebugLogError("Missing player's health image ! Self-destruction of the UIManager");
            Destroy(this);
        }

        beardLadySB.onClick.AddListener(() => TDS_GameManager.Instance.Spawn(PlayerCharacter.BeardLady));
        fatLadySB.onClick.AddListener(() => TDS_GameManager.Instance.Spawn(PlayerCharacter.FatLady));
        fireEaterSB.onClick.AddListener(() => TDS_GameManager.Instance.Spawn(PlayerCharacter.FireEater));
        jugglerSB.onClick.AddListener(() => TDS_GameManager.Instance.Spawn(PlayerCharacter.Juggler));
    }

    private void FixedUpdate()
    {
        if (isInMenu)
        {
            RefreshCharacterSelection();
        }
    }

    private void Start()
    {
    }
    #endregion
    #endregion
}
