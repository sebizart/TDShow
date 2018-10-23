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
    [SerializeField] private int minDamages;
    [SerializeField, Header("Box")] private Vector3 centerPosition;
    public Vector3 CenterPosition { get { return centerPosition; } }
    [SerializeField] private Vector3 extendPosition;
    public Vector3 ExtendPosition { get { return extendPosition; } }

    // Gizmo utilities
    [SerializeField] private bool isVisible = false;
    public bool IsVisible { get { return isVisible; } }
    [SerializeField] private Color boxColor = Color.green;
    public Color BoxColor { get { return boxColor; } }
    #endregion
    
    #region Methods
    public Dictionary<int, int> RayCastAttack()
    {
        // Créer un layer pour chaque type et ne pas prendre en compte le type de layer de l'attaquant (pour éviter que les players se tapent entre eux)
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
    public Dictionary<int, int> RayCastAttack(float _beardOffset)
    {
        // Créer un layer pour chaque type et ne pas prendre en compte le type de layer de l'attaquant (pour éviter que les players se tapent entre eux)
        Vector3 _centerPosition = new Vector3(centerPosition.x * _beardOffset, centerPosition.y, centerPosition.z);
        Vector3 _extendPosition = new Vector3(extendPosition.x * _beardOffset, extendPosition.y, extendPosition.z);
        int[] _elements = Physics.OverlapBox(_centerPosition, _extendPosition).Select(c => c.GetComponent<TDS_DamageableElement>()).ToArray().Where(e => e != null).Select(e => e.PhotonViewElementID).ToArray();
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
}
