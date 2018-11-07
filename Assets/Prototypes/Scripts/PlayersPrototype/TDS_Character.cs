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
    [SerializeField, Header("Bool")] protected bool isGrabObjectBoxVisible = false;

    [SerializeField, Header("Character", order = 0), Header("Bool", order = 1)]protected bool canAttack = true; 
    [SerializeField]protected bool canMove = true;

    [SerializeField, Header("FacingSide")]protected FacingSide facingSide = FacingSide.Right; 
    public FacingSide FacingSide { get { return facingSide; } }

    [SerializeField, Header("Float")] protected float speed = 1;

    [SerializeField, Header("Nav Mesh")] NavMeshAgent navMeshCharacter;
    private Vector3 netOnlinePosition;

    [SerializeField, Header("Projectile")] protected TDS_Throwable projectile;

    [SerializeField, Header("Animator")] protected Animator CharacterAnimator; 

    [SerializeField, Header("AttackBoxes")] protected TDS_AttackBox[] attackBoxes = new TDS_AttackBox[] { };

    [SerializeField, Header("Vector 3")] protected Vector3 grabObjectZoneCenter = Vector3.zero;
    [SerializeField] protected Vector3 grabObjectZoneExtents = Vector3.one;
    #endregion

    #region Methods
    /// <summary>
    /// Makes the character change the side of the screen he's looking, its orientation
    /// </summary>
    /// <param name="_newSide">New orientation side of the character</param>
    protected void ChangeSide(FacingSide _newSide)
    {
        facingSide = _newSide;
        // Change Animator State
        CharacterAnimator.SetInteger("OrientationState", (int)_newSide); 
    }

    protected void CallAction(int _attackID)
    {
        TDS_RPCManager.Instance.RPCManagerPhotonView.RPC("LaunchAction", PhotonTargets.All, PhotonViewElementID, _attackID);
    }

    /// <summary>
    /// Ask the master RPC manager to attack with the attack ID
    /// </summary>
    /// <param name="_attackID">ID of the attack </param>
    protected void CallHit(int _attackID)
    {
        CallAction(_attackID);
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
    /// Apply effects of the action with the ID
    /// </summary>
    /// <param name="_actionID">ID of the action</param>
    public abstract void Action(int _actionID);

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
    #endregion

    #region Object Interaction
    /// <summary>
    /// Grabs an object
    /// </summary>
    /// <param name="_objectID">ID of the object to grab</param>
    public virtual void GrabObject(int _objectID)
    {
        // If the character already have a projectile, return
        if (projectile) return;

        // Find the object via photon ID & get its Throwable component
        PhotonView _objectPhoton = PhotonView.Find(_objectID);

        if (_objectPhoton)
        {
            TDS_Throwable _projectile = _objectPhoton.GetComponent<TDS_Throwable>();

            if (!_projectile)
            {
                TDS_CustomDebug.CustomDebugLogWarning($"The object \"{_objectPhoton.name}\" couldn't be found");
                return;
            }

            // If the character can grab the object, stock it
            if (_projectile.Grab(this))
            {
                projectile = _projectile;
            }
        }
        else
        {
            TDS_CustomDebug.CustomDebugLogWarning($"The object with PhotonID \"{_objectID}\" doesn't have the \"TDS_Throwable\" component !");
            return;
        }
    }

    /// <summary>
    /// Throw the grabed object
    /// </summary>
    public virtual void ThrowObject()
    {
        // If the character doesn't have a projectile to throw, return
        if (!projectile) return;

        // Throw the projectile and remove it from the current weared object
        projectile.Throw(FacingSide);
        projectile = null;
    }

    /// <summary>
    /// Check the object that can be grabed around and take the first
    /// </summary>
    /// <returns>Returns the first object to grab or null if none</returns>
    public virtual TDS_Throwable TryToGrabObject()
    {
        TDS_Throwable _throwable = Physics.OverlapBox(transform.position + grabObjectZoneCenter, grabObjectZoneExtents).Where(o => o.GetComponent<TDS_Throwable>()).Select(o => o.GetComponent<TDS_Throwable>()).FirstOrDefault();

        return _throwable;
    }
    #endregion

    #region NET methods
    protected void NetSetOnlinePosition()
    {
        if (photonViewElement.isMine) return; 
        //SUIVRE LA POSITION DU PLAYER LOCAL AVEC LE ONPHOTONSERIALIZEVIEW
        transform.position = Vector3.Lerp(transform.position, netOnlinePosition, Time.deltaTime * navMeshCharacter.speed); 

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
            _stream.SendNext((int)facingSide); 
        }
        else if(_stream.isReading)
        {
            float _posX = (float)_stream.ReceiveNext();
            float _posY= (float)_stream.ReceiveNext();
            float _posZ = (float)_stream.ReceiveNext();
            FacingSide _side = (FacingSide)_stream.ReceiveNext();
            netOnlinePosition = new Vector3(_posX, _posY, _posZ);
            ChangeSide(_side); 
        }
    }

    #endregion

    #endregion

    #region UnityMethods
    protected virtual void OnDrawGizmos()
    {
        foreach (TDS_AttackBox _box in attackBoxes)
        {
            if (_box.IsVisible)
            {
                Gizmos.color = _box.BoxColor;
                Gizmos.DrawCube(transform.position + _box.CenterPosition, _box.ExtendPosition);
            }
        }

        if (isGrabObjectBoxVisible)
        {
            Gizmos.color = new Color(0, 1, 0, .25f);
            Gizmos.DrawCube(transform.position + grabObjectZoneCenter, grabObjectZoneExtents);
        }
        Gizmos.color = Color.white;
    }

    protected virtual void Start () 
    {
    	
    }
    
    protected virtual void Update () 
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
