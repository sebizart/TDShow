using System;
using System.Collections;
using System.Collections.Generic;
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

    #region Fields/Properties
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
            if(value >= beardDurabilityMax)
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

    /// <summary>
    /// 
    /// </summary>
    protected override void AirAttack()
    {
        ResetBeardAfterAttack();
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void AttackOne()
    {
        ResetBeardAfterAttack();
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void AttackThree()
    {
        ResetBeardAfterAttack();
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void AttackTwo()
    {
        ResetBeardAfterAttack();
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void Catch()
    {
        ResetBeardAfterAttack();
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void Dodge()
    {
        ResetBeardAfterAttack();
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void RodeoAttack()
    {
        ResetBeardAfterAttack();
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void Super()
    {
        ResetBeardAfterAttack();
    }

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
        if(growingBeardValue >= growingBeardCooldown)
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
    #endregion

    #region UnityMethods
    void Start () 
	{
        InvokeRepeating("GrowBeard", 1, 1); 
	}
    
    void Update () 
	{
		
	}
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
