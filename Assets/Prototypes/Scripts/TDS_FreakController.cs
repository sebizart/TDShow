using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class TDS_FreakController : MonoBehaviour
{
    /* TDS_FreakController :
	*
	* Controller initially created for player's characters in "The Dreadful Show" video game
    * 
    * Used to :
    *       - Move the character to a destination avoiding weird interactions with others colliders
    *       - Make the character jump higher or lower depending on the time the key is pressed
	*/

    #region Events
    // When starting being in air
    public UnityEvent OnGetInAir = null;
    // When hitting the ground
    public UnityEvent OnGetOnGround = null;
    // When moving
    public UnityEvent OnMove = null;
    #endregion

    #region Fields / Accessors
    [Header("Bools :")]
    // Should the collider's edges middle positions be drawn
    [SerializeField] private bool doDrawColliderEdgesMiddlePositions = false;

    // Indicates if the character is actually into some cumbersome collider
    [SerializeField] private bool isInCumbersomeCollider = false;

    // Is this character on ground ?
    [SerializeField] private bool isGrounded = true;
    public bool IsGrounded { get { return isGrounded; } }

    [Header("Collider :")]
    // The collider used for collision detection
    [SerializeField] private new BoxCollider collider = null;
    public Collider Collider { get { return collider; } }

    [Header("Floats :")]
    // Initial force of the jump
    public float JumpForce = 1;
    // Increase force of the jump
    public float JumpForceIncrease = .1f;

    // Maximum time length of the jump
    public float JumpMaxTime = 1;

    // Speed of the movement
    public float Speed = 1;

    [Header("Layer Mask :")]
    // What the character should be stoped by
    public LayerMask WhatCollides = new LayerMask();

    [Header("Rigidbody :")]
    // Rigidbody of the character
    [SerializeField] private new Rigidbody rigidbody = null;
    public Rigidbody Rigidbody { get { return rigidbody; } }

    // DEBUG
    Vector3 DEBUGdestination;

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

        // If there is no rigidbody reference, get the first one on the object
        if (!rigidbody) rigidbody = GetComponent<Rigidbody>();
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
        Gizmos.DrawWireCube(DEBUGdestination + collider.center, collider.size);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(DEBUGdestination, .05f);
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), true);

        Jump("Jump");
	}
    #endregion

    #region Original Methods
    #region Jump
    /// <summary>
    /// Makes the player jump plus or less high depending on the time the key is pressed
    /// </summary>
    /// <param name="_key">Jump's key</param>
    /// <returns></returns>
    private IEnumerator JumpCoroutine(KeyCode _key)
    {
        TDS_CustomDebug.CustomDebugLog("Start Jump !");

        // Timer used for the maximum jump time length
        float _timer = 0;

        // Adds the initial jump force to the rigidbody
        rigidbody.AddForce(Vector3.up * JumpForce);

        // While the jump key is hold and the jump is not at its maximum time length, go higher
        while (Input.GetKey(_key) && (_timer < JumpMaxTime))
        {
            // Increases the rigidbody velocity
            rigidbody.AddForce(Vector3.up * JumpForceIncrease);

            // Wait for the next frame
            yield return new WaitForEndOfFrame();

            // Increases the timer
            _timer += Time.deltaTime;
        }

        TDS_CustomDebug.CustomDebugLog("End Jump...");
    }
    /// <summary>
    /// Makes the player jump plus or less high depending on the time the key is pressed
    /// </summary>
    /// <param name="_inputName">Name of the Jump's input</param>
    /// <returns></returns>
    private IEnumerator JumpCoroutine(string _inputName)
    {
        // Timer used for the maximum jump time length
        float _timer = 0;

        // Adds the initial jump force to the rigidbody
        rigidbody.AddForce(Vector3.up * JumpForce);

        // While the jump key is hold and the jump is not at its maximum time length, go higher
        while (Input.GetButton(_inputName) && (_timer < JumpMaxTime))
        {
            // Increases the rigidbody velocity
            rigidbody.AddForce(Vector3.up * JumpForceIncrease);

            // Wait for the next frame
            yield return new WaitForEndOfFrame();

            // Increases the timer
            _timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Makes the character jump
    /// </summary>
    public void Jump()
    {
        // Makes the character jump
        StartCoroutine("JumpCoroutine");
    }
    /// <summary>
    /// Makes the character jump
    /// </summary>
    /// <param name="_shouldBeOnGround">Indicates if the character should be on ground to be able to jump</param>
    public void Jump(bool _shouldBeOnGround)
    {
        // If the character should be on ground and is not, return
        if (_shouldBeOnGround && !isGrounded) return;

        // Makes the character jump
        StartCoroutine("JumpCoroutine");
    }
    /// <summary>
    /// Makes the character jump if the key is pressed
    /// </summary>
    /// <param name="_key">Jump's key</param>
    public void Jump(KeyCode _key)
    {
        // If the Jump key is pressed, makes the character jump
        if (Input.GetKeyDown(_key))
        {
            StartCoroutine("JumpCoroutine", _key);
        }
    }
    /// <summary>
    /// Makes the character jump if the key is pressed
    /// </summary>
    /// <param name="_key">Jump's key</param>
    /// <param name="_shouldBeOnGround">Indicates if the character should be on ground to be able to jump</param>
    public void Jump(KeyCode _key, bool _shouldBeOnGround)
    {
        // If the character should be on ground and is not, return
        if (_shouldBeOnGround && !isGrounded) return;

        // If the Jump key is pressed, makes the character jump
        if (Input.GetKeyDown(_key))
        {
            StartCoroutine("JumpCoroutine", _key);
        }
    }
    /// <summary>
    /// Makes the character jump if the input is pressed
    /// </summary>
    /// <param name="_inputName">Name of the Jump's input</param>
    public void Jump(string _inputName)
    {
        // If the Jump input is pressed, makes the character jump
        if (Input.GetButtonDown(_inputName))
        {
            StartCoroutine("JumpCoroutine", _inputName);
        }
    }
    /// <summary>
    /// Makes the character jump if the input is pressed
    /// </summary>
    /// <param name="_inputName">Name of the Jump's input</param>
    /// <param name="_shouldBeOnGround">Indicates if the character should be on ground to be able to jump</param>
    public void Jump(string _inputName, bool _shouldBeOnGround)
    {
        // If the character should be on ground and is not, return
        if (_shouldBeOnGround && !isGrounded) return;

        // If the Jump input is pressed, makes the character jump
        if (Input.GetButtonDown(_inputName))
        {
            StartCoroutine("JumpCoroutine", _inputName);
        }
    }
    #endregion

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
        Vector3 _newPosition = Vector3.Lerp(transform.position, _position, Time.deltaTime * Speed);

        // DEBUG
        DEBUGdestination = _newPosition;

        // Get the overlap box's extents value
        Vector3 _overlapExtents = (collider.size / 2) - Vector3.one * 0.0001f;

        // If the character is not currently into some cumbersome collider, restrict its moves into the zone where there's no collider

        if (Physics.OverlapBox(transform.position + collider.center, _overlapExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length > 0)
        {
            // Set the boolean indicating there's a bad obstacle
            isInCumbersomeCollider = true;
        }
        // If the character is into some cumbersome collider, let it move to its destination regardless of other colliders
        else
        {
            // Set the boolean indicating there's no obstacle
            isInCumbersomeCollider = false;

            // If the final position is blocked by something, do not move
            if (Physics.OverlapBox(new Vector3(_newPosition.x, transform.position.y, transform.position.z) + collider.center, _overlapExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length > 0)
            {
                Debug.Log("No movement posible in X !");
                _newPosition = new Vector3(transform.position.x, _newPosition.y, _newPosition.z);
            }
            if (Physics.OverlapBox(new Vector3(transform.position.x, _newPosition.y, transform.position.z) + collider.center, _overlapExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length > 0)
            {
                Debug.Log("No movement posible in Y !");
                _newPosition = new Vector3(_newPosition.x, transform.position.y, _newPosition.z);
            }
            if (Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y, _newPosition.z) + collider.center, _overlapExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length > 0)
            {
                Debug.Log("No movement posible in Z !");
                _newPosition = new Vector3(_newPosition.x, _newPosition.y, transform.position.z);
            }
        }

        // If nothing does block the way, move to the objective
        transform.position = _newPosition;
    }
    #endregion
    #endregion
}
