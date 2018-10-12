using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using System.IO;
using System.Diagnostics;
using System.Net;
using UtilsLibrary.GUILibrary; 
using UnityEngine;
using UnityEditor; 

/*
[Script Header] LogSaverEditor Version 0.0.1
Created by: Thiebaut Alexis
Date: 03/10/2018
Description: Editor of the Log Saver
            Show each method available on click in the Inspector
            -> Upload: Upload one or every local files 
            -> Dowload: Download one or every online files

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/

[CustomEditor(typeof(TDS_LogSaver))]
public class TDS_LogSaverEditor : Editor 
{
    #region Fields/Properties
    private TDS_LogSaver p_target;
    #endregion

    #region Methods
    void OpenLogsWindow(LogType _type)
    {
        if (!p_target) return;
        LogWindow _window = (LogWindow)EditorWindow.GetWindow(typeof(LogWindow));
        switch (_type)
        {
            case LogType.Local:
                p_target.GetAllLocalFilesNames();
                _window.LocalFilesInfos = p_target.AllLocalFilesLog; 
                break;
            case LogType.OnServer:
                p_target.GetAllFTPFilesNames();
                _window.FTPFilesInfos = p_target.AllFTPFilesNames;
                break;
            default:
                break;
        }
        _window.CurrentType = _type;
        _window.Saver = p_target;
        _window.Show();
    }
    #endregion

    #region UnityMethods
    /// <summary>
    /// Set the target when the Object is enable
    /// </summary>
    private void OnEnable()
    {
        p_target = (TDS_LogSaver)target;
    }

    public override void OnInspectorGUI()
    {
        if(p_target.AllLogs.Count > 0)
        {
            GUITools.ActionButton("Save last log entry", p_target.SaveLogs, Color.white, Color.black);
        }

        EditorGUILayout.HelpBox("Log entries ", MessageType.None);
        GUITools.ActionButton("Local log entries", OpenLogsWindow, LogType.Local, Color.white, Color.black);
        GUITools.ActionButton("FTP log entries", OpenLogsWindow, LogType.OnServer, Color.white, Color.black);
    }
    #endregion
}

public class LogWindow : EditorWindow
{
    #region Field and Properties
    public LogType CurrentType;
    public TDS_LogSaver Saver;
    public Vector2 ScrollVector = Vector2.zero;
    public List<LogInfos> LocalFilesInfos = new List<LogInfos>();
    public List<string> FTPFilesInfos = new List<string>();
    #endregion

    #region Unity Methods
    private void OnGUI()
    {
        switch (CurrentType)
        {
            case LogType.Local:
                UploadLogs();
                break;
            case LogType.OnServer:
                DownLoadLogs();
                break;
            default:
                break;
        }
        GUITools.ActionButton("Close", Close, Color.red, Color.black);

    }
    #endregion

    #region Methods  

    /// <summary>
    /// Delete a local log from the local files
    /// </summary>
    /// <param name="_index"></param>
    void DeleteLocalLog(int _index)
    {
        File.Delete(Path.Combine(Saver.SavingPath, LocalFilesInfos[_index].LogName));
        LocalFilesInfos.RemoveAt(_index);
    }

    /// <summary>
    /// FTP Files section
    /// Download a particulary file
    /// Download all files
    /// </summary>
    void DownLoadLogs()
    {
        EditorGUILayout.HelpBox("All FTP Files", MessageType.None);
        GUITools.ActionButton("Download all logs", Saver.DownloadAllFilesFromFTP, Color.white, Color.black);
        EditorGUILayout.Space();
        ScrollVector = EditorGUILayout.BeginScrollView(ScrollVector);
        for (int i = 0; i < FTPFilesInfos.Count; i++)
        {
            GUITools.ActionButton(FTPFilesInfos[i], Saver.DownloadFileFromFTP, FTPFilesInfos[i], Color.cyan, Color.black);
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
        GUITools.ActionButton("Update logs on FTP", Saver.GetAllFTPFilesNames, Color.white, Color.black);
    }

    /// <summary>
    /// Open the selected file with Notepads
    /// </summary>
    /// <param name="_fileName"></param>
    void OpenFile(string _fileName) => Process.Start("notepad.exe", Path.Combine(Saver.SavingPath, _fileName));

    /// <summary>
    /// Open the local saving folder
    /// If it does not exists, create the saving folder
    /// </summary>
    void OpenLocalFolder()
    {
        if (!Directory.Exists(Saver.SavingPath))
            Directory.CreateDirectory(Saver.SavingPath); 
        Process.Start(Saver.SavingPath);
    }

    /// <summary>
    /// Local files section
    /// Upload or delete a particulary local file
    /// Upload all local files
    /// </summary>
    void UploadLogs()
    {
        EditorGUILayout.HelpBox("All Local Files", MessageType.None);
        GUITools.ActionButton("Send all logs to FTP", Saver.UploadAllFilesToFTP, Color.white, Color.black);
        EditorGUILayout.Space();
        ScrollVector = EditorGUILayout.BeginScrollView(ScrollVector);
        for (int i = 0; i < LocalFilesInfos.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox(LocalFilesInfos[i].LogName, MessageType.None);
            GUITools.ActionButton("Open", OpenFile, LocalFilesInfos[i].LogName, Color.green, Color.black);
            if (!LocalFilesInfos[i].IsLoading)
            {
                GUITools.ActionButton("Upload", Saver.UploadFileToFTP, LocalFilesInfos[i], Color.cyan, Color.black);
            }
            else
            {
                EditorGUILayout.HelpBox("Uploading", MessageType.None);
            }
            GUITools.ActionButton("X", DeleteLocalLog, i, Color.red, Color.black);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
        GUITools.ActionButton("Open Folder", OpenLocalFolder, Color.white, Color.black);
    }

    #endregion

}

public enum LogType
{
    Local,
    OnServer
}