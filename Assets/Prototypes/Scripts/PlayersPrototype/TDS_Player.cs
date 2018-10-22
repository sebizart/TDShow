using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] TDS_Player Version 0.0.1
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

public class TDS_Player : TDS_Character 
{
    public event Action OnHeal;

    #region Fields/Properties
    [SerializeField, Header("Player", order = 0), Header("Int", order = 1)] protected int comboMax = 3;
    [SerializeField] protected int comboResetTime = 1;
    [SerializeField] protected int currentComboValue = 0; 
    public int ComboValue
    {
        get
        {
            return currentComboValue; 
        }
        protected set
        {
            if(value >= comboMax)
            {
                value = 0; 
            }
            currentComboValue = value; 
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Check the inputs of the player and executes actions in consequences
    /// </summary>
    protected void CheckInputs()
    {

    }

    /// <summary>
    /// Heal the player and so increases his life
    /// </summary>
    /// <param name="_healValue">Value of the health to heal (Increase value)</param>
    protected virtual void Heal(int _healValue)
    {
        Health += _healValue;
        OnHeal?.Invoke();
    }

    protected /*abstract*/ void AirAttack() { }
    protected /*abstract*/ void Catch() { }
    protected /*abstract*/ void Dodge() { }
    protected /*abstract*/ void RodeoAttack() { }
    protected /*abstract*/ void Super() { }

    #region Attacks
    protected override void AttackOne()
    {
    }

    protected override void AttackThree()
    {
    }

    protected override void AttackTwo()
    {
    }
    #endregion 
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
