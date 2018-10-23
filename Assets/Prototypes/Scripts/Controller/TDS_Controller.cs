using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/*
[Script Header] CATA_Controller Version 0.0.1
Created by: Lucas Guibert
Date: 05 / 10 / 2018
Description: Controls the movements and actions of the Player
This object manages the inputs of the player and controls the in-game avatar

********** Networking **********

Send & Receive :
    - transform.position
    - transform.scale.x

///
[UPDATES]
Update n°: 1
Updated by: Lucas
Date: 08 / 10 / 2018
Description:
To know if an element is seen by the camera :
Camera.WorldToViewportPoint :
If both the x and y coordinate of the returned point is between 0 and 1 (and the z coordinate
is positive), then the point is seen by the camera.

To use. Really. For the out of screen infos. Someday...
*/

public class TDS_Controller : TDS_GroundedElement
{
    #region Fields / Properties
    #region Movements
    // Indicates if the character is facing right or left
    [SerializeField] private bool isFacingRight = true;

    // The X & Z axis movements
    [SerializeField] private float xMovement, zMovement = 0;
    public float XMovement
    {
        get { return xMovement; }
    }
    public float ZMovement
    {
        get { return zMovement; }
    }

    // The speed of the player
    [SerializeField] private float speed = 10;

    // The NavMesh of the player
    [SerializeField] private NavMeshAgent agent = null;

    // The renderer of this gameobject
    [SerializeField] private new Renderer renderer = null;

    // Is this object visible or not
    [SerializeField] private bool isVisible = true;
    #endregion

    #region Combat
    // Can the player attack ?
    [SerializeField] private bool canAttack = true;

    // The max combo
    [SerializeField] private int comboMax = 3;
    // The actual combo
    [SerializeField] private int comboValue = 1;
    public int ComboValue
    {
        get { return comboValue; }
        private set
        {
            if (value > comboMax)
            {
                value = 1;
            }

            comboValue = value;
        }
    }

    // The max & min amount of damages when hitting
    [SerializeField] private int damagesMin = 1;
    [SerializeField] private int damagesMax = 2;

    // The damages dealt when projecting
    [SerializeField] private int damagesProjection = 1;

    // The time it takes to reset the combo when the player is not hitting
    [SerializeField] private float comboResetTime = 1;

    // The size of the combat's gizmo debug sphere
    [SerializeField] private float gogoPowerRangers = .25f;

    // The cool down time between two hits
    [SerializeField] private float hitCoolDown = .5f;
    // The timer for attack cool down & combo reset
    [SerializeField] private float hitComboTimer = 0;

    // The coroutine used to reset the combo
    [SerializeField] private Coroutine resetComboCoroutine = null;

    // All attack detection boxes of this player
    [SerializeField] private TDS_OLDAttackBox[] attackBoxes = new TDS_OLDAttackBox[] { };

    // The sprite of the controller
    [SerializeField] private SpriteRenderer sprite = null;
    #endregion

    #region Key Codes
    // The key code for hitting
    [SerializeField] private KeyCode hitKey = KeyCode.Mouse0;
    // The key code for projecting
    [SerializeField] private KeyCode projectKey = KeyCode.Mouse1;
    #endregion

    #region Network
    // The photon ID of the object
    [SerializeField] private PhotonView photonID;

    // The position of the online character
    [SerializeField] private float xPositionOnline, yPositionOnline, zPositionOnline, xScaleOnline = 0;
    #endregion
    #endregion

