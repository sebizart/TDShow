using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] CATA_Glowy Version 0.0.1
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

public class CATA_Glowy : MonoBehaviour 
{
    #region Fields/Properties
    Renderer fireRend;
    float fireIntensity;
    [SerializeField]
    float minIntensity;
    [SerializeField]
    float maxIntensity;
    [SerializeField]
    Color fireColor;
    #endregion

    #region Methods
    void ChangeFireIntensity()
    {
        fireIntensity = UnityEngine.Random.Range(minIntensity,maxIntensity);
        fireColor = fireRend.material.color;
        fireColor.a = fireIntensity;
        fireRend.material.color = fireColor;
    }
    void GetSpriteRender()
    {
        fireRend = GetComponent<SpriteRenderer>();
    }
    #endregion

    #region UnityMethods
    void Start () 
		{
        GetSpriteRender();
        }
		
		void Update () 
		{
        ChangeFireIntensity();
		}
	#endregion
}
