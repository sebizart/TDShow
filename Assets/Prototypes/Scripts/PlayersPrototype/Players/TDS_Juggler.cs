using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ThrowType
{
    None,
    One,
    Two
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

    // The amount of projectile(s) currently in the hands of the Juggler
    [SerializeField] public int ProjectileAmount
    {
        get { return projectiles.Count; }
    }
    // The max amount of projectiles the Juggler can take simultaneously
    [SerializeField] private int projectileMaxAmount = 3;
    // The angles of the two throw types
    [SerializeField] private int throwOneAngle = 70;
    [SerializeField] private int throwTwoAngle = 40;
    // The amount of points to draw for the projectile's trajectory preview
    [SerializeField] private int trajectoryPointsAmount = 10;

    // The line renderer used to draw the preview of the projectile's trajectory
    [SerializeField] private LineRenderer lineRenderer = null;

    // The list of current projectiles in the Juggler's hands
    [SerializeField] private List<TDS_Throwable> projectiles = new List<TDS_Throwable>();

    // The current preparing throw of the character
    [SerializeField] private ThrowType currentThrow = ThrowType.None;

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
    }

    protected override void FixedUpdate()
    {
        // If the juggler is preparing a throw, return
        if (isPreparingThrow) return;
        base.FixedUpdate();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        // Instantiate the cross and disable it
        cross = Instantiate(cross);
        cross.SetActive(false);

        if (!PhotonViewElement.isMine)
        {
            lineRenderer.material.color = new Color(lineRenderer.material.color.r, lineRenderer.material.color.g, lineRenderer.material.color.b, .25f);
        }
    }

    // Update is called once per frame
    protected override void Update ()
    {
        // If the juggler is preparing a throw, return
        if (isPreparingThrow) return;
        base.Update();
	}
    #endregion

    #region Original Methods
    protected override void InteractWithObjects()
    {
        if (!projectile)
        {
            TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("TryToGrabObject", PhotonTargets.MasterClient, PhotonViewElementID);
        }
    }

    private IEnumerator PrepareThrow(ThrowType _throwType)
    {
        // Set the boolean indicating the juggler is preparing to throw
        isPreparingThrow = true;

        // Set the current throw type
        currentThrow = _throwType;

        // Get the angle of the throw
        float _angle = _throwType == ThrowType.One ? throwOneAngle : throwTwoAngle;

        // Active the cross
        cross.SetActive(true);

        // Set the default destination
        switch (facingSide)
        {
            case FacingSide.Bottom:
                projectileDestination = new Vector3(transform.position.x, 0, transform.position.z) + -(Vector3.forward * 1.5f);
                break;
            case FacingSide.Left:
                projectileDestination = new Vector3(transform.position.x, 0, transform.position.z) + -(Vector3.right * 1.5f);
                break;
            case FacingSide.Right:
                projectileDestination = new Vector3(transform.position.x, 0, transform.position.z) + (Vector3.right * 1.5f);
                break;
            case FacingSide.Top:
                projectileDestination = new Vector3(transform.position.x, 0, transform.position.z) + (Vector3.forward * 1.5f);
                break;
            default:
                break;
        }

        // Get the velocity of the default destination
        projectileVelocity = TDS_ProjectileUtils.GetProjectileVelocityAsVector3(projectile.transform.position, projectileDestination, _angle);
        // Get the positions of projectile's trajectory for preview
        trajectoryPositions = TDS_ProjectileUtils.GetProjectileMotionPoints(projectile.transform.position, projectileDestination, projectileVelocity.magnitude, _angle, trajectoryPointsAmount);

        // While the player keep the input down, let him prepare the object's trajectory
        while (Input.GetKey(_throwType == ThrowType.One ? attackOneKey : attackTwoKey))
        {
            // Get the horizontal & vertical movement
            float _horizontal = Input.GetAxis("AimX");
            float _vertical = Input.GetAxis("AimY");

            // If the projectile destination has been changed, update it & the trajectory
            if (_horizontal != 0 || _vertical != 0)
            {
                // Set the new destination
                projectileDestination = new Vector3(projectileDestination.x + _horizontal, projectileDestination.y, projectileDestination.z + _vertical);

                projectileDestination = new Vector3(Mathf.Clamp(projectileDestination.x, transform.position.x - 5, transform.position.x + 5),
                    projectileDestination.y,
                    Mathf.Clamp(projectileDestination.z, transform.position.z - 5, transform.position.z + 5));

                // Directs the player's facing side if needed
                if (projectileDestination.x > transform.position.x && facingSide != FacingSide.Right)
                {
                    ChangeSide(FacingSide.Right);
                }
                else if (projectileDestination.x < transform.position.x && facingSide != FacingSide.Left)
                {
                    ChangeSide(FacingSide.Left);
                }
                else if (projectileDestination.z > transform.position.z && facingSide != FacingSide.Bottom)
                {
                    ChangeSide(FacingSide.Bottom);
                }
                else if (projectileDestination.z < transform.position.z && facingSide != FacingSide.Top)
                {
                    ChangeSide(FacingSide.Top);
                }

                // Get the velocity of the default destination
                projectileVelocity = TDS_ProjectileUtils.GetProjectileVelocityAsVector3(projectile.transform.position, projectileDestination, _angle);
                // Get the positions of projectile's trajectory for preview
                trajectoryPositions = TDS_ProjectileUtils.GetProjectileMotionPoints(projectile.transform.position, projectileDestination, projectileVelocity.magnitude, _angle, trajectoryPointsAmount);
            }
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
            cross.transform.position = _previewPositions.Last() + ((cross.transform.rotation * Vector3.up) * 0.001f);

            // Wait 0.1 second before checking the trajectory once again
            yield return new WaitForSeconds(.05f);
        }
        // If the player releases the input, throw the object
        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("ThrowObject", PhotonTargets.All, PhotonViewElementID, projectileVelocity);

        // Reset projectile informations
        isPreparingThrow = false;
        projectileVelocity = Vector3.zero;
        trajectoryPositions = new Vector3[] { };
        cross.SetActive(false);
        projectileDestination = Vector3.zero;
        lineRenderer.DrawTrajectory(trajectoryPositions);
        yield break;
    }

    public void ThrowObject(Vector3 _velocity)
    {
        projectile.Throw(_velocity);
        projectile = null;
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
        cross.transform.position = _previewPositions.Last() + ((cross.transform.rotation * Vector3.up) * 0.001f);
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

    #region Combat
    protected override void AirAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void AttackOne()
    {
        if (projectile)
        {
            StartCoroutine("PrepareThrow", ThrowType.One);
        }
    }

    protected override void AttackThree()
    {
        throw new System.NotImplementedException();
    }

    protected override void AttackTwo()
    {
        if (projectile)
        {
            StartCoroutine("PrepareThrow", ThrowType.Two);
        }
    }

    protected override void Catch()
    {
        throw new System.NotImplementedException();
    }

    protected override void Dodge()
    {
        throw new System.NotImplementedException();
    }

    protected override void RodeoAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void Super()
    {
        throw new System.NotImplementedException();
    }
    #endregion
    #endregion
    #endregion
}