    #region Methods
    // Call this when a new combo is started to reset it if the player do not hit for a certain time
    private IEnumerator ResetComboTimer()
    {
        // While the timer if superior to 0, decreases it if the player do not hit or reset it if he does
        while (hitComboTimer < comboResetTime)
        {
            hitComboTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Reset the combo value
        comboValue = 1;
        hitComboTimer = 0;
    }

    // Call this to set the cool down between two attacks
    private IEnumerator SetAttackCoolDown()
    {
        // Stops the reset timer coroutine
        if (resetComboCoroutine != null)
        {
            StopCoroutine(resetComboCoroutine);
        }

        canAttack = false;

        // Initializes the value of the timer
        hitComboTimer = 0;

        // While the timer if superior to 0, decreases it
        while (hitComboTimer < hitCoolDown)
        {
            hitComboTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        canAttack = true;

        // Starts the reset timer coroutine if a combo is started
        if (comboValue != 1)
        {
            resetComboCoroutine = StartCoroutine(ResetComboTimer());
        }
        else
        {
            // Reset the timer value
            hitComboTimer = 0;
        }
    }

    // Flips the character's scale on the X axis
    private void FlipX()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        isFacingRight = !isFacingRight;
    }

    // Hit all interactibles elements in range
    public void Hit(TDS_Enemy[] _enemies, int _damages)
    {
        // Damages each enemy
        foreach (TDS_Enemy _enemy in _enemies)
        {
            _enemy.TakeDamages(_damages);
        }

        // Show the combo value
        (Instantiate((GameObject)Resources.Load("DamageBehaviour"), transform.position + Vector3.up + (Vector3.right * 0.5f), Quaternion.identity)).GetComponent<TDS_DamageBehaviour>().Init("x" + comboValue);

        // Increases the combo status
        ComboValue++;

        // Set the cool down between two attacks
        StartCoroutine(SetAttackCoolDown());
    }
    // Check what this player can hit
    public void HitCheck()
    {
        // If the player can't attack, return
        if (!canAttack) return;

        // Raycast in the box of the attack
        TDS_Enemy[] _enemies = AttackRaycast("Melee Box");

        if (_enemies.Length == 0) return;

        int _damages = Mathf.Clamp(Random.Range(1, comboValue + 1), damagesMin, damagesMax + 1);

        // Send to the Player Manager informations about the enemies hit
        string _enemiesAndDamages = string.Empty;
        for (int _i = 0; _i < _enemies.Length; _i++)
        {
            _enemiesAndDamages += _enemies[_i].photonView.viewID;
            if (_i != _enemies.Length - 1) _enemiesAndDamages += ",";
            else
            {
                _enemiesAndDamages += "-" + _damages;
            }
        }
        TDS_PlayerRPCManager.Instance.Hit(_enemiesAndDamages);

        // Now, hit them
        Hit(_enemies, _damages);
    }

    // Get the inputs of the player and executes related actions in-game
    private void Inputs()
    {
        // Get the movements of the player
        xMovement = Input.GetAxis("Horizontal");
        zMovement = Input.GetAxis("Vertical");

        // Flips the character if needed
        if ((xMovement < 0 && isFacingRight) || (xMovement > 0 && !isFacingRight))
        {
            FlipX();
        }

        // Moves the player
        transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(xMovement, 0, zMovement), Time.deltaTime * speed);

        // ********** COMBAT ********** //

        // If the player wants to hit something / someone
        if (Input.GetKeyDown(hitKey))
        {
            if (PhotonNetwork.isMasterClient)
            {
                HitCheck();
            }
            else
            {
                TDS_PlayerRPCManager.Instance.HitCheck(photonID.viewID);
            }
        }
        // If the player wants to project something / soomeone
        else if (Input.GetKeyDown(projectKey))
        {
            if (PhotonNetwork.isMasterClient)
            {
                ProjectCheck();
            }
            else
            {
                TDS_PlayerRPCManager.Instance.ProjectCheck(photonID.viewID);
            }
        }
    }

    // Project one interactible element (nearest) in range
    public void Project(TDS_Enemy _enemy)
    {
        _enemy.SetProjection(damagesProjection, transform);

        // Set the cool down between two attacks
        StartCoroutine(SetAttackCoolDown());

        // Reset the combo
        ComboValue = 1;
    }
    // Check what this player can project
    public void ProjectCheck()
    {
        // If the player can't attack, return
        if (!canAttack) return;

        // Raycast in the box of the attack
        TDS_Enemy[] _interactibles = AttackRaycast("Project Box");

        if (_interactibles.Length == 0) return;

        // Select the nearest interactable element and project it
        TDS_Enemy _enemy = (_interactibles.OrderBy(i => Vector3.Distance(transform.position, i.transform.position)).First());

        string _enemyDamagesAndTransform = _enemy.photonView.viewID.ToString() + "," + damagesProjection + "," + photonView.viewID;
        TDS_PlayerRPCManager.Instance.Project(_enemyDamagesAndTransform);

        Project(_enemy);
    }

