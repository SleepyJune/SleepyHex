using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AmazonS3Helper : MonoBehaviour
{
    public static AmazonS3Helper instance = null;

    public delegate void ListFilesCallback(List<AmazonS3Object> files);
    public delegate void ListVersionCallback(List<LevelVersion> versions);
    public delegate void GetFileCallback(string name, string data);

    public AmazonS3HelperUnity unityHelper;
    public AmazonS3HelperJS jsHelper;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void GetFile(string filePath, string name, GetFileCallback callback)
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            unityHelper.GetFile(filePath, name, callback);
        }
        else
        {
            jsHelper.GetFile(filePath, name);
        }
    }

    public void PostObject(string fileName, string data, IDictionary<string, string> metadata = null)
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            unityHelper.PostObject(fileName, data, metadata);
        }
        else
        {
            jsHelper.PostObject(fileName, data);
        }
    }

    public void ListFiles(string prefix, ListFilesCallback callback)
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            unityHelper.ListFiles(prefix, callback);
        }
        else
        {
            jsHelper.ListFiles(prefix);
        }
    }

    public void DeleteObject(string name, string filePath)
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            unityHelper.DeleteObject(name, filePath);
        }
        else
        {
            jsHelper.DeleteObject(name, filePath);
        }
    }

    public void UploadLevelVersion(LevelVersion version)
    {
        unityHelper.UploadLevelVersion(version);
    }

    public void ListLevelVersions(ListVersionCallback callback)
    {
        unityHelper.ListLevelVersions(callback);
    }

    public void DeleteLevelVersion(string levelName)
    {
        unityHelper.DeleteLevelVersion(levelName);
    }
}