using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_FatLady : TDS_Player
{
    #region Events

    #endregion

    #region Fields / Accessors
    // Indicates if the character is in super rage mode, or not
    [SerializeField] private bool isInRageMode = false;
    public bool IsInRageMode
    {
        get { return isInRageMode; }
    }

    // Rage mode character's damages multiplier
    [SerializeField] private float rageModeMultiplier = 1.5f;

    // Indicates since which health value the character enter into rage mode
    [SerializeField] private int rageModeHealth = 30;
    #endregion

    #region Methods
    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        character = PlayerCharacter.FatLady;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #region Original Methods
    public override void ExecuteAction(string _actionID)
    {
        base.ExecuteAction(_actionID);

        switch (_actionID)
        {
            case "AttackOne":
                if (currentAttack != PlayerAttacks.None) return;

                currentAttack = PlayerAttacks.AttackOne;
                isStroking = true;
                CharacterAnimator.SetInteger("State", 1);

                if (PhotonNetwork.isMasterClient)
                {
                    currentAttackCoroutine = StartCoroutine(Attack((int)(5 * (isInRageMode ? rageModeMultiplier : 1)), (int)(10 * (isInRageMode ? rageModeMultiplier : 1))));
                }
                break;
            case "AttackTwo":
                if (currentAttack != PlayerAttacks.None) return;

                currentAttack = PlayerAttacks.AttackTwo;
                isStroking = true;
                CharacterAnimator.SetInteger("State", 2);
                if (PhotonNetwork.isMasterClient)
                {
                    currentAttackCoroutine = StartCoroutine(Attack((int)(6 * (isInRageMode ? rageModeMultiplier : 1)), (int)(9 * (isInRageMode ? rageModeMultiplier : 1))));
                }
                break;
            case "AttackThree":
                if (currentAttack != PlayerAttacks.None) return;

                currentAttack = PlayerAttacks.AttackThree;
                isStroking = true;
                CharacterAnimator.SetInteger("State", 3);
                if (PhotonNetwork.isMasterClient)
                {
                    currentAttackCoroutine = StartCoroutine(Attack((int)(7 * (isInRageMode ? rageModeMultiplier : 1)), (int)(11 * (isInRageMode ? rageModeMultiplier : 1))));
                }
                break;
            default:
                break;
        }
    }

    protected override void Heal(int _healValue)
    {
        base.Heal(_healValue);

        if (health > rageModeHealth)
        {
            isInRageMode = false;
            CharacterAnimator.SetBool("IsInRagEMode", false);
        }
    }

    /// <summary>
    /// Makes the object take damages and so decreases its health
    /// </summary>
    /// <param name="_damages">Damages amount to inflict</param>
    public override void TakeDamage(int _damages)
    {
        base.TakeDamage(_damages);

        if (health <= rageModeHealth)
        {
            isInRageMode = true;
            CharacterAnimator.SetBool("IsInRagEMode", true);
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
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }
    #endregion
    #endregion
    #endregion
}

