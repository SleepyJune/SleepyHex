using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AmazonS3Helper : MonoBehaviour
{
    public delegate void ListFilesCallback(List<AmazonS3Object> files);
    public delegate void GetFileCallback(string name, string data);

    public AmazonS3HelperUnity unityHelper;
    public AmazonS3HelperJS jsHelper;

    void Start()
    {

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
    
    public void PostObject(string fileName, string s)
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            unityHelper.PostObject(fileName, s);
        }
        else
        {
            jsHelper.PostObject(fileName, s);
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
}