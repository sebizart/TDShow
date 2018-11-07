using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region SpawnPoint
//HEADER
// Spawn point
// Make a spawn for a various number of enemies on a selected position (into or out of the fighting area)
[Serializable]
public class TDS_SpawnPoint
{
    #region FieldAndProperties
    [SerializeField] string name = "SP";
    public string Name { get { return name; } }

    [SerializeField] Vector3 spawnPosition;
    public Vector3 SpawnPosition { get { return spawnPosition; } set { spawnPosition = value; } }

    [SerializeField] int id;
    public int ID { get { return id; } }

    [SerializeField] bool isOnFightingArea = false;
    public bool IsOnFightingArea { get { return isOnFightingArea; } set { isOnFightingArea = value; } }

    [SerializeField] bool isSelected = false;
    public bool IsSelected { get { return isSelected; } set { isSelected = value; } }

    public List<TDS_Enemy> EnemiesSpawnable = new List<TDS_Enemy>();

    #region EditorSettings
    [SerializeField] Color spawnPointColor = Color.red;
    public Color SpawnPointColor { get { return spawnPointColor; } set { spawnPointColor = value; } }
    #endregion

    #endregion

    #region Constructor
    public TDS_SpawnPoint(int _id)
    {
        id = _id;
        name = "Spawn Point " + id;
    }
    #endregion

    #region Methods
    public TDS_Enemy GetEnemy()
    {
        if (!PhotonNetwork.isMasterClient) return null;
        //ESSAYER DE VOIR POUR FAIRE SPAWN UN TYPE D'ENEMY EN PARTICULIER
        //Pour le moment, random enemy
        return EnemiesSpawnable[(int)UnityEngine.Random.Range(0, EnemiesSpawnable.Count - 1)];
    }
    #endregion 
}
#endregion

