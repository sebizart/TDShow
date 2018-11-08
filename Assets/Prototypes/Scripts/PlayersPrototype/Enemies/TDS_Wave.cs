using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] TDS_Wave Version 0.0.1
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
public class TDS_Wave
{
    public bool IsFoldOut { get; set; }

    [SerializeField] bool isScripted = false;
    public bool IsScripted { get { return isScripted; } set { isScripted = value; } }

    [SerializeField] List<TDS_SpawnPoint> linkedPoints = new List<TDS_SpawnPoint>(); 
    public List<TDS_SpawnPoint> LinkedPoints { get { return linkedPoints;  } set { linkedPoints = value; } }

    [SerializeField] bool[] selectedPointsInBool; 
    public bool[] SelectedPointsInBool { get { return selectedPointsInBool; } set { selectedPointsInBool = value; } }
    


}
