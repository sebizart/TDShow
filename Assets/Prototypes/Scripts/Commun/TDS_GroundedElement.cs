using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon; 

/*
[Script Header] CATA_GroundedElement Version 0.0.1
Created by:
Date: 
Description:

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/

public class TDS_GroundedElement : PunBehaviour 
{
    #region Fields/Properties
    [SerializeField, Header("Snapping")] bool snapEnable = true;
    [SerializeField] LayerMask groundLayer; 
	#endregion

	#region Methods
    protected void SnapToGround()
    {
        if (!snapEnable) return; 
        RaycastHit _hit; 
        if(Physics.Raycast(transform.position, Vector3.down, out _hit, 10, groundLayer))
        {
            if(_hit.distance > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.down, Time.deltaTime);
                return; 
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up, Time.deltaTime);
            return; 
        }
       
    }

    /// <summary>
    /// Change the boolean is Snapped to 
    /// </summary>
    protected void SnapActivation()
    {
        snapEnable = !snapEnable;
    }
	#endregion

	#region UnityMethods
	void Start () 
		{
			
		}
	
	void Update () 
		{

		}
	#endregion
}