    // Raycast in a given box and return encounter colliders
    private TDS_Enemy[] AttackRaycast(string _boxName)
    {
        // Get the right box
        TDS_OLDAttackBox _box = attackBoxes.Where(b => b.Name.ToLower() == _boxName.ToLower()).FirstOrDefault();

        // if the box wasn't founded, debug it and return
        if (_box == null)
        {
            TDS_CustomDebug.CustomDebugLogError("The attack box '" + _boxName + "' couldn't be found !");
            return new TDS_Enemy[] { };
        }
        else
        {
            // Else, return the result of the raycast in the box
            return Physics.OverlapBox(transform.position + _box.CenterPosition, _box.HalfExtents).Select(c => c.GetComponent<TDS_Enemy>()).ToArray().Where(e => e != null).ToArray();
        }
    }
    #endregion

    #region Networking Methods
    // Is called by thePhotonView when it wants it
    void OnPhotonSerializeView(PhotonStream _stream, PhotonMessageInfo _info)
    {
        // If it's writing, send informations
        if (_stream.isWriting)
        {
            _stream.SendNext(transform.position.x);
            _stream.SendNext(transform.position.y);
            _stream.SendNext(transform.position.z);
            _stream.SendNext(transform.localScale.x);
        }
        // Else, receive them
        else
        {
            xPositionOnline = (float)_stream.ReceiveNext();
            yPositionOnline = (float)_stream.ReceiveNext();
            zPositionOnline = (float)_stream.ReceiveNext();
            xScaleOnline = (float)_stream.ReceiveNext();
        }
    }

    // Set the online character
    void SetOnline()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(xPositionOnline, yPositionOnline, zPositionOnline), Time.deltaTime * speed);
        transform.localScale = new Vector3(xScaleOnline, transform.localScale.y, transform.localScale.z);
    }
    #endregion

    #region UnityMethods
    private void Awake()
    {
        // Get the PhotonView component
        if (!photonID)
        {
            photonID = GetComponent<PhotonView>();
        }

        // If we are online and this is not my instance of controller, do not set it as Instance
        if (photonID && !photonID.isMine)
        {
            return;
        }

        // If there is a camera singleton, set its player
        if (TDS_Camera.Instance)
        {
            TDS_Camera.Instance.SetPlayer(this);
        }

        // Get the sprite renderer of the object if there is none
        if (!sprite) sprite = GetComponent<SpriteRenderer>();
    }

    private void OnDrawGizmos()
    {
        // If a combo is in progress or the player can't attack :
        if (hitComboTimer != 0)
        {
            Vector3 _position = transform.position + (Vector3.up * 2f);

            // Draws a gizmo indicating the reset combo timer if a combo is started
            if (comboValue > 1)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(_position, gogoPowerRangers);
            }
            // If the player can't attack, draws a gizmo indicating the cool down
            if (!canAttack)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(_position, gogoPowerRangers * (hitCoolDown / comboResetTime));
            }
            Gizmos.color = canAttack ? Color.green : Color.red;
            Gizmos.DrawSphere(_position, gogoPowerRangers * (hitComboTimer / comboResetTime));
        }

        // Draw each box that is visible
        foreach (TDS_OLDAttackBox _box in attackBoxes)
        {
            if (_box.isVisible)
            {
                Gizmos.color = _box.Color;
                Gizmos.DrawCube(transform.position + _box.CenterPosition, _box.HalfExtents);
                Debug.Log("In");
            }
        }

        // Reset the color of the gizmos
        Gizmos.color = Color.white;
    }

    void Start () 
	{
		
	}
	
	void Update ()
    {
        if (photonID.isMine)
        {
            // Executes the player's inputs
            Inputs();
            SnapToGround(); 
        }
        else
        {
            // Set the online character
            SetOnline();
        }
    }
    #endregion
}
