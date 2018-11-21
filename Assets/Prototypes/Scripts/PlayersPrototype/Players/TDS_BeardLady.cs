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

// CHOSES A FAIRE POUR LE 23/10 
// -> FAIRE DES ATTACK BOX POUR CHAQUE TAILLE DE LA BARBE
// -> REGLER LES ANIMATIONS
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
                beardDurability = 0;
            }
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
                    Gizmos.DrawCube(transform.position + _currentBox.CenterPosition, _currentBox.ExtendPosition);
                    Gizmos.color = Color.white;
                }
                break;
            case PlayerAttacks.AttackTwo:
                _currentBox = attackBoxes.Where(b => b.ID == 2).FirstOrDefault();
                if (_currentBox != null)
                {
                    Gizmos.color = _currentBox.BoxColor;
                    Gizmos.DrawCube(transform.position + _currentBox.CenterPosition, _currentBox.ExtendPosition);
                    Gizmos.color = Color.white;
                }
                break;
            case PlayerAttacks.AttackThree:
                _currentBox = attackBoxes.Where(b => b.ID == 3).FirstOrDefault();
                if (_currentBox != null)
                {
                    Gizmos.color = _currentBox.BoxColor;
                    Gizmos.DrawCube(transform.position + _currentBox.CenterPosition, _currentBox.ExtendPosition);
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
                    Gizmos.DrawCube(transform.position + _currentBox.CenterPosition, _currentBox.ExtendPosition);
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
            currentBeardState = (BeardState)((int)currentBeardState + 1);
            if (CharacterAnimator) CharacterAnimator.SetInteger("BeardState", (int)currentBeardState);
        }
    }

    /// <summary>
    /// Reset the beard after the beard lady attacked. Call after every attack
    /// Add one to the beard Durability
    /// set growing beardValue to zero
    /// </summary>
    private void ResetBeardAfterAttack()
    {
        BeardDurability++;
        growingBeardValue = 0;
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_attackID"></param>
    /// <returns></returns>
    public override Dictionary<int, int> CheckHit(int _attackID)
    {
        if (!PhotonNetwork.isMasterClient) return null;
        // Get the right box         
        TDS_AttackBox _box = attackBoxes.Where(b => b.ID == _attackID).FirstOrDefault();
        if (_box == null)
        {
            TDS_CustomDebug.CustomDebugLog($"Attack Box n°{_attackID} can't be found on {this.name}");
            return null;
        }
        float _beardOffset;
        switch (currentBeardState)
        {
            case BeardState.Short:
                _beardOffset = .5f;
                break;
            case BeardState.Average:
                _beardOffset = 1;
                break;
            case BeardState.Long:
                _beardOffset = 1.5f;
                break;
            case BeardState.VeryLong:
                _beardOffset = 2;
                break;
            default:
                _beardOffset = 1;
                break;
        }
        return _box.RayCastAttack(_beardOffset);
    }

    public override void ExecuteAction(string _actionID)
    {
        base.ExecuteAction(_actionID);

        switch (_actionID)
        {
            case "AttackOne":
                StartCoroutine(BeardStroke());
                break;
            case "AttackTwo":
                StartCoroutine(Whirligig());
                break;
            case "AttackThree":
                StartCoroutine(HazelCatcher());
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

    private IEnumerator BeardStroke()
    {
        currentAttack = PlayerAttacks.AttackOne;
        isStroking = true;

        if (PhotonNetwork.isMasterClient)
        {
            float _timer = 0;

            while (_timer < 1f)
            {
                TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("ApplyInfoDamages", PhotonTargets.All, TDS_RPCManager.Instance.SetInfoDamages(CheckHit(1), PhotonViewElementID, 1));

                yield return new WaitForSeconds(.05f);

                _timer += .05f;
            }
        }
        else
        {
            yield return new WaitForSeconds(catchTime);
        }

        currentAttack = PlayerAttacks.None;
        isStroking = false;

        ResetBeardAfterAttack();
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

        ResetBeardAfterAttack();
    }

    private IEnumerator HazelCatcher()
    {
        currentAttack = PlayerAttacks.AttackThree;
        isStroking = true;

        if (PhotonNetwork.isMasterClient)
        {
            float _timer = 0;

            while (_timer < .5f)
            {
                TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("ApplyInfoDamages", PhotonTargets.All, TDS_RPCManager.Instance.SetInfoDamages(CheckHit(3), PhotonViewElementID, 3));

                yield return new WaitForSeconds(.05f);

                _timer += .05f;
            }
        }
        else
        {
            yield return new WaitForSeconds(catchTime);
        }

        currentAttack = PlayerAttacks.None;
        isStroking = false;

        ResetBeardAfterAttack();
    }

    private IEnumerator Whirligig()
    {
        currentAttack = PlayerAttacks.AttackTwo;
        isStroking = true;

        if (PhotonNetwork.isMasterClient)
        {
            float _timer = 0;

            while (_timer < .75f)
            {
                TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("ApplyInfoDamages", PhotonTargets.All, TDS_RPCManager.Instance.SetInfoDamages(CheckHit(2), PhotonViewElementID, 2));

                yield return new WaitForSeconds(.05f);

                _timer += .05f;
            }
        }
        else
        {
            yield return new WaitForSeconds(catchTime);
        }

        currentAttack = PlayerAttacks.None;
        isStroking = false;

        ResetBeardAfterAttack();
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
