using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_FireEater : TDS_Player
{
    #region Events

    #endregion

    #region Fields / Accessors

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
