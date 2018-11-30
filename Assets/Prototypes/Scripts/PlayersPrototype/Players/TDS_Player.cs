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
    Super,
    None
}

public enum PlayerCharacter
{
    BeardLady,
    FatLady,
    FireEater,
    Juggler
}

[RequireComponent(typeof(TDS_FreakController))]
public abstract class TDS_Player : TDS_Character
{
    public event Action OnHeal;

    #region Fields/Properties
    [SerializeField, Header("Player", order = 0), Header("Bool", order = 1)] protected bool isCatching = false;
    [SerializeField] protected bool isDodging = false;
    [SerializeField] protected bool isGrounded = true;
    [SerializeField] protected bool isStroking = false;

    [SerializeField, Header("Attack")] protected PlayerAttacks currentAttack = PlayerAttacks.None;
    [SerializeField] protected Coroutine currentAttackCoroutine = null;

    [SerializeField, Header("Character")] protected PlayerCharacter character = PlayerCharacter.BeardLady;

    [SerializeField, Header("Freak Controller")] protected TDS_FreakController controller = null;

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

    [SerializeField, Header("Float")] protected float dodgeTime = .5f; 
    #endregion

    #region Methods
    /// <summary>
    /// Checks inputs and executes related actions
    /// </summary>
    protected virtual void Actions()
    {
        // Actions verifications
        if (currentAttack != PlayerAttacks.None) return;

        controller.Jump("Jump", true);

        if (Input.GetButtonDown("Fire1"))
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
        else if (Input.GetButtonDown("Fire2"))
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
        else if (Input.GetButtonDown("Fire3"))
        {
            AttackThree();
        }
        else if (Input.GetButtonDown("Alt Fire1"))
        {
            InteractWithObjects();
        }
        else if (Input.GetButtonDown("Alt Fire2"))
        {
            Catch();
        }
        else
        {
            // Get the triggers pression
            float _triggers = Input.GetAxis("Joystick Triggers");

            if (_triggers > .5f)
            {
                ThrowObject();
            }
            else if (_triggers < -.5f)
            {
                StartCoroutine(Dodge());
            }
        }
    }

    protected override IEnumerator Attack(int _minDamage, int _maxDamage)
    {
        return base.Attack(_minDamage, _maxDamage);
    }

    protected virtual IEnumerator EndAttack()
    {
        if (currentAttackCoroutine != null)
        {
            StopCoroutine(currentAttackCoroutine);
        }

        CharacterAnimator.SetInteger("State", 0);

        yield return null;

        currentAttack = PlayerAttacks.None;
        isStroking = false;
    }

    public override void ExecuteAction(string _actionID)
    {
        base.ExecuteAction(_actionID);

        switch (_actionID)
        {
            case "Catch":
                StartCoroutine(Catching());
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Moves the player
    /// </summary>
    protected void Move()
    {
        // Get the movement
        float _horizontal = Input.GetAxis("Horizontal");
        float _vertical = Input.GetAxis("Vertical");

        // Set the orientation side of the player
        if (_horizontal >= .01F)
        {
            if (facingSide != FacingSide.Right)
            {
                ChangeSide(FacingSide.Right);
            }
        }
        else if (_horizontal <= -.01f)
        {
            if (facingSide != FacingSide.Left)
            {
                ChangeSide(FacingSide.Left);
            }
        }
        /*else if (_vertical > 0)
        {
            if (facingSide != FacingSide.Back)
            {
                ChangeSide(FacingSide.Back);
            }
        }
        else if (_vertical < 0)
        {
            if (facingSide != FacingSide.Face)
            {
                ChangeSide(FacingSide.Face);
            }
        }*/

        // Set the destination of the player's inputs
        SetDestination(new Vector3(_horizontal, 0, _vertical));
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

    #region Actions
    protected abstract void AirAttack();
    protected virtual void Catch()
    {
        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("LaunchAction", PhotonTargets.All, PhotonViewElementID, "Catch");
    }
    protected abstract void RodeoAttack();
    protected abstract void Super();
    #endregion
    protected abstract IEnumerator Catching();

    protected virtual IEnumerator Dodge()
    {
        float _timer = 0;
        Vector3 _movement = Vector3.zero;
        float _originalSpeed = speed;

        isDodging = true;
        speed *= 1.5f;

        switch (facingSide)
        {
            case FacingSide.Face:
                _movement = -Vector3.forward;
                break;
            case FacingSide.Left:
                _movement = -Vector3.right;
                break;
            case FacingSide.Right:
                _movement = Vector3.right;
                break;
            case FacingSide.Back:
                _movement = Vector3.forward;
                break;
            default:
                break;
        }

        while (_timer < dodgeTime)
        {
            // Get the movement
            float _horizontal = Input.GetAxis("Horizontal");
            float _vertical = Input.GetAxis("Vertical");
            
            if (_horizontal != 0 || _vertical != 0)
            {
                _movement = new Vector3(_horizontal, 0, _vertical);
            }

            // Set the orientation side of the player
            if (_horizontal >= .5f)
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
                if (facingSide != FacingSide.Back)
                {
                    ChangeSide(FacingSide.Back);
                }
            }
            else if (_vertical < 0)
            {
                if (facingSide != FacingSide.Face)
                {
                    ChangeSide(FacingSide.Face);
                }
            }

            // Set the destination of the player's inputs
            SetDestination(_movement * 1.5f);

            yield return new WaitForEndOfFrame();
            _timer += Time.deltaTime;
        }
        speed = _originalSpeed;
        isDodging = false;
    }

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
            TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("DropObject", PhotonTargets.All, PhotonViewElementID);
        }
    }

    protected override void SetDestination(Vector3 _position)
    {
        controller.Move(_position, true);
    }
    #endregion

    #region UnityMethods
    protected virtual void Awake()
    {
        if (!controller) controller = GetComponent<TDS_FreakController>();
    }

    protected virtual void FixedUpdate()
    {
        // If the player is dodging, return
        if (isDodging || isCatching || isStroking) return;

        // If not, just set the position of this player localy
        else
        {
            NetSetOnlinePosition(); 
        }

    }

    protected override void Start() 
    {
        base.Start();

        CharacterAnimator.SetInteger("OrientationState", (int)facingSide);

        if (photonViewElement.isMine)
        {
            TDS_Camera.Instance.SetPlayer(controller);
        }
        else
        {
        }
    }
    
    protected override void Update () 
    {
        base.Update();

        // If the player is dodging, catching or stroking, return
        if (isDodging || isCatching || isStroking) return;

        // If it's the player's avatar : Checks the inputs of the player
        if (photonViewElement.isMine)
        {
            // Change character button
            if (Input.GetButtonDown("Menu"))
            {
                DropObject();
                TDS_GameManager.Instance.LeaveParty(character);
                PhotonNetwork.Destroy(photonViewElement);
                Destroy(gameObject);
                return;
            }

            Move();
            Actions();
        }
    }
    #endregion
}
