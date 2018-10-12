using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

/*
[Script Header] CATA_CameraEditor Version 0.0.1
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

[CustomEditor(typeof(TDS_Camera))]
public class TDS_CameraEditor : Editor 
{
    #region Fields/Properties
    SerializedProperty zForwardBoundProp;
    SerializedProperty zBackwardBoundProp;
    SerializedProperty cameraOrientationProp;
    SerializedProperty xOffsetProp;
    SerializedProperty yOffsetProp;
    SerializedProperty zOffsetProp;
    SerializedProperty maxSpeedProp;
    SerializedProperty doMoveOnZProp; 

    TDS_Camera p_target; 
    #endregion

    #region Methods

    #endregion

    #region UnityMethods
    public void OnEnable()
    {
        p_target = (TDS_Camera)target; 
        zForwardBoundProp = serializedObject.FindProperty("zForwardBound");
        zBackwardBoundProp = serializedObject.FindProperty("zBackwardBound");
        cameraOrientationProp = serializedObject.FindProperty("cameraOrientation");
        xOffsetProp = serializedObject.FindProperty("xOffset");
        yOffsetProp = serializedObject.FindProperty("yOffset");
        zOffsetProp = serializedObject.FindProperty("zOffset");
        maxSpeedProp = serializedObject.FindProperty("maxSpeed");
        doMoveOnZProp = serializedObject.FindProperty("doMoveOnZ");

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        doMoveOnZProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Do move on Z"), doMoveOnZProp.boolValue); 
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.Slider(xOffsetProp, -50, 50, new GUIContent("X Offset"));
        EditorGUILayout.Slider(yOffsetProp, -50, 50, new GUIContent("Y Offset"));
        EditorGUILayout.Slider(zOffsetProp, -50, 50, new GUIContent("Z Offset"));
        EditorGUILayout.Slider(zForwardBoundProp, -50, 50, new GUIContent("Forward bound"));
        EditorGUILayout.Slider(zBackwardBoundProp, -50, 50, new GUIContent("Backward bound"));
        EditorGUILayout.Slider(cameraOrientationProp, 1, 360, new GUIContent("Camera Orientation"));
        EditorGUILayout.Slider(maxSpeedProp, 0, 50, new GUIContent("Speed"));
        if (EditorGUI.EndChangeCheck())
        {
            p_target.UpdateProperties(); 
        }
        serializedObject.ApplyModifiedProperties(); 

    }


    #endregion
}
