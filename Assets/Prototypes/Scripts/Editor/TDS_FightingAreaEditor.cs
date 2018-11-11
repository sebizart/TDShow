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
        //DETECTION AREA
        p_target.DetectionArea.DebugColor = EditorGUILayout.ColorField("Debug color", p_target.DetectionArea.DebugColor);
    }
    
    /// <summary>
    /// 
    /// </summary>
    void ShowPointManagerSettings()
    {
        GUI.color = Color.red; 
        EditorGUILayout.HelpBox("Point Manager Settings", MessageType.None);
        GUI.color = Color.white;
        p_target.PointsManager.Position = EditorGUILayout.Vector3Field("Manager Position", p_target.PointsManager.Position);
        EditorGUILayout.HelpBox("WAVES", MessageType.None);
        for (int i = 0; i < p_target.PointsManager.AllWavePoints.Count; i++)
        {
            ShowWaveSettings(p_target.PointsManager.AllWavePoints[i]);
            GUITools.ActionButton("Remove Wave point", p_target.PointsManager.RemoveWavePoint, i, Color.white, Color.black); 
        }
        EditorGUILayout.Space(); 
        GUITools.ActionButton("Add Wave point", p_target.PointsManager.AddWavePoint, Color.white, Color.black);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_point"></param>
    void ShowWaveSettings(TDS_WavePoint _point)
    {
        _point.IsFoldOut = EditorGUILayout.Foldout(_point.IsFoldOut, _point.Name, true);
        if (_point.IsFoldOut)
        {
            _point.Position = EditorGUILayout.Vector3Field("Point Position", _point.Position);

            _point.Range = EditorGUILayout.Slider("Spawn Range" ,_point.Range, 1, 10);

            _point.WaveNumber = EditorGUILayout.IntField("Wave Number", _point.WaveNumber); 

            // STATIC WAVE ELEMENT
            ShowWaveElementSettings(_point.StaticWaveElement);
            EditorGUILayout.Space();

            // RANDOM WAVE ELEMENT
            ShowWaveElementSettings(_point.RandomWaveElement);
            EditorGUILayout.Space();

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_element"></param>
    void ShowWaveElementSettings(TDS_WaveElement _element)
    {
        // COLOR
        GUI.color = Color.blue;
        EditorGUILayout.HelpBox("Wave Element", MessageType.None);
        GUI.color = Color.white;
        // ENEMIES AND COUNT
        TDS_Enemy _enemy = null;
        _enemy = EditorGUILayout.ObjectField(_enemy, typeof(TDS_Enemy), false) as TDS_Enemy;
        if (_enemy != null && !_element.EnemiesSpawn.Any(e => e.SpawningEnemy == _enemy))
        {
            _element.AddElement(_enemy);
        }
        for (int i = 0; i < _element.EnemiesSpawn.Count; i++)
        {
            EditorGUILayout.LabelField(_element.EnemiesSpawn[i].SpawningEnemy.PrefabName.ToString());
            _element.EnemiesSpawn[i].NumberOfEnemies = EditorGUILayout.IntSlider("Enemy count", _element.EnemiesSpawn[i].NumberOfEnemies, 0, 10);
            GUITools.ActionButton("Remove Element", _element.RemoveElement, i, Color.white, Color.black);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_element"></param>
    void ShowWaveElementSettings(TDS_RandomWaveElement _element)
    {
        // COLOR
        GUI.color = Color.green;
        EditorGUILayout.HelpBox("Random Wave Element", MessageType.None);
        GUI.color = Color.white;

        // ENEMIES AND COUNT
        TDS_Enemy _enemy = null;
        _enemy = EditorGUILayout.ObjectField(_enemy, typeof(TDS_Enemy), false) as TDS_Enemy;
        if (_enemy != null && !_element.EnemiesSpawn.Any(e => e.SpawningEnemy == _enemy))
        {
            _element.AddElement(_enemy);
        }
        for (int i = 0; i < _element.EnemiesSpawn.Count; i++)
        {
            EditorGUILayout.LabelField(_element.EnemiesSpawn[i].SpawningEnemy.PrefabName.ToString());
            _element.EnemiesSpawn[i].NumberOfEnemies = EditorGUILayout.IntSlider("Enemy count", _element.EnemiesSpawn[i].NumberOfEnemies, 0, 10);
            _element.RandomValues[i] = EditorGUILayout.IntSlider("Spawning Chance", _element.RandomValues[i], 0, 100);
            GUITools.ActionButton("Remove Element", _element.RemoveElement, i, Color.white, Color.black);
        }
    }

    #endregion

    #region UnityMethods
    private void OnEnable()
    {
        p_target = (TDS_FightingArea)target; 
    }

    public override void OnInspectorGUI()
    {
        ShowFigthingAreaSettings();
        ShowPointManagerSettings();
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

        // MANAGER
        p_target.PointsManager.Position = Handles.PositionHandle(p_target.PointsManager.Position, Quaternion.identity);

        // POINTS
        for (int i = 0; i < p_target.PointsManager.AllWavePoints.Count; i++)
        {
            p_target.PointsManager.AllWavePoints[i].Position = Handles.PositionHandle(p_target.PointsManager.AllWavePoints[i].Position, Quaternion.identity);
            Handles.DrawWireDisc(p_target.PointsManager.AllWavePoints[i].Position, Vector3.up, p_target.PointsManager.AllWavePoints[i].Range); 
        }

    }

    #endregion 

}


