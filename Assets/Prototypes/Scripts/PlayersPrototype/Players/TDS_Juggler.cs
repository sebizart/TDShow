﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ThrowType
{
    Bell,
    Linear,
    MysteryBall
}

[RequireComponent(typeof(LineRenderer))]
public class TDS_Juggler : TDS_Player
{
    /* TDS_Juggler :
    *
    * This script manages the behaviour of the Juggler, a character the player can play
    * that can juggle with up to three objects in his three hands and throw them with
    * strength & precision
    */

    #region Events

    #endregion

    #region Fields / Accessors
    #region Projectile
    // Is the juggler currently preparing a throw ?
    [SerializeField] private bool isPreparingThrow = false;

    // Cross indicating the end of a projectile's trajectory
    [SerializeField] private GameObject cross = null;
    // The mystery ball attack projectile prefab
    [SerializeField] private TDS_MysteryBall mysteryBall = null;

    // The amount of projectile(s) currently in the hands of the Juggler
    [SerializeField] public int ProjectileAmount
    {
        get { return projectiles.Count; }
    }
    // The max amount of projectiles the Juggler can take simultaneously
    [SerializeField] private int projectileMaxAmount = 3;
    // The angles of the different throw types
    [SerializeField] private int BellThrowAngle = 70;
    [SerializeField] private int LinearThrowAngle = 25;
    [SerializeField] private int MysteryBallThrowAngle = 45;
    // The amount of points to draw for the projectile's trajectory preview
    [SerializeField] private int trajectoryPointsAmount = 10;
    // The index of the selected projectile
    [SerializeField] private int selectedProjectileIndex = 0;
    [SerializeField]
    public int SelectedProjectileIndex
    {
        get { return selectedProjectileIndex; }
        private set
        {
            if (value > ProjectileAmount - 1) value = 0;
            else if (value < 0) value = ProjectileAmount - 1;

            switch (ProjectileAmount)
            {
                case 1:
                    projectiles[0].transform.position = transform.position + Vector3.up * 1.5f;
                    break;
                case 2:
                    projectiles[value].transform.position = transform.position + Vector3.up * 1.5f;

                    projectiles[value == 0 ? 1 : 0].transform.position = transform.position + Vector3.up * 2f + Vector3.left;
                    break;
                case 3:
                    projectiles[value].transform.position = transform.position + Vector3.up * 1.5f;

                    projectiles[value == 0 ? 1 : value == 1 ? 2 : 0].transform.position = transform.position + Vector3.up * 2f + Vector3.left;

                    projectiles[value == 0 ? 2 : value == 1 ? 0 : 1].transform.position = transform.position + Vector3.up * 2f + Vector3.right;
                    break;
                default:
                    break;
            }

            selectedProjectileIndex = value;
        }
    }

    // The timer for dropping object
    private float dropObjectTimer = 0;

    // The time of the slap action
    [SerializeField] private float slapTime = .75f;

    // The line renderer used to draw the preview of the projectile's trajectory
    [SerializeField] private LineRenderer lineRenderer = null;

    // The list of current projectiles in the Juggler's hands
    [SerializeField] private List<TDS_Throwable> projectiles = new List<TDS_Throwable>();

    // The current preparing throw of the character
    [SerializeField] private ThrowType currentThrow = ThrowType.Bell;

    // Destination of the projectile
    [SerializeField] private Vector3 projectileDestination = Vector3.zero;
    // The velocity calculated to throw the object
    [SerializeField] private Vector3 projectileVelocity = Vector3.one;
    // The positions of the projectile's trajectory
    [SerializeField] private Vector3[] trajectoryPositions = new Vector3[] { };
    #endregion
    #endregion

    #region Methods
    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        character = PlayerCharacter.Juggler;

