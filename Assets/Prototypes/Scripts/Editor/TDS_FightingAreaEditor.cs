using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary; 

/*
[Script Header] TDS_FightingAreaEditor Version 0.0.1
Created by:
Date: 
Description: Editor for the Fighting Area
Show editable values into its Inspector and with Handles on the screen

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/


[CustomEditor(typeof(TDS_FightingArea))]
public class TDS_FightingAreaEditor : Editor
{

    #region FieldsAndProperties
    private TDS_FightingArea p_target;
    private TDS_SpawnPoint selectedPoint = null;
    [SerializeField] private int spawnPointID = 0; 
    #endregion

    #region Methods
    
    /// <summary>
    /// Show and set Detection state
    /// Show and set Remaining waves
    /// Show and set Area color
    /// </summary>
    void ShowFigthingAreaSettings()
    {
        EditorGUILayout.HelpBox("DETECTION AREA", MessageType.None);
        //DETECTION STATE 
        p_target.DetectionState = (SpawnPointState)EditorGUILayout.EnumPopup("Detection State",p_target.DetectionState);
        //REMAINING WAVES
        p_target.RemainingWaves = EditorGUILayout.IntSlider("Remaining waves",p_target.RemainingWaves, 1, 15);
        //DETECTION AREA
        p_target.DetectionArea.DebugColor = EditorGUILayout.ColorField("Debug color", p_target.DetectionArea.DebugColor);
    }

    /// <summary>
    /// Show and set Spawn point position
    /// Show and set if the point is on fighting area
    /// Show and set enemies spawnable
    /// Select and delete point
    /// </summary>
    /// <param name="_point"> SpawnPoint</param>
    /// <param name="_index"> Point Index</param>
    void ShowPointSettings(TDS_SpawnPoint _point, int _index)
    {
        
        _point.IsFoldOut = EditorGUILayout.Foldout(_point.IsFoldOut, _point.Name, true);
        if(_point.IsFoldOut)
        {
            //SpawnPosition -> Vector 3
            _point.SpawnPosition = EditorGUILayout.Vector3Field("Spawn Position",_point.SpawnPosition);
            //IsOnFightingArea -> bool
            _point.IsOnFightingArea = EditorGUILayout.ToggleLeft("Is on fightingArea", _point.IsOnFightingArea);
            //Range -> Float 
            _point.SpawningRange = EditorGUILayout.Slider("Spawning Range",_point.SpawningRange, .1f, 10);
            //MinSpawn
            _point.MinSpawning = EditorGUILayout.IntSlider("Min Spawn", _point.MinSpawning, 1, _point.MaxSpawning);
            //MaxSpawn
            _point.MaxSpawning = EditorGUILayout.IntSlider("Max Spawn", _point.MaxSpawning, _point.MinSpawning, 10);
            //PERCENTAGE 
            _point.PercentageSelection = EditorGUILayout.IntSlider("Selection percentage", _point.PercentageSelection, 1, 100);
            //Enemies Spawnables -> List<SpawningElement>
            if (_point.EnemiesSpawnable.Count > 0)
            {
               for (int i = 0; i < _point.EnemiesSpawnable.Count; i++)
               {
                    if(_point.EnemiesSpawnable[i] != null)
                        ShowSpawnElementSettings(_point.EnemiesSpawnable[i]);
               }
            }
            GUITools.ActionButton("Add spawnableElement", AddSpawningElement, _point, Color.white, Color.black); 
            EditorGUILayout.Space();
            //DebugColor -> Color
            _point.SpawnPointColor = EditorGUILayout.ColorField(_point.SpawnPointColor);
            EditorGUILayout.BeginHorizontal();
            GUITools.ActionButton("Select", SelectSpawnPoint, _point, Color.white, Color.black);
            GUITools.ActionButton("Delete", p_target.RemovePointAt, _index, Color.white, Color.black);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("", MessageType.None); 
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
        if(_element.IsFoldOut)
        {
            _element.SpawningEnemy = EditorGUILayout.ObjectField("Enemy Type",_element.SpawningEnemy, typeof(TDS_Enemy), false) as TDS_Enemy;
            _element.SpawningChance = EditorGUILayout.IntSlider("Spawn chance",_element.SpawningChance, 0, 100);
        }
    }

    /// <summary>
    /// Add points 
    /// Clear points
    /// show every points settings
    /// </summary>
    void ShowSpawnPointsSettings()
    {
        EditorGUILayout.HelpBox("SPAWN POINTS", MessageType.None);
        EditorGUILayout.Space();
        for (int i = 0; i < p_target.SpawnPoints.Count; i++)
        {
            ShowPointSettings(p_target.SpawnPoints[i], i);
        }
        EditorGUILayout.BeginHorizontal();
        GUITools.ActionButton("Add Spawn Point", IncrementSpawnPoint, Color.white, Color.black);
        GUITools.ActionButton("Clear all points", ClearPoints, Color.white, Color.black);
        EditorGUILayout.EndHorizontal(); 
    }

    /// <summary>
    /// 
    /// </summary>
    void ShowWavesSettings()
    {
        EditorGUILayout.HelpBox("WAVES", MessageType.None); 
        for (int i = 0; i < p_target.Waves.Count; i++)
        {
            ShowWave(p_target.Waves[i], i);
        }
        EditorGUILayout.BeginHorizontal(); 
        GUITools.ActionButton("Add Wave", AddWave, Color.white, Color.black);
        GUITools.ActionButton("Update points", UpdatePoints, Color.white, Color.black);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_wave"></param>
    /// <param name="_index"></param>
    void ShowWave(TDS_Wave _wave, int _index)
    {

        _wave.IsFoldOut = EditorGUILayout.Foldout(_wave.IsFoldOut, "Wave", true);
        if(_wave.IsFoldOut)
        {
            _wave.IsScripted = EditorGUILayout.Toggle("Is scripted", _wave.IsScripted);
            if(_wave.IsScripted)
            {
                if(_wave.SelectedPointsInBool == null||_wave.SelectedPointsInBool.Length != p_target.SpawnPoints.Count)
                {
                    UpdatePoints(); 
                }
                for (int i = 0; i < _wave.SelectedPointsInBool.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ColorField(p_target.SpawnPoints[i].SpawnPointColor, GUILayout.Width(45));
                    _wave.SelectedPointsInBool[i] = EditorGUILayout.Toggle(p_target.SpawnPoints[i].Name, _wave.SelectedPointsInBool[i]);
                    EditorGUILayout.EndHorizontal();
                }
                GUITools.ActionButton("Save points", SavePoints,_wave, Color.white, Color.black);
            }
            GUITools.ActionButton("Remove Wave", RemoveWave, _index, Color.white, Color.black);
        }

    }

    void UpdatePoints()
    {
        foreach (TDS_Wave wave in p_target.Waves)
        {
            bool[] _tmp = wave.SelectedPointsInBool;
            wave.SelectedPointsInBool = new bool[p_target.SpawnPoints.Count];
            if (_tmp == null) return; 
            for (int i = 0; i < _tmp.Length; i++)
            {
                wave.SelectedPointsInBool[i] = _tmp[i]; 
            }
        }
    }
    void SavePoints(TDS_Wave _wave)
    {
        _wave.LinkedPoints.Clear();
        for (int i = 0; i < p_target.SpawnPoints.Count; i++)
        {
            if(_wave.SelectedPointsInBool[i] == true)
            {
                _wave.LinkedPoints.Add(p_target.SpawnPoints[i]); 
            }
        }
    }


    #region Points
    /// <summary>
    /// Add spawn point
    /// </summary>
    void IncrementSpawnPoint()
    {
        spawnPointID++;
        p_target.AddSpawnPoint(spawnPointID); 
    }
    /// <summary>
    /// Select spawn point
    /// </summary>
    /// <param name="_point">selected point</param>
    void SelectSpawnPoint(TDS_SpawnPoint _point) => selectedPoint = _point; 
    /// <summary>
    /// Clear all points
    /// </summary>
    void ClearPoints()
    {
        spawnPointID = 0;
        p_target.SpawnPoints.Clear(); 
    }
    #endregion

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

    #region Waves 
    void AddWave()
    {
        p_target.Waves.Add(new TDS_Wave());
    }
    void RemoveWave(int _index)
    {
        p_target.Waves.RemoveAt(_index);
    }
    #endregion

    #endregion

    #region UnityMethods
    private void OnEnable()
    {
        p_target = (TDS_FightingArea)target; 
    }

    public override void OnInspectorGUI()
    {
        ShowFigthingAreaSettings();
        ShowWavesSettings();
        ShowSpawnPointsSettings(); 
    }

    private void OnSceneGUI()
    {
        // POINTS RANGE
        for (int i = 0; i < p_target.SpawnPoints.Count; i++)
        {
            Handles.color = p_target.SpawnPoints[i].SpawnPointColor;
            Handles.DrawWireDisc(p_target.SpawnPoints[i].SpawnPosition, Vector3.up, p_target.SpawnPoints[i].SpawningRange); 
        }
        // POINT POSITION
        if (selectedPoint != null)
        {
            selectedPoint.SpawnPosition = Handles.PositionHandle(selectedPoint.SpawnPosition, Quaternion.identity);
        }

        // DETECTION AREA
        p_target.DetectionArea.CenterPosition = Handles.PositionHandle(p_target.DetectionArea.CenterPosition, Quaternion.identity);
        float _scaleX = p_target.DetectionArea.ExtendedX;
        float _scaleY = p_target.DetectionArea.ExtendedY;
        float _scaleZ = p_target.DetectionArea.ExtendedZ; 
        EditorGUI.BeginChangeCheck();
        Handles.color = p_target.DetectionArea.DebugColor; 
        _scaleX = Handles.ScaleSlider(_scaleX, p_target.DetectionArea.CenterPosition, Mathf.Abs(_scaleX) <= 1? Vector3.right : Vector3.right * _scaleX,  Quaternion.identity, .5f, .5f );
        _scaleY = Handles.ScaleSlider(_scaleY, p_target.DetectionArea.CenterPosition, Mathf.Abs(_scaleY) <= 1 ? Vector3.up : Vector3.up * _scaleY, Quaternion.identity, .5f, .5f);
        _scaleZ = Handles.ScaleSlider(_scaleZ, p_target.DetectionArea.CenterPosition, Mathf.Abs(_scaleZ) <= 1 ? Vector3.forward : Vector3.forward * _scaleZ, Quaternion.identity, .5f, .5f);
        if (EditorGUI.EndChangeCheck())
        {
            p_target.DetectionArea.UpdateAreaScale(_scaleX, _scaleY, _scaleZ); 
        }

    }

    #endregion 

}


