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

    #region Key Codes
    [Header("Key Codes")]
    [SerializeField] KeyCode attackOneKey = KeyCode.Mouse0;
    [SerializeField] KeyCode catchKey = KeyCode.Mouse1;
    #endregion
    #endregion

    #region Methods
    /// <summary>
    /// Check the inputs of the player and executes actions in consequences
    /// </summary>
    protected void CheckInputs()
    {
        float _horizontal = Input.GetAxis("Horizontal");
        float _vertical = Input.GetAxis("Vertical");

        // Set the orientation side of the player
        if (_horizontal >= .5F)
        {
            if (currentSide != FacingSide.Right)
            {
                ChangeSide(FacingSide.Right);
            }
        }
        else if (_horizontal <= -.5f)
        {
            if (currentSide != FacingSide.Left)
            {
                ChangeSide(FacingSide.Left);
            }
        }
        else if (_vertical > 0)
        {
            if (currentSide != FacingSide.Top)
            {
                ChangeSide(FacingSide.Top);
            }
        }
        else if (_vertical < 0)
        {
            if (currentSide != FacingSide.Bottom)
            {
                ChangeSide(FacingSide.Bottom);
            }
        }

        // Set the destination of the player's inputs
        SetDestination(new Vector3(transform.position.x + _horizontal, transform.position.y, transform.position.z + _vertical));

        // Attacks verifications
        if (Input.GetKeyDown(attackOneKey))
        {
            CallHit(1);
        }
        else if (Input.GetKeyDown(catchKey))
        {
            CallHit(4);
        }
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

    public override void Hit(int _attackId)
    {
        Debug.Log("Hit " + _attackId + " !");
    }
    #endregion

    protected override void SetDestination(Vector3 _position)
    {
        base.SetDestination(_position);

    }
    #endregion

    #region UnityMethods
    private void FixedUpdate()
    {
        if(photonViewElement.isMine)
        {
            // Checks the inputs of the player
            CheckInputs();
        }
        else
        {
            NetSetOnlinePosition(); 
        }

    }

    void Start () 
    {
    	
    }
    
    void Update () 
    {
    	
    }
    #endregion
}
