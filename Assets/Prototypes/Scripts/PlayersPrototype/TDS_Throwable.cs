using Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TDS_Throwable : PunBehaviour
{
    /* TDS_Throwable :
	*
	* [Write the Description of the Script here !]
	*/

    #region Events

    #endregion

    #region Fields / Accessors
    // The attack box of this object when thrown
    [SerializeField] protected TDS_AttackBox attackBox = null;

    // Can this object be destroyed by throw ?
    [SerializeField] protected bool canBeDestroyedByThrow = false;
    // Is this object ready to be throw ?
    [SerializeField] protected bool isReadyToThrow = false;
    public bool IsReadyToThrow { get { return isReadyToThrow; } }
    // Is this object currently throwable ?
    [SerializeField] protected bool isThrowable = false;
    // Is this object thrown in the air
    [SerializeField] protected bool isThrown = false;
    // Was this throwable threw by a player, or an enemy ?
    [SerializeField] protected bool wasGrabByPlayer = false;

    // The bonus damage(s) given by the character that threw it
    [SerializeField] protected int bonusDamage = 0;
    // The photon ID of this object
    public int PhotonID { get { return photonViewElement.viewID; } }
    // The amount of remaining throw if this object before it disappear
    [SerializeField] protected int remainingThrows = 0;
    public int RemainingThrows
    {
        get { return remainingThrows; }
        protected set
        {
            value = value < 0 ? 0 : value;
            if (value == 0)
            {
                DestroyThrowable();
            }
        }
    }
    // The ID of the thrower
    [SerializeField] protected int throwerID = 0;
    // The index of the current position of this object in its trajectory
    [SerializeField] protected int trajectoryPositionIndex = 0;
    public int TrajectoryPositionIndex
    {
        get { return trajectoryPositionIndex; }
        protected set
        {
            if (value >= trajectoryPositions.Length)
            {
                isThrown = false;
                isReadyToThrow = false;
                wasGrabByPlayer = false;
            }
            trajectoryPositionIndex = value;
        }
    }

    // The side this object is facing in its trajectory
    [SerializeField] FacingSide facingSide = FacingSide.Right;

    // The speed of the object when it is throw
    [SerializeField] protected float throwSpeed = 1;

    // The Photon View of this object
    [SerializeField] protected PhotonView photonViewElement = null;

    // All positions of the trajectory of the throw
    [SerializeField] Vector3[] trajectoryPositions = new Vector3[] { };
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// Check if any target was hit by this object
    /// </summary>
    /// <returns>Returns the ID of the elements hit with their damages</returns>
    public Dictionary<int, int> CheckHit()
    {
        bool _hasHit = false;
        int _damages = 0;
        TDS_DamageableElement _target = null;
        Dictionary<int, int> _hitElements = new Dictionary<int, int>();

        // Get all colliders in the range of the object's attack box
        Collider[] _touchColliders = Physics.OverlapBox(attackBox.CenterPosition, attackBox.ExtendPosition);

        // Foreach collider, check if it is an available target, and if so it this !
        foreach (Collider _collider in _touchColliders)
        {
            if ((wasGrabByPlayer && !(_target = _collider.GetComponent<TDS_Player>())) || (_target = _collider.GetComponent<TDS_DamageableElement>()))
            {
                _damages = Random.Range(attackBox.MinDamages, attackBox.MaxDamages) + bonusDamage;

                _hitElements.Add(_target.PhotonViewElementID, _damages);

                _hasHit = true;
            }
        }

        return _hitElements;
    }

    protected virtual void DestroyThrowable()
    {
        TDS_CustomDebug.CustomDebugLog($"Destroy throwable => {name}");
    }

    public virtual bool Grab(TDS_Character _graber)
    {
        if (!isThrowable || isReadyToThrow || isThrown) return false;

        transform.position = _graber.transform.position + Vector3.up;
        transform.SetParent(_graber.transform, true);
        isReadyToThrow = true;

        if (_graber is TDS_Player)
        {
            wasGrabByPlayer = true;
        }

        return true;
    }

    public virtual void Hit()
    {
        // Decreases its remaining throws value if it can be destroyed by throw
        if (canBeDestroyedByThrow)
        {
            RemainingThrows--;
        }

        // Project back this object
        Vector3 _orientation = Vector3.zero;

        switch (facingSide)
        {
            case FacingSide.Bottom:
                _orientation = -Vector3.forward;
                break;
            case FacingSide.Left:
                _orientation = -Vector3.right;
                break;
            case FacingSide.Right:
                _orientation = Vector3.right;
                break;
            case FacingSide.Top:
                _orientation = Vector3.forward;
                break;
            default:
                break;
        }

        trajectoryPositions = new Vector3[] { transform.position - _orientation * .5f - (Vector3.up * transform.position.y) };
        trajectoryPositionIndex = 0;

        isThrown = false;
    }

    /// <summary>
    /// Initializes the trajectory of this object from scratch
    /// </summary>
    public virtual void Throw(FacingSide _facingSide)
    {
        if (!isReadyToThrow || !isThrowable) return;

        transform.SetParent(null);

        Vector3 _orientation = Vector3.zero;

        switch (_facingSide)
        {
            case FacingSide.Bottom:
                _orientation = -Vector3.forward;
                break;
            case FacingSide.Left:
                _orientation = -Vector3.right;
                break;
            case FacingSide.Right:
                _orientation = Vector3.right;
                break;
            case FacingSide.Top:
                _orientation = Vector3.forward;
                break;
            default:
                break;
        }
        facingSide = _facingSide;

        trajectoryPositions = new Vector3[] { transform.position, transform.position + (_orientation * 1.25f) + (Vector3.up * .5f),
           transform.position + (_orientation * 2f) - (Vector3.up * .5f),
           transform.position + (_orientation * 2.5f) - (Vector3.up * transform.position.y) };

        isThrown = true;
        isReadyToThrow = false;
        trajectoryPositionIndex = 0;
    }
    /// <summary>
    /// Initializes the trajectory of this object with all its travel positions
    /// </summary>
    /// <param name="_trajectoryPositions">All positions of the throw trajectory</param>
    public virtual void Throw(Vector3[] _trajectoryPositions)
    {
        if (!isReadyToThrow || !isThrowable) return;

        trajectoryPositions = _trajectoryPositions;
        isThrown = true;
        isReadyToThrow = false;
        transform.SetParent(null);
        trajectoryPositionIndex = 0;
    }

    protected virtual void Travel()
    {
        // Makes the object move towards its next trajectory point
        transform.position = Vector3.Lerp(transform.position, trajectoryPositions[trajectoryPositionIndex], Time.deltaTime * throwSpeed);

        // If the object is near its next trajectory point, increases the trajectory position index
        if (Vector3.Distance(transform.position, trajectoryPositions[trajectoryPositionIndex]) <= 0.1f)
        {
            TrajectoryPositionIndex++;
        }
    }
    #endregion

    #region Unity Methods
    private void FixedUpdate()
    {
        // If the object has been throw, travel
        if (isThrown)
        {
            Travel();
        }
    }

    void OnDrawGizmos()
    {
        if (isReadyToThrow)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position + (Vector3.up * 2), new Vector3(.1f, .1f, .1f));
        }

        // Draws each position of the current object's trajectory
        if (!isThrown || trajectoryPositions.Length == 0) return;

        Gizmos.color = Color.cyan;
        for (int _i = 0; _i < trajectoryPositions.Length - 1; _i++)
        {
            Gizmos.DrawLine(trajectoryPositions[_i], trajectoryPositions[_i + 1]);
            Gizmos.DrawSphere(trajectoryPositions[_i], .1f);
        }
        Gizmos.DrawSphere(trajectoryPositions[trajectoryPositions.Length - 1], .1f);
        Gizmos.color = Color.white;
    }
    #endregion
    #endregion
}
