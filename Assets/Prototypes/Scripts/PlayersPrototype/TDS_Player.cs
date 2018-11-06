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
    AirAttack,
    RodeoAttack,
    InteractWithObject,
    Dodge,
    Catch,
    Super
}

public abstract class TDS_Player : TDS_Character
{
    public event Action OnHeal;

    #region Fields/Properties
    [SerializeField, Header("Player", order = 0), Header("Bool", order = 1)] bool isGrounded = true;
    [SerializeField, Header("Int")] protected int comboMax = 3;
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
    [SerializeField] protected KeyCode attackOneKey = KeyCode.Mouse0;
    [SerializeField] protected KeyCode attackThreeKey = KeyCode.Mouse2;
    [SerializeField] protected KeyCode attackTwoKey = KeyCode.Mouse1;
    [SerializeField] protected KeyCode airAttackKey = KeyCode.Mouse0;
    [SerializeField] protected KeyCode airRodeoKey = KeyCode.Mouse1;
    [SerializeField] protected KeyCode interactWithObjectKey = KeyCode.E;
    [SerializeField] protected KeyCode dodgeKey = KeyCode.LeftControl;
    [SerializeField] protected KeyCode catchKey = KeyCode.F;
    [SerializeField] protected KeyCode superKey = KeyCode.X;
    #endregion
    #endregion

    #region Methods
    /// <summary>
    /// Check the inputs of the player and executes actions in consequences
    /// </summary>
    protected void CheckInputs()
    {
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TDS_GameManager.Instance.LeftParty(character);
            PhotonNetwork.Destroy(photonViewElement);
            return;
        }

=======
>>>>>>> parent of d156084... Sélection de Personnage
=======
>>>>>>> parent of d156084... Sélection de Personnage
=======
>>>>>>> parent of d156084... Sélection de Personnage
        float _horizontal = Input.GetAxis("Horizontal");
        float _vertical = Input.GetAxis("Vertical");

        // Set the orientation side of the player
        if (_horizontal >= .5F)
        {
            if (facingSide != FacingSide.Right)
            {
                ChangeSide(FacingSide.Right);
            }
        }
        else if (_horizontal <= -.5f)
        {
            if (facingSide != FacingSide.Left)
            {
                ChangeSide(FacingSide.Left);
            }
        }
        else if (_vertical > 0)
        {
            if (facingSide != FacingSide.Top)
            {
                ChangeSide(FacingSide.Top);
            }
        }
        else if (_vertical < 0)
        {
            if (facingSide != FacingSide.Bottom)
            {
                ChangeSide(FacingSide.Bottom);
            }
        }

        // Set the destination of the player's inputs
        SetDestination(new Vector3(transform.position.x + _horizontal, transform.position.y, transform.position.z + _vertical));

        // Attacks verifications
        if (Input.GetKeyDown(attackOneKey))
        {
            if (isGrounded)
            {
                AttackOne();
            }
            else
            {
                AirAttack();
            }
        }
        else if (Input.GetKeyDown(attackTwoKey))
        {
            if (isGrounded)
            {
                AttackTwo();
            }
            else
            {
                RodeoAttack();
            }
        }
        else if (Input.GetKeyDown(attackThreeKey))
        {
            AttackThree();
        }
        else if (Input.GetKeyDown(interactWithObjectKey))
        {
            InteractWithObjects();
        }
        else if (Input.GetKeyDown(dodgeKey))
        {
            Dodge();
        }
        else if (Input.GetKeyDown(catchKey))
        {
            Catch();
        }
        else if (Input.GetKeyDown(superKey))
        {
            Super();
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

    public override void Action(int _attackId)
    {
        // Triggers the right attack depending on the attack id
        switch ((PlayerAttacks)_attackId)
        {
            case PlayerAttacks.AttackOne:
                AttackOne();
                break;
            case PlayerAttacks.AttackTwo:
                AttackTwo();
                break;
            case PlayerAttacks.AttackThree:
                AttackThree();
                break;
            case PlayerAttacks.AirAttack:
                AirAttack();
                break;
            case PlayerAttacks.RodeoAttack:
                RodeoAttack();
                break;
            case PlayerAttacks.InteractWithObject:
                InteractWithObjects();
                break;
            case PlayerAttacks.Dodge:
                Dodge();
                break;
            case PlayerAttacks.Catch:
                Catch();
                break;
            case PlayerAttacks.Super:
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

    /// <summary>
    /// Makes the player interact with an object :
    ///     - If the player does not have an object in hand, take the nearest of the objects that can be taken in range
    ///     - If he does have an object in hand, throw it in front of him
    /// </summary>
    protected virtual void InteractWithObjects()
    {
        if (!projectile)
        {
            TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("TryToGrabObject", PhotonTargets.MasterClient, PhotonViewElementID);
        }
        else
        {
            TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("ThrowObject", PhotonTargets.All, PhotonViewElementID);
        }
    }
    #endregion

    #region UnityMethods
    protected virtual void FixedUpdate()
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

    protected virtual void Start () 
    {
    	
    }
    
    protected override void Update () 
    {
    	
    }
    #endregion
}
