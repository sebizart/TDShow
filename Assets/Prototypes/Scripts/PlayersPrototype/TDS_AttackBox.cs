using System;
using System.Collections;
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

public class TDS_AttackBox : MonoBehaviour 
{

    #region Fields/Properties
    [SerializeField, Header("string")] private string boxName = "New Attack Box";
    [SerializeField, Header("Int")] private int id;
    public int ID { get { return id;  } }
    [SerializeField] private int maxDamages;
    [SerializeField] private int minDamages;
    [SerializeField, Header("Box")] private Vector3 centerPosition;
    [SerializeField] private Vector3 extendPosition; 
    #endregion
    
    #region Methods
    public Dictionary<int, int> RayCastAttack()
    {
        int[] _elements =  Physics.OverlapBox(centerPosition, extendPosition).Select(c => c.GetComponent<TDS_DamageableElement>()).ToArray().Where(e => e != null).Select(e => e.PhotonViewElementID).ToArray();
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
