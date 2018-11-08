using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEditor; 

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

    [SerializeField] int percentageSelection = 50;
    public int PercentageSelection { get { return percentageSelection; } set { percentageSelection = value; } }

    [SerializeField] int minSpawning = 1; 
    public int MinSpawning { get { return minSpawning; } set { minSpawning = value; } }

    [SerializeField] int maxSpawning = 2; 
    public int MaxSpawning { get { return maxSpawning; } set { maxSpawning = value; } }

    [SerializeField] bool isOnFightingArea = false;
    public bool IsOnFightingArea { get { return isOnFightingArea; } set { isOnFightingArea = value; } }

    [SerializeField] bool isSelected = false;
    public bool IsSelected { get { return isSelected; } set { isSelected = value; } }

    [SerializeField] float spawningRange = 1;
    public float SpawningRange { get { return spawningRange; } set { spawningRange = value; } }

    [SerializeField] List<TDS_SpawningElement> enemiesSpawnable = new List<TDS_SpawningElement>();
    public List<TDS_SpawningElement> EnemiesSpawnable { get { return enemiesSpawnable; } }

    #region EditorSettings
    [SerializeField] Color spawnPointColor = Color.red;
    public Color SpawnPointColor { get { return spawnPointColor; } set { spawnPointColor = value; } }

    public bool IsFoldOut = false; 
    #endregion

    #endregion

    #region Constructor
    public TDS_SpawnPoint(int _id)
    {
        id = _id;
        name = "Spawn Point " + id;
        enemiesSpawnable.Clear(); 
    }
    #endregion

    #region Methods
    public TDS_Enemy GetEnemy()
    {
        if (!PhotonNetwork.isMasterClient) return null;
        //ESSAYER DE VOIR POUR FAIRE SPAWN UN TYPE D'ENEMY EN PARTICULIER
        //Pour le moment, random enemy parmis les ennemis avec une chance plus ou moins grande 
        //Get the max percentage of spawning
        float _max = enemiesSpawnable.OrderBy(e => e.SpawningChance).LastOrDefault().SpawningChance;
        //Get a random value between 0 and max value
        float _value = UnityEngine.Random.Range(0, _max);
        //Get all enemies with chance greater than value
        TDS_Enemy[] _enemies = enemiesSpawnable.Where(e => e.SpawningChance >= _value).Select(e => e.SpawningEnemy).ToArray();
        //Return a random enemy within enemies with a spawn chance greater than the random value
        int _enemyIndex = UnityEngine.Random.Range(0, _enemies.Length - 1);
        return _enemies[_enemyIndex];
    }
    #endregion 
}

[Serializable]
public class TDS_SpawningElement
{
    private bool isFoldOut = false; 
    public bool IsFoldOut { get {return isFoldOut; } set {isFoldOut = value; } }


    [SerializeField] TDS_Enemy spawningEnemy; 
    public TDS_Enemy SpawningEnemy { get { return spawningEnemy; } set { spawningEnemy = value; } }

    [SerializeField] int spawningChance = 50; 
    public int SpawningChance { get { return spawningChance; } set { spawningChance = value; } }
    
    public TDS_SpawningElement()
    {
        spawningEnemy = null;
        spawningChance = 50; 
    }
}