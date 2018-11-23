using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_NavMeshAgent : MonoBehaviour
{
    [SerializeField] TDS_CustomNavPath currentPath;
    [SerializeField] Transform target;
    int indexTarget = 0; 

    bool isMoving = false; 

    private void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (TDS_NavMeshManager.Instance.CalculatePath(transform.position, target.position, currentPath))
            {
                isMoving = true;
            }
        }
        if (!isMoving) return;
        Move(); 
    }

    private void Move()
    {
        Vector3 _targetedPosition = currentPath.NavigationPoints[indexTarget].Position;
        transform.position = Vector3.MoveTowards(transform.position, _targetedPosition , Time.deltaTime * 2); 
        if(Vector3.Distance(transform.position, _targetedPosition ) <= .1f)
        {
            indexTarget++; 
            if(indexTarget == currentPath.NavigationPoints.Count)
            {
                isMoving = false;
                indexTarget = 0;
                currentPath.NavigationPoints.Clear(); 
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        for (int i = 0; i < currentPath.NavigationPoints.Count; i++)
        {
            Gizmos.DrawSphere(currentPath.NavigationPoints[i].Position, .2f); 
        }
        for (int i = 0; i < currentPath.NavigationPoints.Count - 1 ; i++)
        {
            Gizmos.DrawLine(currentPath.NavigationPoints[i].Position, currentPath.NavigationPoints[i + 1].Position); 
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(currentPath.StartPosition, .2f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(currentPath.EndPosition, .2f);

    }
}
