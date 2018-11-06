using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary;

/*
[Script Header] SpritesToolEditor Version 0.0.1
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

[CustomEditor(typeof(SpritesManager))]
public class SpritesToolEditor : Editor
{
    #region Fields/Properties
    SpritesManager p_target;
    #endregion

    #region Methods
    void DropAeraGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        Event _e = Event.current;
        Rect _dropZone = GUILayoutUtility.GetRect(0, 25, GUILayout.ExpandWidth(true));
        GUI.Box(_dropZone, "Drop sprites here");
        switch (_e.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!_dropZone.Contains(_e.mousePosition))
                    return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (_e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    if (DragAndDrop.objectReferences == null) return;
                    foreach (UnityEngine.Object o in DragAndDrop.objectReferences)
                    {
                        GameObject _go = (GameObject)o;
                        if (_go)
                            p_target.AddObject(_go.transform);

                    }
                }
                break;
            default:
                break;
        }
    }

    public void MakeSpriteRotate()
    {
        for (int i = 0; i < p_target.AllSpritesToMove.Count; i++)
        {
            p_target.AllSpritesToMove[i].transform.forward = Camera.main.transform.forward;
        }
    }

    void ManageObjects()
    {
        for (int t = 0; t < p_target.AllSpritesToMove.Count; t++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(p_target.AllSpritesToMove[t], typeof(Sprite), true);
            GUITools.ActionButton("X", p_target.RemoveSpriteAt,t, Color.red, Color.black);
            EditorGUILayout.EndHorizontal();

        }        
    }
    #endregion

    #region UnityMethods

    public void OnEnable()
    {
        p_target = (SpritesManager)target;
    }

    public override void OnInspectorGUI()
    {
        DropAeraGUI();
        ManageObjects();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUITools.ActionButton("Rotate sprites", MakeSpriteRotate, Color.yellow, Color.black);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUITools.ActionButton("Clear", p_target.RemoveAllSprites, Color.red, Color.black);
    }
    #endregion
}