        InputsManager.OnDpadxButton += ChangeProjectile;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void OnDestroy()
    {
        InputsManager.OnDpadxButton -= ChangeProjectile;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (isCatching)
        {
            if (attackBox.Collider != null)
            {
                Gizmos.color = attackBox.BoxColor;
                Gizmos.DrawCube(transform.TransformPoint(attackBox.Collider.center), attackBox.Collider.size);
                Gizmos.color = Color.white;
            }
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        // Instantiate the cross and disable it
        if (cross.scene != gameObject.scene)
        {
            cross = Instantiate(cross, transform, true);
        }
        cross.SetActive(false);

        if (!PhotonViewElement.isMine)
        {
            lineRenderer.material.color = new Color(lineRenderer.material.color.r, lineRenderer.material.color.g, lineRenderer.material.color.b, .25f);
        }
    }

    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();
	}
    #endregion

    #region Original Methods
    protected override void Actions()
    {
        // Actions verifications
        if (Input.GetButtonDown("Menu"))
        {
            DestroyCharacter();
            return;
        }

        // Get the mouse movement
        float _mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (_mouseScroll != 0) ChangeProjectile((int)_mouseScroll);

        // Get the triggers pression
        float _triggers = Input.GetAxis("Joystick Triggers");

        if ((_triggers > .5f || Input.GetButtonDown("Throw")) && !isPreparingThrow)
        {
            ThrowObject();
        }
        else if ((_triggers < -.5f || Input.GetButtonDown("Dodge")) && !isPreparingThrow)
        {
            StartCoroutine(Dodge());
            return;
        }

        else if (Input.GetButtonDown("Fire1"))
        {
            if (isGrounded)
            {
                AttackOne();
            }
            else
            {
                AirAttack();
            }
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            if (isGrounded)
            {
                AttackTwo();
            }
            else
            {
                RodeoAttack();
            }
        }
        else if (Input.GetButtonDown("Fire3"))
        {
            AttackThree();
        }

        if (isPreparingThrow) return;

        else if (Input.GetButtonDown("Alt Fire1"))
        {
            InteractWithObjects();
        }
        else
        {
            if (Input.GetButton("Alt Fire2"))
            {
                dropObjectTimer += Time.deltaTime;

                if (dropObjectTimer >= .5f)
                {
                    DropSelectedObject();
                    dropObjectTimer = 0;
                }
            }

            else if (Input.GetButtonDown("Alt Fire1"))
            {
                Catch();
            }
        }

        controller.Jump("Jump", true);
    }

    protected override IEnumerator Catching()
    {
        float _originalSpeed = speed;
        Vector3 _backMovement = Vector3.zero;

        isCatching = true;

        switch (facingSide)
        {
            case FacingSide.Face:
                _backMovement = Vector3.forward;
                break;
            case FacingSide.Left:
                _backMovement = Vector3.right;
                break;
            case FacingSide.Right:
                _backMovement = -Vector3.right;
                break;
            case FacingSide.Back:
                _backMovement = -Vector3.forward;
                break;
            default:
                break;
        }

        speed = 1;

        float _timer = 0;

        if (PhotonNetwork.isMasterClient)
        {
            while (_timer < slapTime)
            {
                TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("ApplyInfoDamages", PhotonTargets.All, TDS_RPCManager.Instance.SetInfoDamages(CheckHit(3, 5), PhotonViewElementID));

                SetDestination(_backMovement);

                yield return new WaitForEndOfFrame();

                _timer += Time.deltaTime;
            }
        }
        else
        {
            while (_timer < slapTime)
            {
                SetDestination(_backMovement);

                yield return new WaitForEndOfFrame();

                _timer += Time.deltaTime;
            }
        }
        speed = _originalSpeed;
        isCatching = false;
    }

    private void ChangeProjectile(int _increase)
    {
        if (_increase != 0) SelectedProjectileIndex += _increase;
    }

    /// <summary>
    /// Drop all grabbed object
    /// </summary>
    public override void DropObject()
    {
        // If the character doesn't have a projectile to drop, return
        if (ProjectileAmount <= 0) return;

        // Drop the projectile and remove it from the current weared object
        projectiles.ForEach(p => p.Drop());
        projectiles.Clear();
    }

    /// <summary>
    /// Drop the selected object
    /// </summary>
    public void DropSelectedObject()
    {
        // If the character doesn't have a projectile to drop, return
        if (ProjectileAmount <= 0) return;

        // Drop the projectile and remove it from the current weared object
        projectiles[SelectedProjectileIndex].Drop();
        projectiles.RemoveAt(SelectedProjectileIndex);
    }

