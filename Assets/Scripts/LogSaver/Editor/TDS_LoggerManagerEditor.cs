using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary;

/*
[Script Header] LoggerManagerEditor Version 0.0.1
Created by: William Comminges
Date: 03/10/2018
Description:

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/


[CustomEditor(typeof(TDS_LoggerManager))]
public class TDS_LoggerManagerEditor : Editor 
{
    #region Fields and Properties
    TDS_LoggerManager p_target;
    private string tmpName = string.Empty;
    #endregion

    #region UnityMethods
    public void OnEnable()
    {
        p_target = (TDS_LoggerManager)target;
    }

    /// <summary>
    /// Enable Logs 
    /// Show every user and its settings
    /// </summary>
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("Enable ALL logs", MessageType.None);
        p_target.EnableAllLogs = EditorGUILayout.Toggle(p_target.EnableAllLogs);
        EditorGUILayout.EndHorizontal();
        //
        EditorGUILayout.Space();
        //
        if (p_target.AllUsers.Count > 0)
        {
            for (int i = 0; i < p_target.AllUsers.Count; i++)
            {
                DrawUserSettings(p_target.AllUsers[i]);
                GUITools.ActionButton("X Delete user", p_target.RemoveUser, i, Color.red, Color.black);
                EditorGUILayout.Space();
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        tmpName = EditorGUILayout.TextField(tmpName);
        GUITools.ActionButton("+ Add User", AddUser, Color.green, Color.black);
        EditorGUILayout.Space();
    }
    #endregion

    #region Methods    

    /// <summary>
    /// Add a new user to the Users List
    /// If There is no name in the field, it can't create a new user 
    /// </summary>
    void AddUser()
    {
        if (tmpName != string.Empty)
        {
            p_target.AddNewUser(tmpName);
            tmpName = string.Empty;
        }
    }

    /// <summary>
    /// Draw user settings in the inspector
    /// Show User name
    /// Show and select user color
    /// Toggle all kinds of logs
    /// </summary>
    /// <param name="_user"></param>
    void DrawUserSettings(LogUser _user)
    {
        EditorGUILayout.BeginHorizontal();
        _user.UserName = EditorGUILayout.TextField(_user.UserName);
        _user.WorkerColor = (RichTextColor)EditorGUILayout.EnumPopup(_user.WorkerColor);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.HelpBox("Is current User", MessageType.None);
        _user.isCurrentUser = EditorGUILayout.Toggle(_user.isCurrentUser);
        if(EditorGUI.EndChangeCheck())
        {
            p_target.CurrentUser.isCurrentUser = false; 
            p_target.CurrentUser = _user; 
        }
        EditorGUILayout.EndHorizontal();

        if (!p_target.EnableAllLogs) return;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("Enable user log", MessageType.None);
        _user.EnableLogs = EditorGUILayout.Toggle(_user.EnableLogs);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("Enable user log error", MessageType.None);
        _user.EnableLogsError = EditorGUILayout.Toggle(_user.EnableLogsError);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("Enable user warning", MessageType.None);
        _user.EnableLogsWarning = EditorGUILayout.Toggle(_user.EnableLogsWarning);
        EditorGUILayout.EndHorizontal();
    }
    #endregion
}
