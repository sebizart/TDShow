using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
[Script Header] TDS_Enemy Version 0.0.1
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

public class TDS_Enemy : TDS_Character 
{


    #region Fields/Properties
    [Header("Enemy")]
    [SerializeField]private EnemyName prefabName = EnemyName.Acrobat; 
    public EnemyName PrefabName { get { return prefabName; } }

    [SerializeField, Header("Nav Mesh")] protected NavMeshAgent navMeshCharacter;
    private Vector3 netOnlinePosition;
    #endregion

    #region Methods
    protected override void AttackOne()
    {
        throw new NotImplementedException();
    }

    protected override void AttackThree()
    {
        throw new NotImplementedException();
    }

    protected override void AttackTwo()
    {
        throw new NotImplementedException();
    }

    protected override void SetDestination(Vector3 _position)
    {
        navMeshCharacter.SetDestination(_position);
    }
    #endregion

    #region UnityMethods
    protected override void Start () 
	{
        base.Start();
	}

    protected override void Update () 
	{
        base.Update();
	}
    #endregion
}
public enum EnemyName
{
    Acrobat, 
    Fakir, 
    MightyMan, 
    Mime, 
    Punk
}
