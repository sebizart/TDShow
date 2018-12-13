using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 
/*
[Script Header] TDS_WaveElement Version 0.0.1
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
public class TDS_WaveElement
{
    [SerializeField] protected List<TDS_SpawningEnemies> enemiesSpawn = new List<TDS_SpawningEnemies>();
    public List<TDS_SpawningEnemies> EnemiesSpawn { get { return enemiesSpawn; } set { enemiesSpawn = value; } }

    public virtual void AddElement(TDS_Enemy _enemy)
    {
        enemiesSpawn.Add(new TDS_SpawningEnemies(_enemy));
    }

    public virtual void RemoveElement(int _index)
    {
        enemiesSpawn.RemoveAt(_index);
    }

    public virtual List<TDS_SpawningInformation> GetInformations(Vector3 _position, float _range)
    {
        List<TDS_SpawningInformation> _informations = new List<TDS_SpawningInformation>();
        Vector3 _randomPosition;
        //PARCOURIR CHAQUE  DU DICO 
        for (int i = 0; i < enemiesSpawn.Count; i++)
        {
            // POUR CHAQUE ENTRÉE DU DICTIONNAIRE,
            // ON AJOUTERA UN NOMBRE D'INFO EGAL A CHAQUE VALUE DE NOTRE PAIR
            // CRÉER UNE NOUVELLE POSITION ALEATOIRE ET CRÉER UNE SPAWNINGINFORMATION AVEC L'ID DE LA KEY DU DICO A LA POSITION ALEATOIRE
            for (int j = 0; j < enemiesSpawn[i].NumberOfEnemies; j++)
            {
                _randomPosition = _position + new Vector3(Random.Range(-_range, _range), 0, Random.Range(-_range, _range));
                _informations.Add(new TDS_SpawningInformation((int)enemiesSpawn[i].SpawningEnemy.PrefabName, _randomPosition));
            }
        }

        return _informations; 
    }
}
[Serializable]
public class TDS_SpawningEnemies
{
    [SerializeField] TDS_Enemy spawningEnemy; 
    public TDS_Enemy SpawningEnemy { get { return spawningEnemy; } set { spawningEnemy = value; } }
    [SerializeField] int numberOfEnemies = 1;
    public int NumberOfEnemies { get { return numberOfEnemies; } set { numberOfEnemies = value; } }

    public TDS_SpawningEnemies(TDS_Enemy _e)
    {
        spawningEnemy = _e; 
    }
}
