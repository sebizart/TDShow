using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 
/*
[Script Header] TDS_RandomWaveElement Version 0.0.1
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
public class TDS_RandomWaveElement : TDS_WaveElement
{
    [SerializeField] List<int> randomValues = new List<int>(); 
    public List<int> RandomValues { get { return randomValues; } set { randomValues = value; } }

    public override void AddElement(TDS_Enemy _enemy)
    {
        base.AddElement(_enemy);
        randomValues.Add(50); 
    }
    public override void RemoveElement(int _index)
    {
        base.RemoveElement(_index);
        randomValues.RemoveAt(_index); 
    }

    /// <summary>
    /// A VOIR AVEC LES AUTRES POUR LES MODALITÉS DE SPAWN
    /// pour le moment, on fait un aléatoire entre 0 et 100, si le tirage est inférieur à la random value, on fait une spawn info 
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_range"></param>
    /// <returns></returns>
    public override List<TDS_SpawningInformation> GetInformations(Vector3 _position, float _range)
    {
        List<TDS_SpawningInformation> _informations = new List<TDS_SpawningInformation>();
        Vector3 _randomPosition;
        // PARCOURIR CHAQUE ENTRÉE DU DICO
        for (int i = 0; i < enemiesSpawn.Count; i++)
        {
            // POUR CHAQUE ENTRÉE DU DICTIONNAIRE,
            // ON AJOUTERA UN NOMBRE D'INFO EGAL A CHAQUE VALUE DE NOTRE PAIR
            for (int j = 0; j <enemiesSpawn[i].NumberOfEnemies; j++)
            {
                float _random = Random.Range(0, 100);
                if (_random < randomValues[i])
                {
                    _randomPosition = _position + new Vector3(Random.Range(-_range, _range), 0, Random.Range(-_range, _range));
                    _informations.Add(new TDS_SpawningInformation((int)enemiesSpawn[i].SpawningEnemy.PrefabName, _randomPosition));
                }
            }
        }
        return _informations;
    }
}
