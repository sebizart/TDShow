using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] CATA_PointLightScript Version 0.0.1
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

public class CATA_PointLightScript : MonoBehaviour 
{
    #region Fields/Properties
    Light fireLight;
    float lightInt;
    [SerializeField]
    float minInt = 3f;
    [SerializeField]
    float maxInt = 5f;
    [SerializeField]
    Color fireColor;
    #endregion

    #region Methods
    void changeLightIntensity()
    {
        lightInt = UnityEngine.Random.Range(minInt, maxInt);
        fireLight.intensity = lightInt;
        fireLight.color = fireColor;
    }
    void GetLight()
    {
        fireLight = GetComponent<Light>();

    }
    #endregion

    #region UnityMethods
    void Start () 
		{
        GetLight();
		}
		
		void Update () 
		{
        changeLightIntensity();
		}
	#endregion
}
