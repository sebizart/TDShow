using System.Collections;
using System.Collections.Generic;
using System.Linq; 
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
    //private TDS_SpawnPoint selectedPoint = null;
    [SerializeField] private int spawnPointID = 0;

    public bool HasToBeUpdated { get { return (p_target.SpawnPoints.Count > 0 && p_target.SpawnPoints.Any(p => p == null)); } }
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
    /// Add points 
    /// Clear points
    /// show every points settings
    /// </summary>
    void ShowSpawnPointsSettings()
    {
        p_target.ShowSpawnPoints = EditorGUILayout.Foldout(p_target.ShowSpawnPoints, "SPAWN POINTS", true); 
        if(HasToBeUpdated)
        {
            p_target.UpdatePoints(); 
            return;
        }
        if(p_target.ShowSpawnPoints)
        {
            EditorGUILayout.Space();
            for (int i = 0; i < p_target.SpawnPoints.Count; i++)
            {
                EditorGUILayout.HelpBox(p_target.SpawnPoints[i].name, MessageType.None);
                EditorGUILayout.BeginHorizontal();
                GUITools.ActionButton("Select", SelectSpawnPoint, p_target.SpawnPoints[i], Color.white, Color.black);
                GUITools.ActionButton("Delete", p_target.RemovePointAt, i, Color.white, Color.black);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUITools.ActionButton("Add Spawn Point", IncrementSpawnPoint, Color.white, Color.black);
            GUITools.ActionButton("Clear all points", ClearPoints, Color.white, Color.black);
            EditorGUILayout.EndHorizontal();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    void ShowWavesSettings()
    {
        p_target.ShowWaves = EditorGUILayout.Foldout(p_target.ShowWaves, "WAVES", true);
        if (p_target.ShowWaves)
        {


            EditorGUILayout.HelpBox("WAVES", MessageType.None);
            for (int i = 0; i < p_target.Waves.Count; i++)
            {
                ShowWave(p_target.Waves[i], i);
            }
            EditorGUILayout.BeginHorizontal();
            GUITools.ActionButton("Add Wave", AddWave, Color.white, Color.black);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
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
            ShowWaveInfo(_wave);
            EditorGUILayout.Space(); 
            GUITools.ActionButton("Remove Wave", RemoveWave, _index, Color.white, Color.black);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_wave"></param>
    void ShowWaveInfo(TDS_Wave _wave)
    {
        
        for (int i = 0; i < _wave.FixElement.Count; i++)
        {
            TDS_WaveElement _info = _wave.FixElement[i];
            _info.IsFoldOut = EditorGUILayout.Foldout(_info.IsFoldOut, "Info", true);
            if(_info.IsFoldOut)
            {
                _info.LinkedPoint = EditorGUILayout.ObjectField("LinkedPoint", _info.LinkedPoint, typeof(TDS_SpawnPoint), true) as TDS_SpawnPoint;
                _info.IsRandom = EditorGUILayout.Toggle("Is random?", _info.IsRandom); 
                if(_info.IsRandom)
                {
                    _info.NumberEnemy = EditorGUILayout.IntSlider("Number of enemies", _info.NumberEnemy, 1, 10); 
                }
                else
                {
                    TDS_Enemy _e = null;
                    _e = EditorGUILayout.ObjectField("Ennemies", _e, typeof(TDS_Enemy), false) as TDS_Enemy;
                    if(_e != null)
                    {
                        _info.SpawningEnemies.Add(_e); 
                    }
                    for (int j = 0; j < _info.SpawningEnemies.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.HelpBox(_info.SpawningEnemies[j].name, MessageType.None);
                        GUITools.ActionButton("Remove Enemy", _info.RemoveEnemyAt, j, Color.white, Color.black);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            GUITools.ActionButton("Remove Info", _wave.RemoveInfoAt, i, Color.white, Color.black);
            EditorGUILayout.Space(); 
        }
        GUITools.ActionButton("Add Info", _wave.AddInfo, Color.white, Color.black);
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
    void SelectSpawnPoint(TDS_SpawnPoint _point)
    {
        Selection.activeObject = _point.gameObject; 
    }
    /// <summary>
    /// Clear all points
    /// </summary>
    void ClearPoints()
    {
        spawnPointID = 0;
        p_target.SpawnPoints.Clear(); 
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
        if (HasToBeUpdated) return; 
        // POINTS RANGE
        for (int i = 0; i < p_target.SpawnPoints.Count; i++)
        {
            Handles.color = p_target.SpawnPoints[i].SpawnPointColor;
            Handles.DrawWireDisc(p_target.SpawnPoints[i].SpawnPosition, Vector3.up, p_target.SpawnPoints[i].SpawningRange);
        }
    }

    #endregion 

}


