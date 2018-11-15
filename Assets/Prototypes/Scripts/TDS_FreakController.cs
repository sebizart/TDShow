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

    [Header("Speed :")]
    // The initial speed of the movement
    [SerializeField] private float initialSpeed = 1;
    // Maximum default speed of the movement
    [SerializeField] private float maxSpeed = 1;
    // Speed of the movement
    [SerializeField] private float speed = 1;
    public float Speed { get { return speed; } }

    [Header("Layer Mask :")]
    // What the character should be stoped by
    public LayerMask WhatCollides = new LayerMask();

    [Header("Rigidbody :")]
    // Rigidbody of the character
    [SerializeField] private new Rigidbody rigidbody = null;
    public Rigidbody Rigidbody { get { return rigidbody; } }

    [Header("Vector3 :")]
    [SerializeField] private Vector3 groundBoxCenter = Vector3.zero;
    [SerializeField] private Vector3 groundBoxExtents = Vector3.one;
    // Prvious position of the character
    [SerializeField] Vector3 previousPosition;

    // DEBUG
    [SerializeField] Vector3 DEBUGdestination;

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

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + groundBoxCenter, groundBoxExtents);
    }

    // Use this for initialization
    void Start ()
    {

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
        if (Physics.OverlapBox(transform.position + groundBoxCenter, groundBoxExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length > 0)
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
    /// <param name="_isDirection">Should the given position be used as a direction from the actual position, or as world new position</param>
    public void Move(Vector3 _position, bool _isDirection = false)
    {
        // If the given position is a direction, convert it as world position
        if (_isDirection) _position = transform.position + _position;

        // Get the final position after movement
        Vector3 _newPosition = Vector3.Lerp(transform.position, _position, Time.deltaTime * speed);

        // Set the boolean indicating there's a bad obstacle
        isInCumbersomeCollider = false;

        // DEBUG
        DEBUGdestination = _newPosition;

        // Get the overlap box's extents value
        Vector3 _overlapExtents = (collider.size / 2) - Vector3.one * 0.0001f;

        // If the character is not currently into some cumbersome collider, restrict its moves into the zone where there's no collider (only in X & Z axis)
        if (Physics.OverlapBox(transform.position + collider.center, _overlapExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length > 0)
        {
            // OK ALORS JE L'AI
            // Lorsque le personnage retombe au sol suite à la gravité,
            // Il se trouve momentanément trèèèès légèrement en dessous du sol
            // Du coup, je détecte que je suis dans un obstacle
            // Sauf que, le rigidbody remet tout seul l'objet au Y correct
            // Mais si j'essaye de le faire revenir à sa position précédente,
            // Il tombe à l'infini et tout et tout
            // En revanche, si je dis qu'il est coincé & qu'il peut aller n'importe où
            // Alors il va traverser n'importe quel mur
            // C'est franchement vraiment très très nul
            // J'en ai marre

            // Si une force est appliquée via le rigidbody dans une direction,
            // ne pas changer la position sur les axes de cette direction et raycaster
            // depuis la précédente position si celle-ci est est libre sur ces axes

            // Set the new position to go as this one, 'cause there are bad colliders around
            Vector3 _realPosition = previousPosition;

            Debug.Log("Get back ! Get back ! Get back to where you once belong !");

            // Foreach axis, if the movement is posible from the previous position to the actual, let the actual position
            if (rigidbody.velocity.x != 0 || Physics.OverlapBox(new Vector3(transform.position.x, previousPosition.y, previousPosition.z) + collider.center, _overlapExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length < 1)
            {
                Debug.Log("Movement posible in X !");
                _realPosition = new Vector3(transform.position.x, _realPosition.y, _realPosition.z);
            }
            if (rigidbody.velocity.y != 0 || Physics.OverlapBox(new Vector3(previousPosition.x, transform.position.y, previousPosition.z) + collider.center, _overlapExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length < 1)
            {
                Debug.Log("Movement posible in Y !");
                _realPosition = new Vector3(_realPosition.x, transform.position.y, _realPosition.z);
            }
            if (rigidbody.velocity.z != 0 || Physics.OverlapBox(new Vector3(previousPosition.x, previousPosition.y, transform.position.z) + collider.center, _overlapExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length < 1)
            {
                Debug.Log("Movement posible in Z !");
                _realPosition = new Vector3(_realPosition.x, _realPosition.y, transform.position.z);
            }

            // If no movement from the previous position to the actual is posible & that the previous position contains obstacle, set character in powerful mode
            if (_realPosition == transform.position && Physics.OverlapBox(new Vector3(previousPosition.x, previousPosition.y, previousPosition.z) + collider.center, _overlapExtents, Quaternion.identity, WhatCollides, QueryTriggerInteraction.Ignore).Length > 0)
            {
                    Debug.Log("Stuck ! Powerful mode !");

                    // Set the boolean indicating there's a bad obstacle
                    isInCumbersomeCollider = true;
            }
            // Else, go to the real position
            else
            {
                Debug.Log($"OLD => {transform.position} | New => {_realPosition}");
                transform.position = _realPosition;
            }
        }
        // If the character is into some cumbersome collider, let it move to its destination regardless of other colliders
        if (!isInCumbersomeCollider)
        {
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
        // Update the previous position as this position, before movement
        previousPosition = transform.position;

        // If nothing does block the way, move to the objective
        transform.position = _newPosition;
    }
    #endregion

    string StringVector3(Vector3 _vector3)
    {
        string _return = "(";
        _return += _vector3.x + ", ";
        _return += _vector3.y + ", ";
        _return += _vector3.z + ")";

        return _return;
    }
    #endregion
}
