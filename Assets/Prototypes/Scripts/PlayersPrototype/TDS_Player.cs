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

public enum PlayerAttacks
{
    AttackOne = 1,
    AttackTwo,
    AttackThree,
    Catch,
    Dodge,
    AirAttack,
    RodeoAttack,
    Super
}

public abstract class TDS_Player : TDS_Character
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
            if (value >= comboMax)
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
            CallHit((int)PlayerAttacks.AttackOne);
        }
        else if (Input.GetKeyDown(catchKey))
        {
            CallHit((int)PlayerAttacks.Catch);
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

    public override void Hit(int _attackId)
    {
        // Triggers the right attack depending on the attack id
        switch (_attackId)
        {
            case 1:
                AttackOne();
                break;
            case 2:
                AttackTwo();
                break;
            case 3:
                AttackThree();
                break;
            case 4:
                Catch();
                break;
            case 5:
                Dodge();
                break;
            case 6:
                AirAttack();
                break;
            case 7:
                RodeoAttack();
                break;
            case 8:
                Super();
                break;
            default:
                TDS_CustomDebug.CustomDebugLogWarning($"The Player's attack ID \"{_attackId}\" is unknown, sorry miss");
                break;
        }
    }

    #region Attacks
    protected abstract void AirAttack();
    protected abstract void Catch();
    protected abstract void Dodge();
    protected abstract void RodeoAttack();
    protected abstract void Super();
    #endregion
    #endregion

    #region UnityMethods
    private void FixedUpdate()
    {
        // If it's the player's avatar : Checks the inputs of the player
        if (photonViewElement.isMine)
        {
            CheckInputs();
        }
        // If not, just set the position of this player localy
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