    /// <summary>
    /// Grabs an object
    /// </summary>
    /// <param name="_objectID">ID of the object to grab</param>
    public override void GrabObject(int _objectID)
    {
        // If the character already have a projectile, return
        if (ProjectileAmount >= projectileMaxAmount) return;

        // Find the object via photon ID & get its Throwable component
        PhotonView _objectPhoton = PhotonView.Find(_objectID);

        if (_objectPhoton)
        {
            TDS_Throwable _projectile = _objectPhoton.GetComponent<TDS_Throwable>();

            if (!_projectile)
            {
                TDS_CustomDebug.CustomDebugLogWarning($"The object \"{_objectPhoton.name}\" couldn't be found");
                return;
            }

            // If the character can grab the object, stock it
            if (_projectile.Grab(this))
            {
                projectiles.Add(_projectile);
                SelectedProjectileIndex = ProjectileAmount - 1;
            }
        }
        else
        {
            TDS_CustomDebug.CustomDebugLogWarning($"The object with PhotonID \"{_objectID}\" doesn't have the \"TDS_Throwable\" component !");
            return;
        }
    }

    protected override void InteractWithObjects()
    {
        if (ProjectileAmount < projectileMaxAmount)
        {
            TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("TryToGrabObject", PhotonTargets.MasterClient, PhotonViewElementID);
        }
    }

