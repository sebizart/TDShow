using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] TDS_WavePoint Version 0.0.1
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
public class TDS_WavePoint
{
    #region Fields and Properties
    public string Name { get; set; }

    [SerializeField] int waveNumber = 0; 
    public int WaveNumber { get { return waveNumber; } set { waveNumber = value; } }

    [SerializeField] Vector3 position = new Vector3(); 
    public Vector3 Position { get { return position; } set { position = value; } }

    [SerializeField] float range = 1; 
    public float Range { get { return range; } set { range = value; } }

    [SerializeField] TDS_WaveElement staticWaveElement = new TDS_WaveElement(); 
    public TDS_WaveElement StaticWaveElement { get { return staticWaveElement; } set { staticWaveElement = value; } }

    [SerializeField] TDS_RandomWaveElement randomWaveElement = new TDS_RandomWaveElement();
    public TDS_RandomWaveElement RandomWaveElement { get { return randomWaveElement; } set { randomWaveElement = value; } }
    #endregion

    #region Constructor
    public TDS_WavePoint(int _id)
    {
        Name = "Wave Point n°" + _id;
    }
    #endregion


    #region Methods

    public List<TDS_SpawningInformation> GetSpawnInformations()
    {
        List<TDS_SpawningInformation> _informations = new List<TDS_SpawningInformation>();
        _informations.AddRange(staticWaveElement.GetInformations(position, range));
        _informations.AddRange(randomWaveElement.GetInformations(position, range)); 
        return _informations;
    }
    #endregion

}
