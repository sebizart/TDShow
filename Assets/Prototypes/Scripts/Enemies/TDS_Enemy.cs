using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon; 

/*
[Script Header] CATA_Enemies Version 0.0.1
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

public class TDS_Enemy : TDS_GroundedElement, TDS_IInterractible 
{
    public event Action<float> OnHealthModification; 

    #region Fields/Properties
    [SerializeField, Header("Int")] int health = 10;
    [SerializeField] int maxHealth = 10;
    [SerializeField, Header("Float")] float resetHealthTimer = 2;
    float resetHealthTimerValue = 0;

    [SerializeField, Header("Boolean")] bool isProjected = false;
    [SerializeField] bool isTakingDamages = false;

    [Header("Photon view")]public PhotonView EnemyPhotonView;      
    #endregion

    #region Methods
    /// <summary>
    /// Apply Damages to the enemy
    /// </summary>
    /// <param name="_damages"></param>
    public void ApplyDamages(int _damages)
    {
        isTakingDamages = true;
        resetHealthTimerValue = 0;
        //Remove enemy's health
        health -= _damages;
        health = Mathf.Clamp(health, 0, maxHealth);
        OnHealthModification?.Invoke((float)health / maxHealth);
        // Instantiate an DamageBehaviour Object
        GameObject _obj = Instantiate(Resources.Load("DamageBehaviour"), transform.position + Vector3.up, Quaternion.identity) as GameObject;
        DamageType _type = (DamageType)Mathf.Clamp(_damages - 1, 0, 2);
        _obj.GetComponent<TDS_DamageBehaviour>().Init(_type);
    }

    /// <summary>
    /// The enemy regenerate all his life
    /// </summary>
    public void ApplyRegeneration()
    {
        isTakingDamages = false;
        resetHealthTimerValue = 0;
        health = maxHealth;
        OnHealthModification?.Invoke((float)health / maxHealth);
    }

    /// <summary>
    /// Call when the enemy is projected
    /// </summary>
    /// <param name="_damages"></param>
    /// <param name="_playerPosition"></param>
    public void BeingProjected(int _damages, Transform _playerPosition)
    {
        // If he is already projected, he can't be projected again
        if (isProjected) return;
        SnapActivation();
        isTakingDamages = true;
        resetHealthTimerValue = 0;
        // He is projected
        isProjected = true;
        StartCoroutine(StartProjection(_damages, _playerPosition)); 
    }
   

    /// <summary>
    /// Start the projection
    /// </summary>
    /// <param name="_damages"></param>
    /// <param name="_playerPosition"></param>
    /// <returns></returns>
    private IEnumerator StartProjection(int _damages, Transform _playerPosition)
    {
        bool _isOnRight = _playerPosition.position.x > transform.position.x;
        float _xDistance = Vector3.Distance(new Vector3(_playerPosition.position.x, 0, 0), new Vector3(transform.position.x, 0, 0)); 
        // Get the position behind the player  
        Vector3 _endingPos; 
        if (_isOnRight)
        {
            _endingPos = new Vector3(_playerPosition.transform.position.x + _xDistance, transform.position.y, _playerPosition.position.z);
        }
        else
        {
             _endingPos = new Vector3(_playerPosition.transform.position.x - _xDistance, transform.position.y, _playerPosition.position.z);
        }

        // While the player isn't over the player, interpolate him to his destination
        while (Vector3.Distance(transform.position, (_playerPosition.position + Vector3.up)) >.01f)
        {
            transform.position = Vector3.Lerp(transform.position, _playerPosition.position + Vector3.up, Time.deltaTime * 10);
            yield return null;
        }
        // While the player isn't behind the player (on the left or the right), interpolate him to his destination
        while (Vector3.Distance(transform.position, _endingPos) > .01f)
        {
            transform.position = Vector3.Lerp(transform.position, _endingPos, Time.deltaTime * 10);
            yield return null;
        }
        ApplyDamages(_damages); 
        // Enemy isn't projected anymore
        isProjected = false;
        SnapActivation();
    }


    /// ONLINE INFOS TO SEND

    /// <summary>
    /// if the player is taking damages reset his timer
    /// else regenerate all his life
    /// </summary>
    private void ResetLifeTimer()
    {
        if (isTakingDamages)
        {
            resetHealthTimerValue += Time.deltaTime;
            if (resetHealthTimerValue >= resetHealthTimer)
            {
                ApplyRegeneration();
                TDS_EnemyRPCManager.Instance.EnemyRPCManagerPhotonView.RPC("RegenerateLife", PhotonTargets.Others, EnemyPhotonView.viewID);
            }
        }
    }

    public void SetProjection(int _damages, Transform _playerTransform)
    {
        BeingProjected(_damages, _playerTransform);
        TDS_EnemyRPCManager.Instance.EnemyRPCManagerPhotonView.RPC("ProjectEnemyAndApplyLifeModification", PhotonTargets.Others, EnemyPhotonView.viewID, _damages, TDS_Networking.Instance.OwnerID);
    }

    /// <summary>
    /// Call when the enemy takes damages
    /// </summary>
    /// <param damages="_damages"></param>
    public void TakeDamages(int _damages)
    {
        ApplyDamages(_damages);
        TDS_EnemyRPCManager.Instance.EnemyRPCManagerPhotonView.RPC("ApplyLifeModification", PhotonTargets.Others, EnemyPhotonView.viewID, _damages);
    }
    #endregion

    #region UnityMethods	
    void Start()
    {
    }
    void Update () 
	{
        SnapToGround();
        if (EnemyPhotonView.isMine)
        {
            ResetLifeTimer();
        }
    }


    #endregion
}