    private IEnumerator PrepareThrow()
    {
        // Set the boolean indicating the juggler is preparing to throw
        isPreparingThrow = true;

        // Get the angle of the throw
        float _angle = 0;
        switch (currentThrow)
        {
            case ThrowType.Bell:
                _angle = BellThrowAngle;
                break;
            case ThrowType.Linear:
                _angle = LinearThrowAngle;
                break;
            case ThrowType.MysteryBall:
                _angle = MysteryBallThrowAngle;
                break;
            default:
                break;
        }

        // Active the cross
        cross.SetActive(true);

        // Set the default destination
        switch (facingSide)
        {
            case FacingSide.Face:
                projectileDestination = new Vector3(transform.position.x, 0, transform.position.z) - (Vector3.forward * 2.5f);
                break;
            case FacingSide.Left:
                projectileDestination = new Vector3(transform.position.x, 0, transform.position.z) - (Vector3.right * 2.5f);
                break;
            case FacingSide.Right:
                projectileDestination = new Vector3(transform.position.x, 0, transform.position.z) + (Vector3.right * 2.5f);
                break;
            case FacingSide.Back:
                projectileDestination = new Vector3(transform.position.x, 0, transform.position.z) + (Vector3.forward * 2.5f);
                break;
            default:
                break;
        }

        // Get the velocity of the default destination
        projectileVelocity = TDS_ProjectileUtils.GetProjectileVelocityAsVector3(ProjectileAmount - 1 >= selectedProjectileIndex ? projectiles[selectedProjectileIndex].transform.position : transform.position, projectileDestination, _angle);
        // Get the positions of projectile's trajectory for preview
        trajectoryPositions = TDS_ProjectileUtils.GetProjectileMotionPoints(ProjectileAmount - 1 >= selectedProjectileIndex ? projectiles[selectedProjectileIndex].transform.position : transform.position, projectileDestination, projectileVelocity.magnitude, _angle, trajectoryPointsAmount);

        // While the player keep the input down, let him prepare the object's trajectory
        while (Input.GetAxis("Joystick Triggers") > .5f || Input.GetButton("Throw"))
        {
            // If the juggler doesn't have projectile & is not preparing a mystery ball, he cannot throw anything, so cancel the throw
            if (currentThrow != ThrowType.MysteryBall && ProjectileAmount <= 0) break;

            // Get the horizontal & vertical movement
            float _lookX = Input.GetAxis("Look X");
            float _lookY = Input.GetAxis("Look Y");

            // Set the destination
            projectileDestination = new Vector3(projectileDestination.x + _lookX, projectileDestination.y, projectileDestination.z + _lookY);

            projectileDestination = new Vector3(Mathf.Clamp(projectileDestination.x, transform.position.x - 5, transform.position.x + 5),
                projectileDestination.y,
                Mathf.Clamp(projectileDestination.z, transform.position.z - 5, transform.position.z + 5));

            // Directs the player's facing side if needed
            if (projectileDestination.x > transform.position.x)
            {
                if (facingSide != FacingSide.Right) ChangeSide(FacingSide.Right);
            }
            else if (projectileDestination.x < transform.position.x)
            {
                if (facingSide != FacingSide.Left) ChangeSide(FacingSide.Left);
            }
            else if (projectileDestination.z < transform.position.z)
            {
                if (facingSide != FacingSide.Face) ChangeSide(FacingSide.Face);
            }
            else if (projectileDestination.z > transform.position.z)
            {
                if (facingSide != FacingSide.Back) ChangeSide(FacingSide.Back);
            }

            // Refresh the throw angle
            switch (currentThrow)
            {
                case ThrowType.Bell:
                    _angle = BellThrowAngle;
                    break;
                case ThrowType.Linear:
                    _angle = LinearThrowAngle;
                    break;
                case ThrowType.MysteryBall:
                    _angle = MysteryBallThrowAngle;
                    break;
                default:
                    break;
            }

            // Get the velocity of the default destination
            projectileVelocity = TDS_ProjectileUtils.GetProjectileVelocityAsVector3(currentThrow == ThrowType.MysteryBall ? transform.position : projectiles[selectedProjectileIndex].transform.position, projectileDestination, _angle);
            // Get the positions of projectile's trajectory for preview
            trajectoryPositions = TDS_ProjectileUtils.GetProjectileMotionPoints(currentThrow == ThrowType.MysteryBall ? transform.position : projectiles[selectedProjectileIndex].transform.position, projectileDestination, projectileVelocity.magnitude, _angle, trajectoryPointsAmount);

            // Positions to send to draw the trajectory's preview
            Vector3[] _previewPositions = trajectoryPositions;
            // The raycast hit indicating where the obstacle is
            RaycastHit _raycastHit = new RaycastHit();

            // Raycast to know if there is an obstacle on the projectile's trajectory
            for (int _i = 0; _i < trajectoryPositions.Length - 1; _i++)
            {
                // If something was hit, set this point as end of the trajectory
                if (Physics.Linecast(trajectoryPositions[_i], trajectoryPositions[_i + 1], out _raycastHit, controller.WhatIsObstacle))
                {
                    _previewPositions = new Vector3[_i + 2];
                    for (int _j = 0; _j <= _i; _j++)
                    {
                        _previewPositions[_j] = trajectoryPositions[_j];
                    }
                    _previewPositions[_i + 1] = _raycastHit.point;

                    // Set the cross orientation indicating the end of the trajectory
                    cross.transform.rotation = Quaternion.Lerp(cross.transform.rotation, Quaternion.FromToRotation(Vector3.up, _raycastHit.normal), Time.deltaTime * 50);
                    break;
                }
            }

            // Draws the curve of the destination
            lineRenderer.DrawTrajectory(_previewPositions);

            // Set the cross position indicating the end of the trajectory
            cross.transform.position = _previewPositions.Last() + ((cross.transform.rotation * Vector3.up) * 0.001f);

            // Wait 0.1 second before checking the trajectory once again
            yield return new WaitForSeconds(.01f);
        }
        if (currentThrow == ThrowType.MysteryBall)
        {
            // Throws a mystery ball
            TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("ThrowMysteryBall", PhotonTargets.All, PhotonViewElementID, projectileVelocity);
        }
        else if (ProjectileAmount > 0)
        {
            // Throws the projectile
            TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("ThrowObject", PhotonTargets.All, PhotonViewElementID, projectileVelocity);
        }

        // Reset projectile informations
        isPreparingThrow = false;
        projectileVelocity = Vector3.zero;
        trajectoryPositions = new Vector3[] { };
        cross.SetActive(false);
        projectileDestination = Vector3.zero;
        lineRenderer.DrawTrajectory(trajectoryPositions);
        yield break;
    }

