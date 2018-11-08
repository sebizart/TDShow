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
    [SerializeField] BeardState currentBeardState = BeardState.Average;
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
                if (currentBeardState > BeardState.VeryShort)
                {
                    currentBeardState = (BeardState)((int)currentBeardState - 1);
                    if (beardAnimator) beardAnimator.SetInteger("BeardState", (int)currentBeardState);
                }
                beardDurability = 0;
            }
        }
    }
    [SerializeField] int beardDurabilityMax = 2;
    [SerializeField] int growingBeardCooldown = 5;
    [SerializeField] int growingBeardValue = 0;
    [Header("Animator"), SerializeField] Animator beardAnimator;
    #endregion

    #region Methods
    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        character = PlayerCharacter.BeardLady;
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
            if (currentBeardState < BeardState.VeryLong)
            {
                currentBeardState = (BeardState)((int)currentBeardState + 1);
                if (beardAnimator) beardAnimator.SetInteger("BeardState", (int)currentBeardState);
            }
        }
    }

    /// <summary>
    /// Reset the beard after the beard lady attacked. Call after every attack
    /// Add one to the beard Durability
    /// set growing beardValue to zero
    /// </summary>
    private void ResetBeardAfterAttack()
    {
        beardDurability++;
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
            case BeardState.VeryShort:
                _beardOffset = .5f;
                break;
            case BeardState.Short:
                _beardOffset = .75f;
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

    #region Combat
    protected override void AirAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void AttackOne()
    {
        throw new System.NotImplementedException();
    }

    protected override void AttackThree()
    {
        throw new System.NotImplementedException();
    }

    protected override void AttackTwo()
    {
        throw new System.NotImplementedException();
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

public enum BeardState
{
    VeryShort,
    Short,
    Average,
    Long,
    VeryLong
}
