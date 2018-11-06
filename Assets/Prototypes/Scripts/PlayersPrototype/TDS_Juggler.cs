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
        // Set the current throw type
        currentThrow = _throwType;

        // Set the default destination
        switch (facingSide)
        {
            case FacingSide.Bottom:
                projectileDestination = transform.position + -(Vector3.forward * 1.5f);
                break;
            case FacingSide.Left:
                projectileDestination = transform.position + -(Vector3.right * 1.5f);
                break;
            case FacingSide.Right:
                projectileDestination = transform.position + (Vector3.right * 1.5f);
                break;
            case FacingSide.Top:
                projectileDestination = transform.position + (Vector3.forward * 1.5f);
                break;
            default:
                break;
        }

        // Draws the curbe of the default destination

        // While the player keep the input down, let him prepare the object's trajectory
        while (Input.GetKey(_throwType == ThrowType.One ? attackOneKey : attackTwoKey))
        {
            // Get the horizontal & vertical movement
            float _horizontal = Input.GetAxis("MouseX");
            float _vertical = Input.GetAxis("MouseY");

            // If the projectile destination has been changed, update it & the trajectory
            if (_horizontal != 0 || _vertical != 0)
            {
                // Set the new destination
                projectileDestination = new Vector3(projectileDestination.x + _horizontal, projectileDestination.y, projectileDestination.z + _vertical);

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

                // Draw the new curve

            }

            // Wait 0.1 second before checking the trajectory once again
            yield return new WaitForSeconds(.1f);
        }

        // If the player releases the input, throw the object
        projectile.Throw(projectileVelocity);
        projectile = null;
        yield break;
    }

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
