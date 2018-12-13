using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UtilsLibrary.GUILibrary;

/*
[Script Header] TDS_CustomNavMeshEditor Version 0.0.1
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
[CustomEditor(typeof(TDS_NavMeshBuilder))]
public class TDS_CustomNavMeshBuilderEditor : Editor 
{

    #region Fields/Properties
    TDS_NavMeshBuilder p_target;

    #endregion

    #region Methods
    void DrawNavMeshBuilder()
    {
        EditorGUILayout.HelpBox("Custom Nav Mesh Baking\nSet NavSurfaces on the scene to bake the custom NavMesh", MessageType.None); 
        GUITools.ActionButton("Bake NavPoints from MeshSurfaces", p_target.GetNavPointsFromNavSurfaces, Color.white, Color.black);
        GUITools.ActionButton("Clear NavMesh", p_target.ClearNavDatas, Color.white, Color.black);
        GUITools.ActionButton("Save Datas", p_target.SaveDatas, Color.white, Color.black);
        GUITools.ActionButton("Open Saving Folder", p_target.OpenDirectory, Color.white, Color.black); 

        EditorGUILayout.Space();
        p_target.ShowVertices = EditorGUILayout.Toggle("Show Vertices", p_target.ShowVertices);
        p_target.ShowSegments = EditorGUILayout.Toggle("Show Segment", p_target.ShowSegments);
        p_target.ShowMiddle = EditorGUILayout.Toggle("Show Middle", p_target.ShowMiddle);
        p_target.ShowLinks = EditorGUILayout.Toggle("Show Links", p_target.ShowLinks); 
    }
    #endregion

    #region UnityMethods
    private void OnEnable()
    {
        p_target = (TDS_NavMeshBuilder)target; 
    }
    public override void OnInspectorGUI()
    {
        DrawNavMeshBuilder(); 
    }
    #endregion
}