    public void ThrowMysteryBall(Vector3 _velocity)
    {
        Instantiate(mysteryBall, transform.position, Quaternion.Euler(Camera.main.transform.localEulerAngles.x, 0, 0)).Init(_velocity, Random.Range(2, 4));
    }

    public void ThrowObject(Vector3 _velocity)
    {
        projectiles[SelectedProjectileIndex].Throw(_velocity, Random.Range(3, 5));
        projectiles.RemoveAt(SelectedProjectileIndex);

        SelectedProjectileIndex = 0;
    }

    #region On Photon Serialize View
    private void DrawThrowPreview(string _positions)
    {
        // Activate the cross
        cross.SetActive(true);

        // Get the trajectory positions
        trajectoryPositions = _positions.Split('|').ToList().Select(p => new Vector3(float.Parse(p.Split('#')[0]), float.Parse(p.Split('#')[1]), float.Parse(p.Split('#')[2]))).ToArray();

        // Positions to send to draw the trajectory's preview
        Vector3[] _previewPositions = trajectoryPositions;
        // The raycast hit indicating where the obstacle is
        RaycastHit _raycastHit = new RaycastHit();

        // Raycast to know if there is an obstacle on the projectile's trajectory
        for (int _i = 0; _i < trajectoryPositions.Length - 1; _i++)
        {
            // If something was hit, set this point as end of the trajectory
            if (Physics.Linecast(trajectoryPositions[_i], trajectoryPositions[_i + 1], out _raycastHit))
            {
                _previewPositions = new Vector3[_i + 2];
                for (int _j = 0; _j <= _i; _j++)
                {
                    _previewPositions[_j] = trajectoryPositions[_j];
                }
                _previewPositions[_i + 1] = _raycastHit.point;

                // Set the cross orientation indicating the end of the trajectory
                cross.transform.rotation = Quaternion.Lerp(cross.transform.rotation, Quaternion.FromToRotation(Vector3.up, _raycastHit.normal), Time.deltaTime * 50);
                break;
            }
        }

        // Draws the curve of the destination
        lineRenderer.DrawTrajectory(_previewPositions);

        // Set the cross position indicating the end of the trajectory
        cross.transform.position = Vector3.Lerp(cross.transform.position, _previewPositions.Last() + ((cross.transform.rotation * Vector3.up) * 0.001f), Time.deltaTime * 25);
    }

    protected override void OnPhotonSerializeView(PhotonStream _stream, PhotonMessageInfo _messageInfo)
    {
        base.OnPhotonSerializeView(_stream, _messageInfo);

        if (_stream.isWriting)
        {
            // Send all preview projectile positions
            string _positions = string.Empty;
            for (int _i = 0; _i < trajectoryPositions.Length; _i++)
            {
                _positions += $"{trajectoryPositions[_i].x}#{trajectoryPositions[_i].y}#{trajectoryPositions[_i].z}";

                if (_i < trajectoryPositions.Length - 1)
                {
                    _positions += '|';
                }
            }
            _stream.SendNext(_positions);
        }
        else if (_stream.isReading)
        {
            // Get the preview projectile positions, and draw them if it's not null
            string _positions = (string)_stream.ReceiveNext();
            if (_positions != string.Empty)
            {
                DrawThrowPreview(_positions);
            }
            else
            {
                // Desctivate the cross & draw a null trajectory
                cross.SetActive(false);
                lineRenderer.DrawTrajectory(new Vector3[] { });
            }
        }
    }
    #endregion

    #region Actions
    protected override void AirAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void AttackOne()
    {
        currentThrow = ThrowType.Bell;
    }

    protected override void AttackThree()
    {
        currentThrow = ThrowType.MysteryBall;
    }

    protected override void AttackTwo()
    {
        currentThrow = ThrowType.Linear;
    }

    protected override void Catch()
    {
        base.Catch();
    }

    protected override void RodeoAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void Super()
    {
        throw new System.NotImplementedException();
    }

    public override void ThrowObject()
    {
        if (ProjectileAmount > 0 || currentThrow == ThrowType.MysteryBall)
        {
            StartCoroutine("PrepareThrow");
        }
    }
    #endregion
    #endregion
    #endregion
}
