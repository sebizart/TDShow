using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/*
[Script Header] TDS_DamageableElement Version 0.0.1
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

public abstract class TDS_DamageableElement : PunBehaviour
{
    #region Events
    public event Action OnDiying;
    public event Action OnTakingDamages;
    #endregion

    #region Fields/Properties
    [SerializeField, Header("Damageable Element", order = 0), Header("Bool",order =  1)] protected bool isAlive = true; 
    public bool IsAlive
    {
        get
        {
            return isAlive; 
        }
        protected set
        {
            if(value == false)
                OnDiying?.Invoke();
            isAlive = value; 
        }
    }

    [SerializeField, Header("Int")] protected int health; 
    public int Health
    {
        get
        {
            return health;
        }
        protected set
        {
            value = Mathf.Clamp(value, 0, maxHealth);
            if (value == 0)
            {
                IsAlive = false;
            }
            health = value;
        }
    }

    [SerializeField] protected int maxHealth = 50;
    public int MaxHealth
    {
        get { return maxHealth; }
    }

    [SerializeField, Header("Photon")] protected PhotonView photonViewElement; 
    public int PhotonViewElementID { get { return photonViewElement.viewID; } }
    #endregion

    #region Methods
    /// <summary>
    /// Makes the object take damages and so decreases its health
    /// </summary>
    /// <param name="_damages">Damages amount to inflict</param>
    public virtual void TakeDamage(int _damages)
    {
        Health -= _damages;
        OnTakingDamages?.Invoke();
    }
    #endregion

    #region UnityMethods

    #endregion
}
