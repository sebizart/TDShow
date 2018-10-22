using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEngine.AI;
using Photon; 

/*
[Script Header] TDS_Character Version 0.0.1
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

public abstract class TDS_Character : TDS_DamageableElement 
{

    #region Fields/Properties
    [SerializeField, Header("Character", order = 0), Header("Bool", order = 1)]protected bool canAttack = true; 
    [SerializeField]protected bool canMove = true;

    [SerializeField, Header("FacingSide")]protected FacingSide currentSide = FacingSide.Right; 
    public FacingSide CurrentSide { get { return currentSide; } }

    [SerializeField, Header("Float")] protected float speed = 1;

    [SerializeField, Header("Nav Mesh")] NavMeshAgent navMeshCharacter;

    //PROJECTILE

    [SerializeField, Header("Animator")] protected Animator CharacterAnimator; 

    [SerializeField, Header("AttackBoxes")] protected TDS_AttackBox[] attackBoxes = new TDS_AttackBox[] { };  
    #endregion

    #region Methods
    /// <summary>
    /// Makes the character change the side of the screen he's looking, its orientation
    /// </summary>
    /// <param name="_newSide">New orientation side of the character</param>
    protected void ChangeSide(FacingSide _newSide)
    {
        currentSide = _newSide;
        // Change Animator State
        CharacterAnimator.SetInteger("OrientationState", (int)_newSide); 
    }

    /// <summary>
    /// Ask the master RPC manager to attack with the attack ID
    /// </summary>
    /// <param name="_attackID">ID of the attack </param>
    protected void CallHit(int _attackID)
    {
        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("LaunchAttack", PhotonTargets.MasterClient, PhotonViewElementID, _attackID); 
    }

    /// <summary>
    /// Check what the character has in his attackbox with the id "_attackID"
    /// </summary>
    /// <param name="_attackID"> ID of the attack </param>
    /// <returns></returns>
    public virtual Dictionary<int, int> CheckHit(int _attackID)
    {
        if (!PhotonNetwork.isMasterClient) return null;
        // Get the right box         
        TDS_AttackBox _box = attackBoxes.Where(b => b.ID == _attackID).FirstOrDefault();
        if (_box == null)
        {
            TDS_CustomDebug.CustomDebugLog($"Attack Box n°{_attackID} can't be found on {this.name}");
            return null;
        }
        return _box.RayCastAttack(); 
    }

    /// <summary>
    /// Apply effects of the attack with the ID
    /// </summary>
    /// <param name="_attackId">ID of the attack </param>
    public abstract void Hit(int _attackId);

    /// <summary>
    /// Set the destination of this character to a new position
    /// </summary>
    /// <param name="_position">New destination position of the character</param>
    protected virtual void SetDestination(Vector3 _position)
    {
        navMeshCharacter.SetDestination(_position);
    }

    #region Combats
    protected abstract void AttackOne();
    protected abstract void AttackThree();
    protected abstract void AttackTwo();

    /// <summary>
    /// Makes the character interact with an object :
    ///     - If the player does not have an object in hand, take the nearest of the objects that can be taken in range
    ///     - If he does have an object in hand, throw it in front of him
    /// </summary>
    protected virtual void InterractWithObjects()
    {

    }
    #endregion

    #region NET methods
    protected void NetSetOnlinePosition(Vector3 _onlinePos)
    {
        //SUIVRE LA POSITION DU PLAYER LOCAL AVEC LE ONPHOTONSERIALIZEVIEW
        transform.position = Vector3.Lerp(transform.position, _onlinePos, Time.deltaTime * speed); 

        //OU ENVOYER LE PATH AVEC UNE REQUËTE RPC SOUS FORME D'UNE CHAINE STRING POUR QUE TOUS LES PLAYERS SUIVENT LE PATH DONNÉ PAR LE MASTERMANAGER
    }

    //ON PHOTON SERIALIZE VIEW
    protected override void OnPhotonSerializeView(PhotonStream _stream, PhotonMessageInfo _messageInfo)
    {
        base.OnPhotonSerializeView(_stream, _messageInfo); 
        if(_stream.isWriting)
        {
            _stream.SendNext(transform.position.x);
            _stream.SendNext(transform.position.y);
            _stream.SendNext(transform.position.z);
            _stream.SendNext((int)currentSide); 
        }
        else if(_stream.isReading)
        {
            float _posX = (float)_stream.ReceiveNext();
            float _posY= (float)_stream.ReceiveNext();
            float _posZ = (float)_stream.ReceiveNext();
            FacingSide _side = (FacingSide)_stream.ReceiveNext();
            Vector3 _onlinePos = new Vector3(_posX, _posY, _posZ);
            NetSetOnlinePosition(_onlinePos);
            ChangeSide(_side); 
        }
    }

    #endregion

    #endregion

    #region UnityMethods
    private void OnDrawGizmos()
    {
        foreach (TDS_AttackBox _box in attackBoxes)
        {
            if (_box.IsVisible)
            {
                Gizmos.color = _box.BoxColor;
                Gizmos.DrawCube(transform.position + _box.CenterPosition, _box.ExtendPosition);
            }
        }
    }

    void Start () 
    {
    	
    }
    
    void Update () 
    {
    	
    }
    #endregion
}

/// <summary>
/// Enum indicating the possible side to look
/// </summary>
public enum FacingSide
{
    Bottom = 0, 
    Left = 1 , 
    Right = 2, 
    Top = 3
}
