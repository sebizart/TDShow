using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] CustomDebug Version 0.0.1
Created by: William Comminges
Date: 03/10/2018
Description: Static Methods to use a Debug log with a user name and a color

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/

public static class TDS_CustomDebug  
{
    #region Fields/Properties
    public static event Action<LogUser, string> OnLogDebugged; 
    #endregion

    #region Methods
    public static void CustomDebugLog(object _log, string _userName)
    {
        LogUser _user = TDS_LoggerManager.Instance.GetUserFromName(_userName); 
        if (!_user.EnableLogs) return;
        Debug.Log($"{_user.UserName} Log: <color={_user.WorkerColor}>{_log}</color>");
        OnLogDebugged?.Invoke(_user, _log.ToString()); 
    }
    public static void CustomDebugLog(object _log)
    {

        if (TDS_LoggerManager.Instance.CurrentUser == null && !TDS_LoggerManager.Instance.CurrentUser.EnableLogs) return;
        Debug.Log($"{TDS_LoggerManager.Instance.CurrentUser.UserName} Log: <color={TDS_LoggerManager.Instance.CurrentUser.WorkerColor}>{_log}</color>");
        OnLogDebugged?.Invoke(TDS_LoggerManager.Instance.CurrentUser, _log.ToString());
    }

    public static void CustomDebugLogError(object _logError, string _userName)
    {
        LogUser _user = TDS_LoggerManager.Instance.GetUserFromName(_userName);
        if (!_user.EnableLogsError) return;
        Debug.LogError($"{_user.UserName} LogError: <color={_user.WorkerColor}>{_logError}</color>");
        OnLogDebugged?.Invoke(_user, _logError.ToString());
    }
    public static void CustomDebugLogError(object _logError)
    {
        if (TDS_LoggerManager.Instance.CurrentUser == null && !TDS_LoggerManager.Instance.CurrentUser.EnableLogsError) return;
        Debug.LogError($"{TDS_LoggerManager.Instance.CurrentUser.UserName} LogError: <color={TDS_LoggerManager.Instance.CurrentUser.WorkerColor}>{_logError}</color>");
        OnLogDebugged?.Invoke(TDS_LoggerManager.Instance.CurrentUser, _logError.ToString());
    }

    public static void CustomDebugLogWarning(object _logWarning, string _userName)
    {
        LogUser _user = TDS_LoggerManager.Instance.GetUserFromName(_userName);
        if (!_user.EnableLogsWarning) return;
        Debug.LogWarning($"{_user.UserName} LogWarning: <color={_user.WorkerColor}>{_logWarning}</color>");
        OnLogDebugged?.Invoke(_user, _logWarning.ToString());
    }
    public static void CustomDebugLogWarning(object _logWarning)
    {
        if (TDS_LoggerManager.Instance.CurrentUser == null && !TDS_LoggerManager.Instance.CurrentUser.EnableLogsWarning) return;
        Debug.LogWarning($"{TDS_LoggerManager.Instance.CurrentUser.UserName} LogWarning: <color={TDS_LoggerManager.Instance.CurrentUser.WorkerColor}>{_logWarning}</color>");
        OnLogDebugged?.Invoke(TDS_LoggerManager.Instance.CurrentUser, _logWarning.ToString());
    }
    #endregion


}
