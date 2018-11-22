using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireMiniGameState
{
    Early,
    Good,
    Late
}

public class TDS_FireEater : TDS_Player
{
    #region Events

    #endregion

    #region Fields / Accessors
    // Is the fire eater preparing a fire attack
    [SerializeField] private bool isPreparingFire = false;

    [SerializeField] private FireMiniGameState currentMiniGameState = FireMiniGameState.Early;
    #endregion

    #region Methods
    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        character = PlayerCharacter.FireEater;
    }

    protected override void FixedUpdate()
    {
        if (isPreparingFire) return;
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
                CharacterAnimator.SetInteger("AttackState", 1);
                if (PhotonNetwork.isMasterClient)
                {
                    currentAttackCoroutine = StartCoroutine(Attack(1));
                }
                break;
            case "AttackTwo_Good":
                Debug.Log("Attack Two Good !!");
                //if (currentAttack != PlayerAttacks.None) return;

                //currentAttack = PlayerAttacks.AttackTwo;
                //CharacterAnimator.SetInteger("AttackState", 2);
                // Instantiate fire ball
                break;
            case "AttackTwo_Early":
                Debug.Log("Attack Two Early...");
                break;
            case "AttackTwo_Late":
                Debug.Log("Attack Two Late...");
                break;
            case "AttackThree_Good":
                Debug.Log("Attack Three Good !!");
                //if (currentAttack != PlayerAttacks.AttackThree) return;

                //currentAttack = PlayerAttacks.AttackOne;
                //CharacterAnimator.SetInteger("AttackState", 3);
                //if (PhotonNetwork.isMasterClient)
                //{
                //    currentAttackCoroutine = StartCoroutine(Attack(2));
                //}
                break;
            case "AttackThree_Early":
                Debug.Log("Attack Three Early...");
                break;
            case "AttackThree_Late":
                Debug.Log("Attack Three Late...");
                break;
        }
    }

    public void FailMiniGame() => isPreparingFire = false;

    public void SetMiniGameState(FireMiniGameState _state) => currentMiniGameState = _state;

    private IEnumerator StartFireMiniGame(bool _isAttackTwo)
    {
        isPreparingFire = true;

        CharacterAnimator.SetBool("IsPreparingFire", true);

        while(!Input.GetButtonDown(_isAttackTwo ? "Fire2" : "Fire3") && isPreparingFire)
        {
            yield return new WaitForEndOfFrame();
        }

        CharacterAnimator.SetBool("IsPreparingFire", false);

        if (!isPreparingFire)
        {
            yield break;
        }
        isPreparingFire = false;

        switch (currentMiniGameState)
        {
            case FireMiniGameState.Early:
                TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("LaunchAction", PhotonTargets.All, PhotonViewElementID, _isAttackTwo ? "AttackTwo" : "AttackThree" + "_Early");
                break;
            case FireMiniGameState.Good:
                TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("LaunchAction", PhotonTargets.All, PhotonViewElementID, _isAttackTwo ? "AttackTwo" : "AttackThree" + "_Good");
                break;
            case FireMiniGameState.Late:
                TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("LaunchAction", PhotonTargets.All, PhotonViewElementID, _isAttackTwo ? "AttackTwo" : "AttackThree" + "_Late");
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
        StartCoroutine(StartFireMiniGame(false));
    }

    protected override void AttackTwo()
    {
        StartCoroutine(StartFireMiniGame(true));
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
