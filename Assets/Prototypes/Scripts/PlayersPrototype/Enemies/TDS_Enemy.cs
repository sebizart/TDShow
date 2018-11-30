using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

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
    #region Action
    
    #endregion

    #region Fields/Properties
    [Header("Enemy")]
    #region Enums
    [Header("Prefab Name")]
    [SerializeField] private EnemyName prefabName = EnemyName.Acrobat;
    public EnemyName PrefabName { get { return prefabName; } }
    [Header("Enemy State")]
    [SerializeField] private EnemyState currentState = EnemyState.Searching;
    public EnemyState CurrentState { get { return currentState; } }
    #endregion
    #region NavMeshAgent
    [Header("NavMeshAgent")]
    [SerializeField] TDS_NavMeshAgent navMeshAgent;
    public TDS_NavMeshAgent NavMeshAgent { get { return navMeshAgent; } }
    #endregion
    #region owner
    [Header("Owner")]
    [SerializeField] private TDS_FightingArea ownerArea;
    public TDS_FightingArea OwnerArea {get {return ownerArea;}}
    #endregion
    #region Detection
    [Header("Cone of sight")]
    [SerializeField] ConeOfSight enemySight = new ConeOfSight(); 
    public ConeOfSight EnemySight { get { return enemySight; } }
    [Header("Target")]
    [SerializeField] TDS_DamageableElement currentTarget; 
    public TDS_DamageableElement CurrentTarget { get { return currentTarget; } }
    #endregion 
    [Header("Bool")]
    [SerializeField] bool isCalculatingPath = false;
    #endregion

    #region Methods
    protected override void AttackOne()
    {
    }

    protected override void AttackThree()
    {
    }

    protected override void AttackTwo()
    {
    }

    protected override void SetDestination(Vector3 _position)
    {
    }

    public void SetOwner(TDS_FightingArea _owner)
    {
        if (ownerArea != null) return;
        ownerArea = _owner; 
    }

    //

    /// <summary>
    /// Behaviour of the enemy
    /// Call every .5 seconds
    /// Determine what to do using the current state of the enemy
    /// </summary>
    protected void Behaviour()
    {
        switch (currentState)
        {
            case EnemyState.Searching:
                Search();
                break;
            case EnemyState.CalculatingPath:
                if(isCalculatingPath) break; 
                StartCoroutine("CheckDestination");
                break;
            case EnemyState.GettingInRange:
                StartMovement();
                break;
            case EnemyState.Attacking:
                Debug.Log("canAttack"); 
                break;
            case EnemyState.Escaping:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Change the state of the enemy to searching
    /// Called on the navMeshAgent's OnDestinationReached event
    /// </summary>
    private void EndMoving() => currentState = EnemyState.Searching;

    /// <summary>
    /// If the enemy isn't moving or if the path is ready 
    /// Start the nav mesh agent moving method
    /// </summary>
    private void StartMovement()
    {
        if (navMeshAgent.PathState != CalculatingState.Ready  || navMeshAgent.IsMoving == true) return;
        StartCoroutine(navMeshAgent.FollowPath(speed)); 
    }

    /// <summary>
    /// Detect all damageable elements and get these who are in its cone of sight
    /// Get the closest one and set its state to calculating path
    /// </summary>
    private void Search()
    {
        TDS_DamageableElement[] _elements = Physics.OverlapSphere(transform.position, enemySight.DetectionRange).Select(c => c.GetComponent<TDS_DamageableElement>()).Where(e => e != null).Where(e => e != this).ToArray();
        if (_elements.Length == 0) return;
        bool[] _detected = new bool[_elements.Length];
        for (int i = 0; i < _detected.Length; i++)
        {
            Vector3 _direction = _elements[i].transform.position - transform.position;
            Vector3 _ref = facingSide == FacingSide.Right ? Vector3.right : Vector3.left;
            if (Vector3.Angle(_direction, _ref) < enemySight.DetectionAngle)
            {
                _detected[i] = true;
            }
            else _detected[i] = false;
        }
        if (!_detected.Any(d => d == true)) return; 
        List<TDS_DamageableElement> _targetedElements = new List<TDS_DamageableElement>();
        for (int i = 0; i < _elements.Length; i++)
        {
            if(_detected[i])
            {
                _targetedElements.Add(_elements[i]); 
            }
        }
        currentTarget = _targetedElements.OrderBy(e => Vector3.Distance(e.transform.position, transform.position)).FirstOrDefault();
        currentState = EnemyState.CalculatingPath; 
    }

    /// <summary>
    /// Calculate path to reach the target
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckDestination()
    {

        if (!currentTarget && navMeshAgent.PathState != CalculatingState.Waiting) yield break;
        isCalculatingPath = true; 
        if(navMeshAgent.PathState == CalculatingState.Waiting)
        {
            navMeshAgent.SetDestination(currentTarget.transform.position);
            while(navMeshAgent.PathState == CalculatingState.Calculating)
            {
                yield return new WaitForEndOfFrame(); 
            }
        }
        currentState = EnemyState.GettingInRange; 
        isCalculatingPath = false;
        yield break; 
    }

    private void RemoveEnemyFromFightingArea()
    {
        ownerArea.RemoveEnemy(this); 
    }
    #endregion

    #region UnityMethods
    private void Awake()
    {
        navMeshAgent.OnDestinationReached += EndMoving;
        OnDiying += RemoveEnemyFromFightingArea;
    }

    protected override void Start () 
	{
		base.Start();
        //InvokeRepeating("Behaviour", 0, .5f); 
	}
	
	protected override void Update () 
	{
		base.Update();
	}

    protected override void OnDrawGizmos()
    {
        switch (currentState)
        {
            case EnemyState.Searching:
                Gizmos.color = Color.blue; 
                break;
            case EnemyState.GettingInRange:
                Gizmos.color = Color.green;
                break;
            case EnemyState.Attacking:
                Gizmos.color = Color.red;
                break;
            case EnemyState.Escaping:
                Gizmos.color = Color.yellow;
                break;
            default:
                break;
        }
        Gizmos.DrawSphere(transform.position + Vector3.up, .2f);
        Gizmos.color = Color.red;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-enemySight.DetectionAngle /2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(enemySight.DetectionAngle / 2, Vector3.up);
        Vector3 leftRayDirection = facingSide == FacingSide.Right ?  leftRayRotation * transform.right : leftRayRotation * -transform.right;
        Vector3 rightRayDirection = facingSide == FacingSide.Right ? rightRayRotation * transform.right : rightRayRotation * -transform.right;
        Gizmos.DrawRay(transform.position, leftRayDirection * enemySight.DetectionRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * enemySight.DetectionRange);
    }
    #endregion
}

[Serializable]
public class ConeOfSight
{
    [SerializeField, Range(1, 15)] float detectionRange = 1;
    public float DetectionRange { get { return detectionRange; } }
    [SerializeField, Range(1, 180)] float detectionAngle = 35; 
    public float DetectionAngle { get { return detectionAngle;  } }
}

public enum EnemyName
{
    Acrobat, 
    Fakir, 
    MightyMan, 
    Mime, 
    Punk, 
    TmpManequin
}
public enum EnemyState
{
    Searching, 
    CalculatingPath,
    GettingInRange, 
    Attacking, 
    Escaping
}
