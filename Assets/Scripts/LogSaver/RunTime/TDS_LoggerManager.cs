using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] Loggermanager Version 1.0.0
Created by: Comminges William
Date: 03/10/2018
Description: Contains every stocked users

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/

public class TDS_LoggerManager : MonoBehaviour 
{
    
    #region Fields/Properties
    public static TDS_LoggerManager Instance;

    public List<LogUser> AllUsers = new List<LogUser>();
    public LogUser CurrentUser; 
    public bool EnableAllLogs = true;
    #endregion

    #region Methods
    public void AddNewUser(string _name) => AllUsers.Add(new LogUser(_name));

    public LogUser GetUserFromName(string _name)
    {
        for (int i = 0; i < AllUsers.Count; i++)
        {
            if (string.Equals(AllUsers[i].UserName.ToLower(), _name.ToLower()))
                return AllUsers[i];
        }
        if (CurrentUser != null) return CurrentUser;
        return new LogUser("Default User");
    }

    public void RemoveUser(int _index) => AllUsers.RemoveAt(_index); 

    #endregion

    #region UnityMethods
    private void Awake()
    {
        if (Instance != null) return;
        Instance = this; 
    }
    #endregion
}

[Serializable]
public class LogUser
{

    #region Fields/Properties    
    public bool isCurrentUser = false; 
    public bool EnableLogs = true;
    public bool EnableLogsError = true;
    public bool EnableLogsWarning = true;


    public RichTextColor WorkerColor = RichTextColor.black;

    public string UserName = "Default user";

    #endregion

    #region Constructor
    public LogUser(string _userName)
    {
        UserName = _userName;
    }
    #endregion
}

public enum RichTextColor
{
    black,
    blue,
    brown,
    cyan,
    darkblue,
    green,
    grey,
    lightblue,
    lime,
    maroon,
    navy,
    olive,
    orange,
    purple,
    red,
    silver,
    teal,
    yellow
}
