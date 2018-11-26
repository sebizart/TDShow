using System;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Random = UnityEngine.Random; 

/*
[Script Header] TDS_AttackBox Version 0.0.1
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

[Serializable]
public class TDS_AttackBox 
{

    #region Fields/Properties
    [SerializeField, Header("string")] private string boxName = "New Attack Box";
    [SerializeField, Header("Int")] private int id;
    public int ID { get { return id;  } }
    [SerializeField] private int maxDamages;
    public int MaxDamages { get { return maxDamages; } }
    [SerializeField] private int minDamages;
    public int MinDamages
    {
        get { return minDamages; }
    }
    [SerializeField, Header("Collider")] private BoxCollider collider;
    public BoxCollider Collider { get { return collider; } }

    // Gizmo utilities
    [SerializeField] private bool isVisible = false;
    public bool IsVisible { get { return isVisible; } }
    [SerializeField] private Color boxColor = Color.green;
    public Color BoxColor { get { return boxColor; } }
    #endregion
    
    #region Methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, int> RayCastAttack()
    {
        if (!collider) return null;

        int[] _elements =  Physics.OverlapBox(collider.transform.TransformPoint(collider.center), Vector3.Scale(collider.size, collider.transform.lossyScale)).Select(c => c.GetComponent<TDS_DamageableElement>()).ToArray().Where(e => e != null).Select(e => e.PhotonViewElementID).ToArray();
        int _damages;
        Dictionary<int, int> _characterDamages = new Dictionary<int, int>();
        foreach (int id in _elements)
        {
            _damages = Random.Range(minDamages, maxDamages);
            _characterDamages.Add(id, _damages); 
        }
        return _characterDamages; 
    }
    #endregion

    #region UnityMethods

    #endregion
}
