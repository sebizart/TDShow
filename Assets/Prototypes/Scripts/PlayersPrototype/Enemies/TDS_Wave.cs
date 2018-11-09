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

    [SerializeField] List<TDS_WaveElement> fixElements = new List<TDS_WaveElement>(); 
    public List<TDS_WaveElement> FixElement { get { return fixElements;  } set { fixElements = value; } }

    public void AddInfo()
    {
        fixElements.Add(new TDS_WaveElement()); 
    }
    public void RemoveInfoAt(int _index)
    {
        fixElements.RemoveAt(_index); 
    }

}

[Serializable]
public class TDS_WaveElement
{
    public bool IsFoldOut { get; set; }

    //POINT LINKED
    [SerializeField] TDS_SpawnPoint linkedPoint;
    public TDS_SpawnPoint LinkedPoint { get { return linkedPoint; } set { linkedPoint = value; } }

    //IS RANDOM 
    [SerializeField] bool isRandom = false;
    public bool IsRandom { get { return isRandom; } set { isRandom = value; } }

    //LINKED ENEMY
    [SerializeField] List<TDS_Enemy> spawningEnemies = new List<TDS_Enemy>();
    public List<TDS_Enemy> SpawningEnemies { get { return spawningEnemies; } set { spawningEnemies = value; } }

    //NUMBER OF ENEMIES
    [SerializeField] int numberEnemy;
    public int NumberEnemy { get { return numberEnemy; } set { numberEnemy = value; } }


    public void RemoveEnemyAt(int _i)
    {
        spawningEnemies.RemoveAt(_i); 
    }
}
