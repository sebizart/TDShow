using System;
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
    *       
    * To do :
    *       - Increase speed gradualy when starting to move before reaching max speed
    *       - Reduce air control
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
    [Header("# Debug & Help #")]
    // Aiming destination position 
    [SerializeField] private Vector3 destinationPosition;

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
    public Vector3 GetColliderCenterPosition
    {
        get { return transform.position + collider.center; }
    }

    [Header("Floats :")]
    // Initial force of the jump
    public float JumpForce = 1;
    // Increase force of the jump
    public float JumpForceIncrease = .1f;

    // Maximum time length of the jump
    public float JumpMaxTime = 1;

    [Header("Speed :")]
    // The initial speed of the movement
    [SerializeField] private float initialSpeed = 1;
    // Maximum default speed of the movement
    [SerializeField] private float maxSpeed = 1;
    // Speed of the movement
    [SerializeField] private float speed = 1;
    public float Speed { get { return speed; } }

    [Header("Ground Box :")]
    // The box used for ground detection
    [SerializeField] private TDS_Box groundBox = new TDS_Box();
    public Vector3 GetGroundBoxCenterPosition
    {
        get { return transform.position + groundBox.Center; }
    }

    [Header("Layer Mask :")]
    // What the character should be stoped by
    public LayerMask WhatCollides = new LayerMask();

    [Header("Rigidbody :")]
    // Rigidbody of the character
    [SerializeField] private new Rigidbody rigidbody = null;
    public Rigidbody Rigidbody { get { return rigidbody; } }

    [Header("Vector3 :")]
    // The extents of the collider to use in raycast
    [SerializeField] private Vector3 colliderExtents = Vector3.one;
    public Vector3 ColliderExtents
    {
        get { return colliderExtents; }
    }
    // Previous "real" position of the character, that is the closest one to the actual where there were no bad colliders at the time
    [SerializeField] private Vector3 previousRealPosition;

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
        Gizmos.DrawWireCube(destinationPosition + collider.center, collider.size);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(destinationPosition, .05f);


    }
    private void OnDrawGizmosSelected()
    {
        // Draws the ground detection box
        Gizmos.color = groundBox.Color;
        Gizmos.DrawWireCube(GetGroundBoxCenterPosition, groundBox.Extents);
    }

    // Use this for initialization
    void Start ()
    {
        // Set the first previous position as this transform's one
        previousRealPosition = transform.position;

        // Set the value of the collider extents to use in raycast
        colliderExtents = (collider.size / 2) - Vector3.one * 0.001f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        GroundCheck();

        Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), true);

        Jump("Jump", true);
	}
    #endregion

    #region Original Methods
    /// <summary>
    /// Checks if the character is on ground or not
    /// </summary>
    private void GroundCheck()
    {
        // If something is detected in the ground box detection, the character is grounded
        if (RaycastZone(GetGroundBoxCenterPosition, groundBox.Extents))
        {
            isGrounded = true;
        }
        // If nothing was detected, the character is not grounded
        else
        {
            isGrounded = false;
        }
    }

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
            //rigidbody.AddForce(Vector3.up * JumpForceIncrease);
            rigidbody.AddForce(rigidbody.velocity * JumpForceIncrease);

            // Wait for the next frame
            yield return new WaitForEndOfFrame();

            // Increases the timer
            _timer += Time.deltaTime;
        }

        Debug.Log("End Jump...");
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
    /// <param name="_isDirection">Should the given position be used as a direction from the actual position, or as a world new position</param>
    public void Move(Vector3 _position, bool _isDirection)
    {
        // If the given position is a direction, convert it as world position
        if (_isDirection) _position = transform.position + _position;

        // Set the speed of the movement
        float _speed = speed;

        // Get the final position after movement
        Vector3 _newPosition = Vector3.Lerp(transform.position, _position, Time.deltaTime * _speed);

        // Get the position to raycast from : basically, the actual position
        Vector3 _raycastPosition = transform.position;

        // DEBUG
        destinationPosition = _newPosition;

        // Set the boolean indicating there's a bad obstacle
        isInCumbersomeCollider = false;

        // If the character is not currently into some cumbersome collider, restrict its moves into the zone where there's no collider (only in X & Z axis)
        if (RaycastZone(GetColliderCenterPosition, colliderExtents))
        {
            // Améliorer en ne raycastant que la toute petite zone qui a été franchie
            // entre la précédente position et l'actuelle

            Debug.Log("Get back ! Get back ! Get back to where you once belong !");

            // If the previous real position is clean from bad colliders, for each axis, set the raycast position as previous if the movement was not allowed
            if (!RaycastZone(previousRealPosition + collider.center, colliderExtents))
            {
                // Raycast in the 3 different axis in the zone between the previous position and the actual one
                bool[] _raycastResults = RaycastZoneOutsideBox(previousRealPosition, transform.position, colliderExtents);

                // Foreach axis were the movement was not posible, update the raycast position

                // X axis
                if (_raycastResults[0])
                {
                    _raycastPosition = new Vector3(previousRealPosition.x, _raycastPosition.y, _raycastPosition.z);

                    Debug.Log("Raycast back in X");
                }
                // Y axis
                if (_raycastResults[1])
                {
                    _raycastPosition = new Vector3(_raycastPosition.x, previousRealPosition.y, _raycastPosition.z);

                    Debug.Log("Raycast back in Y");
                }
                // Z axis
                if (_raycastResults[2])
                {
                    _raycastPosition = new Vector3(_raycastPosition.x, _raycastPosition.y, previousRealPosition.z);

                    Debug.Log("Raycast back in Z");
                }

                /*
                // X axis
                if (RaycastZone(new Vector3(transform.position.x, previousRealPosition.y, previousRealPosition.z) + collider.center, colliderExtents))
                {
                    _raycastPosition = new Vector3(previousRealPosition.x, _raycastPosition.y, _raycastPosition.z);

                    Debug.Log("Raycast back in X");
                }
                // Y axis
                if (RaycastZone(new Vector3(previousRealPosition.x, transform.position.y, previousRealPosition.z) + collider.center, colliderExtents))
                {
                    _raycastPosition = new Vector3(_raycastPosition.x, previousRealPosition.y, _raycastPosition.z);

                    Debug.Log("Raycast back in Y");

                    Debug.Log($"Movement Y => : Center : {Vector3String(new Vector3(previousRealPosition.x, transform.position.y, previousRealPosition.z) + collider.center)} | Extents : {Vector3String(colliderExtents)}");
                }
                // Z axis
                if (RaycastZone(new Vector3(previousRealPosition.x, previousRealPosition.y, transform.position.z) + collider.center, colliderExtents))
                {
                    _raycastPosition = new Vector3(_raycastPosition.x, _raycastPosition.y, previousRealPosition.z);

                    Debug.Log("Raycast back in Z");
                }*/
            }

            // If the raycast position is still the same as the position of the character, where there are bad colliders stucking the character, triggers Powerful Mode
            if (_raycastPosition == transform.position)
            {
                Debug.Log("Stuck ! Powerful Mode !");

                // Set the boolean indicating there's a bad obstacle
                isInCumbersomeCollider = true;
            }
        }

        // If the character is not into some cumbersome collider, restrains its movements in the zones where there are no bad colliders
        if (!isInCumbersomeCollider)
        {
            // If the final position is blocked by something, do not move
            if (Physics.OverlapBox(new Vector3(_newPosition.x, _raycastPosition.y, _raycastPosition.z) + collider.center, colliderExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length > 0)
            {
                Debug.Log("No movement posible in X !");
                _newPosition = new Vector3(transform.position.x, _newPosition.y, _newPosition.z);
            }
            if (Physics.OverlapBox(new Vector3(_raycastPosition.x, _newPosition.y, _raycastPosition.z) + collider.center, colliderExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length > 0)
            {
                Debug.Log("No movement posible in Y !");
                _newPosition = new Vector3(_newPosition.x, transform.position.y, _newPosition.z);
            }
            if (Physics.OverlapBox(new Vector3(_raycastPosition.x, _raycastPosition.y, _newPosition.z) + collider.center, colliderExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length > 0)
            {
                Debug.Log("No movement posible in Z !");
                _newPosition = new Vector3(_newPosition.x, _newPosition.y, transform.position.z);
            }
        }
        // Update the previous position as this position, before movement
        previousRealPosition = _raycastPosition;

        // If nothing does block the way, move to the objective
        transform.position = _newPosition;
    }

    /// <summary>
    /// With 2 box with the same size, raycast separatly in the 3 axis in the zone of the second box not touching the first one
    /// </summary>
    /// <param name="_firstBoxCenter">Center position of the first box</param>
    /// <param name="_secondBoxCenter">Center position of the second box</param>
    /// <param name="_extents">Extents (half size) of the boxes</param>
    /// <returns>Returns an array of 3 bools indicating respectively for the X, Y & Z axis if a bad collider was touched</returns>
    private bool[] RaycastZoneOutsideBox(Vector3 _firstBoxCenter, Vector3 _secondBoxCenter, Vector3 _extents)
    {
        // Get the movement between the two positions, in absolute value
        Vector3 _movement = _secondBoxCenter - _firstBoxCenter;

        // Creates the array that will contain the results
        bool[] _result = new bool[3];

        // Foreach axis, if a movement has been made, get if it was posible ; if no movement, it was posible

        // X axis
        if (_movement.x != 0)
        {
            _result[0] = RaycastZone(new Vector3(_firstBoxCenter.x + (_extents.x * Mathf.Sign(_movement.x)) + (_movement.x / 2), _firstBoxCenter.y, _firstBoxCenter.z) + collider.center, new Vector3(Mathf.Abs(_movement.x / 2), _extents.y, _extents.z));

        }
        else
        {
            _result[0] = false;
        }
        // Y axis
        if (_movement.y != 0)
        {
            _result[1] = RaycastZone(new Vector3(_firstBoxCenter.x, _firstBoxCenter.y + (_extents.y * Mathf.Sign(_movement.y)) + (_movement.y / 2), _firstBoxCenter.z) + collider.center, new Vector3(_extents.x, Mathf.Abs(_movement.y / 2), _extents.z));
        }
        else
        {
            _result[1] = false;
        }
        // Z axis
        if (_movement.z != 0)
        {
            _result[2] = RaycastZone(new Vector3(_firstBoxCenter.x, _firstBoxCenter.y, _firstBoxCenter.z + (_extents.z * Mathf.Sign(_movement.z)) + (_movement.z / 2)) + collider.center, new Vector3(_extents.x, _extents.y, Mathf.Abs(_movement.z / 2)));
        }
        else
        {
            _result[2] = false;
        }

        // Returns the results of the raycasts in the zone
        return _result;
    }

    /// <summary>
    /// Raycast in a given zone to know if some bad colliders are in it
    /// </summary>
    /// <param name="_center">Center position to raycast from</param>
    /// <param name="_extents">Extents (half size) of the cube zone to raycast</param>
    /// <returns>Returns true if a collider is touched, else way false</returns>
    private bool RaycastZone(Vector3 _center, Vector3 _extents)
    {
        return Physics.OverlapBox(_center, _extents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length > 0;
    }

    // Debug Utility
    string Vector3String(Vector3 _vector)
    {
        string _return = $"({_vector.x}, {_vector.y}, {_vector.z})";
        return _return;
    }
    #endregion
    #endregion
}

[Serializable]
public class TDS_Box
{
    // Box :
    // 
    // Serializable class containing box settings, including :
    //      - The box center
    //      - Extents of the box
    //      - Color of the box (Gizmos uses)

    [Header("Color :")]
    // The color of the box
    [SerializeField] private Color color = Color.green;
    public Color Color
    {
        get { return color; }
        set { color = value; }
    }

    [Header("Vector3 :")]
    // The center position of the box
    [SerializeField] private Vector3 center = Vector3.zero;
    public Vector3 Center
    {
        get { return center; }
        set { center = value; }
    }
    // The extents (half size) of the box
    [SerializeField] private Vector3 extents = Vector3.one;
    public Vector3 Extents
    {
        get { return extents; }
        set { extents = value; }
    }
}
