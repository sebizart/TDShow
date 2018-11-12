using Photon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Rigidbody))]
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
    // Can this object hit ?
    [SerializeField] protected bool canHit = true;
    // Is this object already grab ?
    [SerializeField] protected bool isGrab = false;
    public bool IsGrab { get { return isGrab; } }
    // Is this object currently throwable ?
    [SerializeField] protected bool isThrowable = false;
    // Is this object thrown in the air
    [SerializeField] protected bool isThrown = false;
    // Was this throwable threw by a player, or an enemy ?
    [SerializeField] protected bool wasGrabByPlayer = false;

    // The character who bear this object
    [SerializeField] protected TDS_Character bearer = null;

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

    // The speed of the object when it is throw
    [SerializeField] protected float throwForce = 1;

    // The Photon View of this object
    [SerializeField] protected PhotonView photonViewElement = null;

    // The rigidbody of this object
    [SerializeField] protected new Rigidbody rigidbody = null;
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

    public void Drop()
    {
        if (!isGrab) return;

        rigidbody.isKinematic = false;
        transform.SetParent(null, true);
        isGrab = false;
    }

    public virtual bool Grab(TDS_Character _bearer)
    {
        if (!isThrowable || isGrab || isThrown) return false;

        rigidbody.isKinematic = true;
        transform.position = _bearer.transform.position + Vector3.up;
        transform.SetParent(_bearer.transform, true);
        isGrab = true;

        if (_bearer is TDS_Player)
        {
            wasGrabByPlayer = true;
        }

        bearer = _bearer;

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

        canHit = false;
    }

    /// <summary>
    /// Throw the object in the direction of the facing side
    /// </summary>
    /// <param name="_facingSide">Facing side to look at</param>
    public virtual void Throw(FacingSide _facingSide)
    {
        if (!isGrab) return;

        rigidbody.isKinematic = false;
        transform.SetParent(null, true);

        Vector3 _force = Vector3.up;

        switch (_facingSide)
        {
            case FacingSide.Bottom:
                _force += -Vector3.forward;
                break;
            case FacingSide.Left:
                _force += -Vector3.right;
                break;
            case FacingSide.Right:
                _force += Vector3.right;
                break;
            case FacingSide.Top:
                _force += Vector3.forward;
                break;
            default:
                break;
        }

        rigidbody.velocity = _force * throwForce;

        isGrab = false;
        isThrown = true;
    }
    /// <summary>
    /// Throw the object in a direction with a given velocity
    /// </summary>
    /// <param name="_velocity"></param>
    public virtual void Throw(Vector3 _velocity)
    {
        if (!isGrab || !isThrowable) return;

        rigidbody.isKinematic = false;
        transform.SetParent(null, true);

        rigidbody.velocity = _velocity;

        isGrab = false;
        isThrown = true;
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isThrown && rigidbody.velocity == Vector3.zero)
        {
            isThrown = false;
            canHit = true;
        }
        if (isGrab)
        {

        }
    }

    void OnDrawGizmos()
    {
        if (isThrowable && !isThrown && !isGrab)
        {
            Gizmos.color = Color.green;
        }
        else Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + Vector3.up, new Vector3(.1f, .1f, .1f));
    }
    #endregion
    #endregion
}
