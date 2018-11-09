using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary;

[CustomEditor(typeof(TDS_SpawnPoint))]
public class TDS_SpawnPointEditor : Editor
{
    #region Field and properties
    private TDS_SpawnPoint p_target;
    #endregion


    #region Methods
    private void OnEnable()
    {
        p_target = (TDS_SpawnPoint)target;
    }
    public override void OnInspectorGUI()
    {
        ShowPointSettings(); 
    }
    private void OnSceneGUI()
    {
        Handles.color = p_target.SpawnPointColor;
        Handles.DrawWireDisc(p_target.SpawnPosition, Vector3.up, p_target.SpawningRange);
    }

    /// <summary>
    /// Show and set Spawn point position
    /// Show and set if the point is on fighting area
    /// Show and set enemies spawnable
    /// Select and delete point
    /// </summary>
    /// <param name="p_target"> SpawnPoint</param>
    /// <param name="_index"> Point Index</param>
    void ShowPointSettings()
    {

        p_target.IsFoldOut = EditorGUILayout.Foldout(p_target.IsFoldOut, p_target.PointName, true);
        if (p_target.IsFoldOut)
        {
            //SpawnPosition -> Vector 3
            p_target.transform.position = EditorGUILayout.Vector3Field("Spawn Position", p_target.transform.position);
            //IsOnFightingArea -> bool
            p_target.IsOnFightingArea = EditorGUILayout.ToggleLeft("Is on fightingArea", p_target.IsOnFightingArea);
            //Range -> Float 
            p_target.SpawningRange = EditorGUILayout.Slider("Spawning Range", p_target.SpawningRange, .1f, 10);
            //MinSpawn
            p_target.MinSpawning = EditorGUILayout.IntSlider("Min Spawn", p_target.MinSpawning, 1, p_target.MaxSpawning);
            //MaxSpawn
            p_target.MaxSpawning = EditorGUILayout.IntSlider("Max Spawn", p_target.MaxSpawning, p_target.MinSpawning, 10);
            //PERCENTAGE 
            p_target.PercentageSelection = EditorGUILayout.IntSlider("Selection percentage", p_target.PercentageSelection, 1, 100);
            //Enemies Spawnables -> List<SpawningElement>
            EditorGUILayout.HelpBox("Spawning elements", MessageType.None);
            if (p_target.EnemiesSpawnable.Count > 0)
            {
                for (int i = 0; i < p_target.EnemiesSpawnable.Count; i++)
                {
                    if (p_target.EnemiesSpawnable[i] != null)
                        ShowSpawnElementSettings(p_target.EnemiesSpawnable[i]);
                }
            }
            GUITools.ActionButton("Add spawnableElement", AddSpawningElement, p_target, Color.white, Color.black);
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Color", MessageType.None);
            //DebugColor -> Color
            p_target.SpawnPointColor = EditorGUILayout.ColorField(p_target.SpawnPointColor);
        }
    }

    /// <summary>
    /// Show and set spawning enemy
    /// Show and set spawn chance
    /// </summary>
    /// <param name="_element">Spawning Element</param>
    void ShowSpawnElementSettings(TDS_SpawningElement _element)
    {
        _element.IsFoldOut = EditorGUILayout.Foldout(_element.IsFoldOut, _element.SpawningEnemy != null ? _element.SpawningEnemy.name : "Enemy", true);
        if (_element.IsFoldOut)
        {
            _element.SpawningEnemy = EditorGUILayout.ObjectField("Enemy Type", _element.SpawningEnemy, typeof(TDS_Enemy), false) as TDS_Enemy;
            _element.SpawningChance = EditorGUILayout.IntSlider("Spawn chance", _element.SpawningChance, 0, 100);
        }
    }

    #region SpawningElements
    void AddSpawningElement(TDS_SpawnPoint _point)
    {
        _point.EnemiesSpawnable.Add(new TDS_SpawningElement());
    }
    void RemoveSpawningElement(TDS_SpawnPoint _point, int _index)
    {
        _point.EnemiesSpawnable.RemoveAt(_index);
    }
    #endregion

    #endregion
}
