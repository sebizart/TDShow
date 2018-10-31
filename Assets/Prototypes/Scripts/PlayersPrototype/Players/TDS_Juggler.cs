using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TDS_Juggler : TDS_Player
{
    /* TDS_Juggler :
    *
    * This script manages the behaviour of the Juggler, a character the player can play
    * that can juggle with up to three objects in his three hands and throw them with
    * strength & precision
    */

    #region Events

    #endregion

    #region Fields / Accessors
    // The amount of projectile(s) currently in the hands of the Juggler
    [SerializeField] public int ProjectileAmount
    {
        get { return projectiles.Count; }
    }
    // The max amount of projectiles the Juggler can take simultaneously
    [SerializeField] private int projectileMaxAmount = 3;

    // The list of current projectiles in the Juggler's hands
    [SerializeField] private List<TDS_Throwable> projectiles = new List<TDS_Throwable>();
    #endregion

    #region Methods
    #region Unity Methods
    // Use this for initialization
    void Start ()
    {
		       
	}
	
	// Update is called once per frame
	void Update ()
    {

	}
    #endregion

    #region Original Methods
    protected override void ExecuteAction(PlayerAttacks _attack)
    {
        switch (_attack)
        {
            case PlayerAttacks.AttackOne:
                CallHit((int)PlayerAttacks.AttackOne);
                break;
            case PlayerAttacks.AttackTwo:
                CallHit((int)PlayerAttacks.AttackTwo);
                break;
            case PlayerAttacks.AttackThree:
                CallHit((int)PlayerAttacks.AttackThree);
                break;
            case PlayerAttacks.AirAttack:
                CallHit((int)PlayerAttacks.AirAttack);
                break;
            case PlayerAttacks.RodeoAttack:
                CallHit((int)PlayerAttacks.RodeoAttack);
                break;
            case PlayerAttacks.InteractWithObject:
                CallAction((int)PlayerAttacks.InteractWithObject);
                break;
            case PlayerAttacks.Dodge:
                CallAction((int)PlayerAttacks.Dodge);
                break;
            case PlayerAttacks.Catch:
                CallHit((int)PlayerAttacks.Catch);
                break;
            case PlayerAttacks.Super:
                CallHit((int)PlayerAttacks.Super);
                break;
            default:
                break;
        }
    }

    protected override void InterractWithObjects()
    {
        base.InterractWithObjects();
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

    protected override void Dodge()
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
    #endregion
    #endregion
    #endregion
}
