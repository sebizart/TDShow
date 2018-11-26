using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] TDS_PointsManager Version 0.0.1
Created by:
Date: 
Description: Contains all WavePoints of the figthing area 
Select the wave points that had to be activated

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/
[Serializable]
public class TDS_PointsManager
{
    #region Fields and Properties
    [SerializeField] Vector3 position; 
    public Vector3 Position { get { return position; } set { position = value; } }

    [SerializeField] List<TDS_WavePoint> allWavePoints = new List<TDS_WavePoint>(); 
    public List<TDS_WavePoint> AllWavePoints { get { return allWavePoints; } set { allWavePoints = value; } }
    #endregion

    #region EditorMethods
    public void AddWavePoint()
    {
        allWavePoints.Add(new TDS_WavePoint(allWavePoints.Count + 1)); 
    }
    public void RemoveWavePoint(int _index)
    {
        allWavePoints.RemoveAt(_index); 
    }
    #endregion


    #region Methods
    public List<TDS_SpawningInformation> GetSpawningInformations(int _waveNumber)
    {
        List<TDS_SpawningInformation> _informations = new List<TDS_SpawningInformation>();
        List<TDS_WavePoint> _wavePoints = allWavePoints.FindAll(p => _waveNumber == p.WaveNumber); 
        if(_wavePoints.Count == 0)
        {
            return null;
        }
        foreach (TDS_WavePoint point in _wavePoints)
        {
            _informations.AddRange(point.GetSpawnInformations()); 
        }
        return _informations; 

    }
    #endregion
}

[Serializable]
public class TDS_SpawningInformation
{
    public int PrefabId { get; set; } 
    public Vector3 SpawnPosition { get; set; }

    public TDS_SpawningInformation(int _id, Vector3 _pos)
    {
        PrefabId = _id;
        SpawnPosition = _pos; 
    }
}