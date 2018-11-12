using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class TDS_FreakController : MonoBehaviour
{
    /* TDS_FreakController :
	*
	* [Write the Description of the Script here !]
	*/

    #region Events

    public UnityEvent OnGetOnGround = null;
    #endregion

    #region Fields / Accessors
    [Header("Bools :")]
    // Should the collider's edges middle positions be drawn
    [SerializeField] private bool doDrawColliderEdgesMiddlePositions = false;

    // Is this character on ground ?
    [SerializeField] private bool isGrounded = true;
    public bool IsGrounded { get { return isGrounded; } }

    [Header("Collider :")]
    // The collider used for collision detection
    [SerializeField] private new BoxCollider collider = null;
    public Collider Collider { get { return collider; } }

    [Header("Floats :")]
    // Speed of the movement
    [SerializeField] private float speed = 1;
    public float Speed { get { return speed; } }

    [Header("Ints :")]
    [SerializeField] private int i = 0;

    [Header("Layer Mask :")]
    // What the character should be stoped by
    [SerializeField] LayerMask whatCollides = new LayerMask();

    Vector3 closestpoint;
    Vector3 destination;

    // Colldier's edges middle positions
    public Vector3 BackColliderEdge
    {
        get { return collider.bounds.center + (Vector3.back * collider.bounds.extents.z); }
    }
    public Vector3 DownColliderEdge
    {
        get { return collider.bounds.center + (Vector3.down * collider.bounds.extents.y); }
    }
    public Vector3 ForwardColliderEdge
    {
        get { return collider.bounds.center + (Vector3.forward * collider.bounds.extents.z); }
    }
    public Vector3 LeftColliderEdge
    {
        get { return collider.bounds.center + (Vector3.left * collider.bounds.extents.x); }
    }
    public Vector3 RightColliderEdge
    {
        get { return collider.bounds.center + (Vector3.right * collider.bounds.extents.x); }
    }
    public Vector3 UpColliderEdge
    {
        get { return collider.bounds.center + (Vector3.up * collider.bounds.extents.y); }
    }
    #endregion

    #region Methods
    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // If there is no collider reference, get the first one on the object
        if (!collider) collider = GetComponent<BoxCollider>();
    }

    private void OnDrawGizmos()
    {
        // Draws the collider's edges middle positions
        if (doDrawColliderEdgesMiddlePositions && collider)
        {
            Gizmos.color = new Color(0, 1, 0, .5f);
            Gizmos.DrawSphere(BackColliderEdge, .025f);
            Gizmos.DrawSphere(DownColliderEdge, .025f);
            Gizmos.DrawSphere(ForwardColliderEdge, .025f);
            Gizmos.DrawSphere(LeftColliderEdge, .025f);
            Gizmos.DrawSphere(RightColliderEdge, .025f);
            Gizmos.DrawSphere(UpColliderEdge, .025f);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(closestpoint, .05f);

        Gizmos.DrawWireCube(destination + collider.center, collider.size);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(destination, .05f);
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), true);
	}
    #endregion
	
    #region Original Methods
    /// <summary>
    /// Moves the character to a new position
    /// </summary>
    /// <param name="_position">New position to move to</param>
    /// <param name="_isDirection">Should the given position be used as a direction from the actual position, or as world new position</param>
	public void Move(Vector3 _position, bool _isDirection = false)
    {
        // If the given position is a direction, convert it as world position
        if (_isDirection) _position = transform.position + _position;

        // Get the final position after movement
        Vector3 _newPosition = Vector3.Lerp(transform.position, _position, Time.deltaTime * speed);

        destination = _newPosition;

        // If the final position is blocked by something, do not move
        if (Physics.OverlapBox(new Vector3(_newPosition.x, transform.position.y, transform.position.z) + collider.center, collider.size / 2, Quaternion.identity, whatCollides).Length > 0)
        {
            Debug.Log("No movement posible in X !");
            _newPosition = new Vector3(transform.position.x, _newPosition.y, _newPosition.z);
        }
        if (Physics.OverlapBox(new Vector3(transform.position.x, _newPosition.y, transform.position.z) + collider.center, collider.size / 2, Quaternion.identity, whatCollides).Length > 0)
        {
            Debug.Log("No movement posible in Y !");
            _newPosition = new Vector3(_newPosition.x, transform.position.y, _newPosition.z);
        }
        if (Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y, _newPosition.z) + collider.center, collider.size / 2, Quaternion.identity, whatCollides).Length > 0)
        {
            Debug.Log("No movement posible in Z !");
            _newPosition = new Vector3(_newPosition.x, _newPosition.y, transform.position.z);
        }

        // If nothing does block the way, move to the objective
        transform.position = _newPosition;
    }
    #endregion
    #endregion
}
