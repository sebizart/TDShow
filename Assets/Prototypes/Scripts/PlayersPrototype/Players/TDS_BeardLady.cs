using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
[Script Header] TDS_BeardLady Version 0.0.1
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

public class TDS_BeardLady : TDS_Player
{
    #region Events

    #endregion

    #region Fields / Accessors
    [Header("BeardLady")]
    [SerializeField] BeardState currentBeardState = BeardState.Short;
    [Header("Int"), SerializeField] int beardDurability = 0;
    public int BeardDurability
    {
        get
        {
            return beardDurability;
        }
        private set
        {
            if (value >= beardDurabilityMax)
            {
                if (currentBeardState > BeardState.Short)
                {
                    currentBeardState = (BeardState)((int)currentBeardState - 1);
                    if (CharacterAnimator) CharacterAnimator.SetInteger("BeardState", (int)currentBeardState);
                }
                value = 0;
            }
            beardDurability = value;
        }
    }
    [SerializeField] int beardDurabilityMax = 2;
    [SerializeField] int growingBeardCooldown = 5;
    [SerializeField] int growingBeardValue = 0;

    [Header("Float"), SerializeField] private float catchTime = .5f;

    [SerializeField] Component test = null;
    #endregion

    #region Methods
    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        character = PlayerCharacter.BeardLady;
        currentBeardState = BeardState.Short;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        TDS_AttackBox _currentBox = new TDS_AttackBox();

        switch (currentAttack)
        {
            case PlayerAttacks.AttackOne:
                _currentBox = attackBoxes.Where(b => b.ID == 1).FirstOrDefault();
                if (_currentBox != null)
                {
                    Gizmos.color = _currentBox.BoxColor;
                    Gizmos.DrawCube(transform.TransformPoint(_currentBox.Collider.center), Vector3.Scale(_currentBox.Collider.size, _currentBox.Collider.transform.lossyScale));
                    Gizmos.color = Color.white;
                }
                break;
            case PlayerAttacks.AttackTwo:
                _currentBox = attackBoxes.Where(b => b.ID == 2).FirstOrDefault();
                if (_currentBox != null)
                {
                    Gizmos.color = _currentBox.BoxColor;
                    Gizmos.DrawCube(transform.TransformPoint(_currentBox.Collider.center), Vector3.Scale(_currentBox.Collider.size, _currentBox.Collider.transform.lossyScale));
                    Gizmos.color = Color.white;
                }
                break;
            case PlayerAttacks.AttackThree:
                _currentBox = attackBoxes.Where(b => b.ID == 3).FirstOrDefault();
                if (_currentBox != null)
                {
                    Gizmos.color = _currentBox.BoxColor;
                    Gizmos.DrawCube(transform.TransformPoint(_currentBox.Collider.center), Vector3.Scale(_currentBox.Collider.size, _currentBox.Collider.transform.lossyScale));
                    Gizmos.color = Color.white;
                }
                break;
            case PlayerAttacks.AirAttack:
                break;
            case PlayerAttacks.RodeoAttack:
                break;
            case PlayerAttacks.InteractWithObject:
                break;
            case PlayerAttacks.Dodge:
                break;
            case PlayerAttacks.Catch:
                _currentBox = attackBoxes.Where(b => b.ID == 4).FirstOrDefault();
                if (_currentBox != null)
                {
                    Gizmos.color = _currentBox.BoxColor;
                    Gizmos.DrawCube(transform.TransformPoint(_currentBox.Collider.center), Vector3.Scale(_currentBox.Collider.size, _currentBox.Collider.transform.lossyScale));
                    Gizmos.color = Color.white;
                }
                break;
            case PlayerAttacks.Super:
                break;
            case PlayerAttacks.None:
                break;
            default:
                break;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        InvokeRepeating("GrowBeard", 1, 1);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #region Original Methods
    #region Beard
    /// <summary>
    /// Call Every seconds, if the value is greater than the cooldown 
    /// Set the value to zero
    /// Add 1 to the current beard state
    /// </summary>
    private void GrowBeard()
    {
        if (currentBeardState == BeardState.VeryLong) return;

        growingBeardValue++;

        if (growingBeardValue >= growingBeardCooldown)
        {
            growingBeardValue = 0;
            beardDurability = 0;
            currentBeardState = (BeardState)((int)currentBeardState + 1);
            if (CharacterAnimator) CharacterAnimator.SetInteger("BeardState", (int)currentBeardState);
        }
    }

    /// <summary>
    /// Reset the beard after the beard lady attacked. Call after every attack
    /// Add one to the beard Durability
    /// set growing beardValue to zero
    /// </summary>
    protected override void EndAttack()
    {
        BeardDurability++;
        growingBeardValue = 0;

        base.EndAttack();

        InvokeRepeating("GrowBeard", 1, 1);
    }
    #endregion

    public override void ExecuteAction(string _actionID)
    {
        base.ExecuteAction(_actionID);

        switch (_actionID)
        {
            case "AttackOne":
                if (currentBeardState == BeardState.Short || currentAttack != PlayerAttacks.None) return;

                CancelInvoke("GrowBeard");
                currentAttack = PlayerAttacks.AttackOne;
                CharacterAnimator.SetInteger("AttackState", 1);
                if (PhotonNetwork.isMasterClient)
                {
                    currentAttackCoroutine = StartCoroutine(Attack(1));
                }
                break;
            case "AttackTwo":
                if (currentBeardState == BeardState.Short || currentAttack != PlayerAttacks.None) return;

                CancelInvoke("GrowBeard");
                currentAttack = PlayerAttacks.AttackTwo;
                CharacterAnimator.SetInteger("AttackState", 2);
                if (PhotonNetwork.isMasterClient)
                {
                    currentAttackCoroutine = StartCoroutine(Attack(2));
                }
                break;
            case "AttackThree":
                if (currentBeardState == BeardState.Short || currentAttack != PlayerAttacks.None) return;

                CancelInvoke("GrowBeard");
                currentAttack = PlayerAttacks.AttackThree;
                CharacterAnimator.SetInteger("AttackState", 3);
                if (PhotonNetwork.isMasterClient)
                {
                    currentAttackCoroutine = StartCoroutine(Attack(3));
                }
                break;
            default:
                break;
        }
    }

    #region Combat
    protected override void AirAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void AttackOne()
    {
        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("LaunchAction", PhotonTargets.All, PhotonViewElementID, "AttackOne");
    }

    protected override void AttackThree()
    {
        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("LaunchAction", PhotonTargets.All, PhotonViewElementID, "AttackThree");
    }

    protected override void AttackTwo()
    {
        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("LaunchAction", PhotonTargets.All, PhotonViewElementID, "AttackTwo");
    }

    protected override void Catch()
    {
        base.Catch();
    }

    protected override void RodeoAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void Super()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator Catching()
    {
        currentAttack = PlayerAttacks.Catch;
        isCatching = true;

        if (PhotonNetwork.isMasterClient)
        {
            float _timer = 0;

            while (_timer < catchTime)
            {
                TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("ApplyInfoDamages", PhotonTargets.All, TDS_RPCManager.Instance.SetInfoDamages(CheckHit(4), PhotonViewElementID, 4));

                yield return new WaitForSeconds(.05f);

                _timer += .05f;
            }
        }
        else
        {
            yield return new WaitForSeconds(catchTime);
        }

        currentAttack = PlayerAttacks.None;
        isCatching = false;
    }
    #endregion
    #endregion
    #endregion
}

public enum BeardState
{
    Short,
    Average,
    Long,
    VeryLong
}
