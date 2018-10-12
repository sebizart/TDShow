using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] CATA_Hitbox Version 0.0.1
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

public class TDS_Hitbox : MonoBehaviour 
{
    #region Fields/Properties
    [SerializeField] Collider bounds; 
    public Collider Bounds { get { return bounds; } }

    [SerializeField] HitBoxType currentType = HitBoxType.DealDamages;

    public List<TDS_Enemy> InterractibleElements = new List<TDS_Enemy>();
    #endregion

    #region Methods

    #endregion

    #region UnityMethods
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.GetComponentInParent<TDS_Enemy>())
        {
            InterractibleElements.Add(collision.GetComponentInParent<TDS_Enemy>());
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.GetComponentInParent<TDS_Enemy>())
        {
            InterractibleElements.Remove(collision.GetComponentInParent<TDS_Enemy>());
        }
    }
    #endregion
}

public enum HitBoxType
{
    DealDamages, 
    TakeDamages
}
