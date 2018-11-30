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
    /// Open the Editor Window for the selected point
    /// </summary>
    /// <param name="_point">selected point</param>
    void OpenPointWindow(TDS_WavePoint _point)
    {
        TDS_PointEditorWindow _window = (TDS_PointEditorWindow)EditorWindow.GetWindow(typeof(TDS_PointEditorWindow));
        _window.WavePoint = _point;
        _window.Show();
    }

    /// <summary>
    /// Show a display dialog before deleting a wave point
    /// </summary>
    /// <param name="_index">wave point to delete</param>
    void RemoveWavePoint(int _index)
    {
        if(EditorUtility.DisplayDialog("Remove Point", $"Remove {p_target.PointsManager.AllWavePoints[_index].Name}", "Remove", "Cancel"))
        {
            p_target.PointsManager.RemoveWavePoint(_index); 
        }
    }

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
        p_target.DetectionArea.CenterPosition = EditorGUILayout.Vector3Field("Center Position", p_target.DetectionArea.CenterPosition); 
        p_target.DetectionArea.DebugColor = EditorGUILayout.ColorField("Debug color", p_target.DetectionArea.DebugColor);
        //IS LOOPING
        p_target.IsLooping = EditorGUILayout.Toggle("Is Looping", p_target.IsLooping); 
    }
    
    /// <summary>
    /// Show the point manager settings
    /// Edit its position
    /// Edit all waves in the manager
    /// </summary>
    void ShowPointManagerSettings()
    {
        GUI.color = Color.red; 
        EditorGUILayout.HelpBox("Point Manager Settings", MessageType.None);
        GUI.color = Color.white;
        p_target.PointsManager.Position = EditorGUILayout.Vector3Field("Manager Position", p_target.PointsManager.Position);
        GUI.color = Color.cyan;
        EditorGUILayout.HelpBox("WAVES", MessageType.None);
        for (int i = 0; i < p_target.PointsManager.AllWavePoints.Count; i++)
        {
            ShowWaveSettings(p_target.PointsManager.AllWavePoints[i]);
            GUITools.ActionButton("Remove Wave point", RemoveWavePoint, i, Color.white, Color.black);
            EditorGUILayout.Space(); 
        }
        EditorGUILayout.Space(); 
        GUITools.ActionButton("Add Wave point", p_target.PointsManager.AddWavePoint, Color.white, Color.black);
    }

    /// <summary>
    /// Button to show the point editor window 
    /// </summary>
    /// <param name="_point">point to edit</param>
    void ShowWaveSettings(TDS_WavePoint _point)
    {
        GUITools.ActionButton($"Edit {_point.Name}", OpenPointWindow, _point, Color.white, Color.black); 
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

        // POINTS
        for (int i = 0; i < p_target.PointsManager.AllWavePoints.Count; i++)
        {
            p_target.PointsManager.AllWavePoints[i].Position = Handles.PositionHandle(p_target.PointsManager.AllWavePoints[i].Position, Quaternion.identity);
            Handles.DrawWireDisc(p_target.PointsManager.AllWavePoints[i].Position, Vector3.up, p_target.PointsManager.AllWavePoints[i].Range);
            Handles.Label(p_target.PointsManager.AllWavePoints[i].Position + Vector3.up, p_target.PointsManager.AllWavePoints[i].Name); 
        }

    }

    #endregion 

}

public class TDS_PointEditorWindow : EditorWindow
{
    public TDS_WavePoint WavePoint; 

    private void OnGUI()
    {
        WavePoint.Position = EditorGUILayout.Vector3Field("Point Position", WavePoint.Position);

        WavePoint.Range = EditorGUILayout.Slider("Spawn Range", WavePoint.Range, 1, 10);

        WavePoint.WaveNumber = EditorGUILayout.IntField("Wave Number", WavePoint.WaveNumber);

        // STATIC WAVE ELEMENT
        ShowWaveElementSettings(WavePoint.StaticWaveElement);
        EditorGUILayout.Space();

        // RANDOM WAVE ELEMENT
        ShowWaveElementSettings(WavePoint.RandomWaveElement);
        EditorGUILayout.Space();

        GUITools.ActionButton("Close", Close, Color.red, Color.black); 

    }

    /// <summary>
    /// Edit wave element 
    /// Edit type and number of enemies
    /// </summary>
    /// <param name="_element">Element to Edit</param>
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
    /// Edit wave element 
    /// Edit type and number of enemies
    /// Edit also the percantage of spawning chance
    /// </summary>
    /// <param name="_element">Element to Edit</param>
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

}

