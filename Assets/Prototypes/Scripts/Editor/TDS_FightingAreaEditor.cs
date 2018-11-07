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
    private int spawnPointIndex = 0; 
    #endregion

    #region Serialized Properties
    SerializedProperty detectionAreaProperty;     
    #endregion

    #region Methods
    void ShowSpawnPointsSettings()
    {
        GUITools.ActionButton("Add Spawn Point", IncrementSpawnPoint, Color.cyan, Color.black);
        for (int i = 0; i < p_target.SpawnPoints.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(p_target.SpawnPoints[i].Name);
            GUITools.ActionButton("Select", SelectSpawnPoint, p_target.SpawnPoints[i], Color.cyan, Color.black);
            GUITools.ActionButton("Delete", p_target.RemovePointAt, i, Color.red, Color.black);
            EditorGUILayout.EndHorizontal(); 
        }
        GUITools.ActionButton("Clear all points", ClearPoints, Color.red, Color.black);
    }

    #region Points
    void IncrementSpawnPoint()
    {
        spawnPointIndex++;
        p_target.AddSpawnPoint(spawnPointIndex); 
    }
    void SelectSpawnPoint(TDS_SpawnPoint _point) => selectedPoint = _point; 
    void ClearPoints()
    {
        spawnPointIndex = 0;
        p_target.SpawnPoints.Clear(); 
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
        base.OnInspectorGUI();
        ShowSpawnPointsSettings(); 
        
    }

    private void OnSceneGUI()
    {
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

//HEADER
// Property Drawer for the Detection Area
[CustomPropertyDrawer(typeof(TDS_DetectionArea))]
public class TDS_DetectionAreaDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //Draw Label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        //Calculate rects
        Rect ColorRect = new Rect(position.x-10, position.y + 2, position.width, position.height);

        //Draw Fields      
        GUIContent _content = new GUIContent("Debug Color");
        EditorGUI.PropertyField(ColorRect, property.FindPropertyRelative("debugColor"), _content);


        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();

    }
}


