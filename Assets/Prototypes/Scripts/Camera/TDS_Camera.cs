using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] CATA_Camera Version 0.0.1
Created by: Lucas Guibert
Date: 05 / 10 / 2018
Description: Manages the behaviour of the camera
Follows the movements of the player in the scene

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/

public class TDS_Camera : MonoBehaviour 
{
    #region Fields/Properties
    // Should the camera move on Y & Z axis ?
    [SerializeField] private bool doMoveOnZ = false;

    // The orientation of the camera (X axis)
    [SerializeField, Range(0,359)] private float cameraOrientation = 45;

    //The limits of the bounds of the camera
    [SerializeField] private float zForwardBound;
    public float ZForwardBound { get { return zForwardBound; } }
    public Vector3 ZForwardBoundsPosition
    {
        get
        {
            return new Vector3(transform.position.x, transform.position.y, zForwardBound); 
        }
    }
    [SerializeField] private float  zBackwardBound;
    public float ZBackwardBound { get { return  zForwardBound; } }
    public Vector3 ZBackwardBoundPosition
    {
        get
        {
          return new Vector3(transform.position.x, transform.position.y, zBackwardBound);
        }
    }

    // The max speed movement of the camera
    [SerializeField] private float maxSpeed = 10.5f;

    // The current speed movement of the camera
    [SerializeField] private float speed = 0;

    // The time the camera takes to get its max speed
    [SerializeField] private float speedInitTime = 0;

    // The offset of the camera
    [SerializeField, Range(-50, 50)] private float xOffset, yOffset, zOffset = 0;

    // The player the camera is following
    [SerializeField] private TDS_Controller player = null;

    // The rigidbody of the player
    [SerializeField] private Rigidbody playerRigidbody = null;
    #endregion

    #region Singleton
    // The singleton instance of this script
    public static TDS_Camera Instance = null;
    #endregion

    #region Methods
    // Follows the player movements
    private void FollowPlayer()
    {
        // Set the speed of the camera
        if (player.XMovement == 0 && player.ZMovement == 0) speed = maxSpeed / 2;
        else
        {
            speed += Mathf.Clamp(speed + (maxSpeed / speedInitTime), 0, maxSpeed);
        }

        // Moves the camera to the player's position
        transform.position = Vector3.Lerp(transform.position, new Vector3 (player.transform.position.x + xOffset,
                                     player.transform.position.y + yOffset,
                                     doMoveOnZ ? player.transform.position.z + zOffset : transform.position.z),
                                     Time.deltaTime * speed);

        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, zBackwardBound, zForwardBound));
    }


    // Use this to set the player
    public void SetPlayer(TDS_Controller _controller)
    {
        player = _controller;
        playerRigidbody = _controller.gameObject.GetComponent<Rigidbody>();
    }

    public void UpdateProperties()
    {
        Vector3 _target = player ? player.transform.position : Vector3.zero;
        transform.position = _target + new Vector3(xOffset, yOffset, zOffset);
        transform.localRotation = Quaternion.Euler(new Vector3(cameraOrientation, 0, 0)); 
    }
    #endregion

    #region UnityMethods
    private void Awake()
    {
        // Set the instance if needed
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            TDS_CustomDebug.CustomDebugLog("There is already a camera instance in this scene ! Destroys CameraBehaviour script", "Lucas");
            Destroy(this);
            return;
        }
    }

    private void OnDestroy()
    {
        // Nullifies the instance
        Instance = null;
    }

    void Start () 
	{

	}
	
	void Update () 
	{
        // If there is no player, returns
        if (!player) return;

        // Else, follows it
        FollowPlayer();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawSphere(ZForwardBoundsPosition, .5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(ZBackwardBoundPosition, .5f);
    }
    #endregion
}
