using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using UnityEngine;
using System.Net;

/*
[Script Header] LogSaver Version 1.0.0
Created by: Thiebaut Alexis
Date: 02/10/2018
Description: Upload/Download LogFiles from/to Localfolder/FTPFolder

///
[UPDATES]
Update n°: 0
Updated by: Thiebaut Alexis
Date: 02/10/2018
Description: Methods to download and upload files using WebClient
//
Update n°: 1
Updated by: Thiebaut Alexis
Date: 03/10/2018
Description: Methods to save local files from unity debugs
//
Update n°: 
Updated by: Thiebaut Alexis
Date: 04/10/2018
Description: Modifications of the structure of the uploaded log files
             Each LogInfo contains a filename and a bool
             Using this bool in the Editor to show the button "Upload" of the linked file
*/

public class TDS_LogSaver : MonoBehaviour
{

    #region Fields/Properties
    public string SavingPath { get { return Path.Combine(Application.persistentDataPath, "Logs"); } }

    public List<string> AllFTPFilesNames = new List<string>();
    public List<LogInfos> AllLocalFilesLog = new List<LogInfos>(); 

    public List<string> AllLogs = new List<string>();
    #endregion

    #region Methods

    #region Saving Methods
    /// <summary>
    /// Add a Debug Line to the soon-to-be saved Log
    /// FORMAT: [DATETIME] User: Content
    /// </summary>
    /// <param User="_user"></param>
    /// <param Content="_logContent"></param>
    public void SaveDebug(LogUser _user, string _logContent)
    {
        string _log = $"[{DateTime.Now}] {_user.UserName}: {_logContent}";
        AllLogs.Add(_log);
    }

    /// <summary>
    /// Save a new log entry with all debug lines in the AllLogs list
    /// Clean the allLogs list when the file is created
    /// </summary>
    public void SaveLogs()
    {
        if (!Directory.Exists(SavingPath)) Directory.CreateDirectory(SavingPath);
        string _date = DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
        string _fileDirectory = Path.Combine(SavingPath, $"LOG_{_date}.cata");
        File.AppendAllLines(_fileDirectory, AllLogs);
        AllLogs.Clear();
    }
    #endregion 

    #region Upload Methods
    /// <summary>
    /// Send the selected file in the FTP
    /// </summary>
    /// <param name="_fileInfo"></param>
    public void UploadFileToFTP(LogInfos _fileInfo)
    {
        _fileInfo.IsLoading = true;
        WebClient _sendToFTP = new WebClient();
        _sendToFTP.UploadFileCompleted += ((object sender, UploadFileCompletedEventArgs e) => _fileInfo.IsLoading = false);
        _sendToFTP.Credentials = new NetworkCredential("u89869971-jv2", "&piiC2017");
        _sendToFTP.UploadFileAsync(new Uri("ftp://home689765717.1and1-data.host/cata/" +_fileInfo.LogName), WebRequestMethods.Ftp.UploadFile, Path.Combine(SavingPath, _fileInfo.LogName));
    }
    /// <summary>
    /// Send mutiple local files in the FTP
    /// </summary>
    /// <param name="_filesInfo"></param>
    public void UploadFilesToFTP(LogInfos[] _filesInfo)
    {

        for (int i = 0; i < _filesInfo.Length; i++)
        {
            WebClient _sendToFTP = new WebClient();
            _sendToFTP.Credentials = new NetworkCredential("u89869971-jv2", "&piiC2017");
            _sendToFTP.UploadFileAsync(new Uri("ftp://home689765717.1and1-data.host/cata/" + _filesInfo[i].LogName), WebRequestMethods.Ftp.UploadFile, Path.Combine(SavingPath ,_filesInfo[i].LogName));
        }

    }
    /// <summary>
    /// Send all local files in the FTP
    /// </summary>
    public void UploadAllFilesToFTP()
    {
       GetAllLocalFilesNames();
       UploadFilesToFTP(AllLocalFilesLog.ToArray());
    }
    #endregion

    #region DownloadMethods
    /// <summary>
    /// Download the file from the FTP in a directory
    /// </summary>
    /// <param name="_fileName"></param>
    public void DownloadFileFromFTP(string _fileName)
    {
        WebClient _sendToFTP = new WebClient();
        _sendToFTP.Credentials = new NetworkCredential("u89869971-jv2", "&piiC2017");
        _sendToFTP.DownloadFileAsync(new Uri("ftp://home689765717.1and1-data.host/cata/" + _fileName), Path.Combine(SavingPath, _fileName));
    }
    /// <summary>
    /// Download all the files from the FTP
    /// </summary>
    public void DownloadAllFilesFromFTP()
    {

        for (int i = 0; i < AllFTPFilesNames.Count; i++)
        {
            WebClient _sendToFTP = new WebClient();
            _sendToFTP.Credentials = new NetworkCredential("u89869971-jv2", "&piiC2017");
            _sendToFTP.DownloadFileAsync(new Uri("ftp://home689765717.1and1-data.host/cata/" + AllFTPFilesNames[i]), Path.Combine(SavingPath, AllFTPFilesNames[i]));
        }
    }
    #endregion


    #region Getting Files
    /// <summary>
    /// Return all file names
    /// </summary>
    /// <returns></returns>
    public void GetAllLocalFilesNames()
    {
        if (!Directory.Exists(SavingPath))
            Directory.CreateDirectory(SavingPath);
        AllLocalFilesLog.Clear(); 
        string[] _directories = Directory.GetFiles(SavingPath, "*.cata");
        for (int i = 0; i < _directories.Length; i++)
        {
            LogInfos _infos = new LogInfos(Path.GetFileName(_directories[i]));
            AllLocalFilesLog.Add(_infos); 
        }

    }
    /// <summary>
    /// Get the name of every file in the FTP
    /// </summary>
    public void GetAllFTPFilesNames()
    {
        AllFTPFilesNames.Clear();
        FtpWebRequest _request = (FtpWebRequest)HttpWebRequest.Create("ftp://home689765717.1and1-data.host/cata/");
        _request.Credentials = new NetworkCredential("u89869971-jv2", "&piiC2017");
        _request.Method = WebRequestMethods.Ftp.ListDirectory;
        FtpWebResponse _response = (FtpWebResponse)_request.GetResponse();
        StreamReader _responseStream = new StreamReader(_response.GetResponseStream());
        string _lineRequest = _responseStream.ReadLine();
        while (!string.IsNullOrEmpty(_lineRequest))
        {
            if (_lineRequest.Contains(".cata"))
            {
                AllFTPFilesNames.Add(_lineRequest);
            }
            _lineRequest = _responseStream.ReadLine();
        }
        _responseStream.Close();
    }
    #endregion
    #endregion

    #region UnityMethods
    private void Awake()
    {
        TDS_CustomDebug.OnLogDebugged += SaveDebug;
    }
    private void OnDisable()
    {
        SaveLogs(); 
    }
    #endregion 

}

public class LogInfos
{
    public string LogName;
    public bool IsLoading = false;
    
    public LogInfos(string _directory)
    {
        LogName = _directory; 
    }
}
