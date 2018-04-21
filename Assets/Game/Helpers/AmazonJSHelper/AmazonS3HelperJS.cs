using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using System.Runtime.InteropServices;

public class AmazonS3HelperJS : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void listFilesJS();

    [DllImport("__Internal")]
    private static extern void getObjectJS(string str);

    [DllImport("__Internal")]
    private static extern void deleteObjectJS(string str);

    [DllImport("__Internal")]
    private static extern void postObjectJS(string path, string data);

    [DllImport("__Internal")]
    private static extern void setAlert(string str);

    public LevelSelector levelSelector;

    string lastGetFileName = "";

    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            //Application.ExternalCall();
        }
    }

    public void ListFiles(string prefix)
    {
        listFilesJS();
    }

    public void ListFileCallback(string data)
    {
        var fileArray = JsonUtility.FromJson<AmazonS3ObjectList>(data);
        List<AmazonS3Object> fileList = fileArray.Contents.ToList();

        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        foreach (var file in fileList)
        {
            DateTime date = start.AddMilliseconds(file.LastModifiedTime);
            file.LastModified = date;
        }

        //setAlert(fileList.FirstOrDefault().LastModified.ToString());

        //levelSelector.LoadLevelNamesWeb(fileList);
    }

    public void GetFile(string filePath, string name)
    {
        lastGetFileName = name;
        getObjectJS(filePath);
    }

    public void GetFileCallback(string data)
    {
        if (lastGetFileName == DataPath.fileListName)
        {
            //levelSelector.LoadLevelListWeb(lastGetFileName, data);
        }
        else
        {
            var level = JsonUtility.FromJson<Level>(data);

            levelSelector.LoadLevelTextWeb(level.levelName, data);
        }
    }

    public void PostObject(string fileName, string s)
    {
        postObjectJS(fileName, s);
    }

    public void DeleteObject(string name, string filePath)
    {
        deleteObjectJS(filePath);
    }
}

