﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.CognitoIdentity;
using Amazon.Runtime;

public class AmazonS3HelperUnity : MonoBehaviour
{
    string IdentityPoolId = "us-east-1:22e866d2-8c4f-447d-b6f0-7f3227dd1807";
    string CognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;
    private RegionEndpoint _CognitoIdentityRegion
    {
        get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
    }
    string S3Region = RegionEndpoint.USWest2.SystemName;
    private RegionEndpoint _S3Region
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }
    string S3BucketName = "unitytestprojects";

    void Awake()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            UnityInitializer.AttachToGameObject(this.gameObject);

            /*GetBucketListButton.onClick.AddListener(() => { GetBucketList(); });
            PostBucketButton.onClick.AddListener(() => { PostObject(); });
            GetObjectsListButton.onClick.AddListener(() => { GetObjects(); });
            DeleteObjectButton.onClick.AddListener(() => { DeleteObject(); });
            GetObjectButton.onClick.AddListener(() => { GetObject(); });*/

            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        }
    }

    public void GetFile(string filePath, string name, AmazonS3Helper.GetFileCallback callback)
    {
        Client.GetObjectAsync(S3BucketName, filePath, (responseObj) =>
        {
            string data = null;
            var response = responseObj.Response;
            if (response.ResponseStream != null)
            {
                using (StreamReader reader = new StreamReader(response.ResponseStream))
                {
                    data = reader.ReadToEnd();
                }
                                
                Debug.Log("Downloaded: " + name);

                callback(name, data);


                //ResultText.text += "\n";
                //ResultText.text += data;
            }
        });
    }

    public void PostObject(string fileName, string s)
    {
        //string fileName = GetFileHelper();

        var stream = GenerateStreamFromString(s);

        var request = new PostObjectRequest()
        {
            Bucket = S3BucketName,
            Key = fileName,
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = RegionEndpoint.USWest2,
        };

        Client.PostObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                Debug.Log("Uploaded: " + fileName);
            }
            else
            {
                Debug.Log(responseObj.Response.HttpStatusCode.ToString());
            }
        });
    }

    public void ListFiles(string prefix, AmazonS3Helper.ListFilesCallback callback)
    {
        var request = new ListObjectsRequest()
        {
            BucketName = S3BucketName,
            Prefix = prefix,
        };

        Client.ListObjectsAsync(request, (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                Debug.Log("Downloaded file list");

                List<AmazonS3Object> fileList = new List<AmazonS3Object>();
                foreach(var file in responseObject.Response.S3Objects)
                {
                    var newfile = new AmazonS3Object()
                    {
                        Key = file.Key,
                        Size = file.Size,
                        LastModified = file.LastModified.ToUniversalTime(),
                    };

                    fileList.Add(newfile);
                }

                callback(fileList);
            }
            else
            {
                Debug.Log(responseObject.Exception.Message);
            }
        });
    }

    public void DeleteObject(string name, string filePath)
    {
        var request = new DeleteObjectRequest()
        {
            BucketName = S3BucketName,
            Key = filePath
        };

        Client.DeleteObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                Debug.Log("Deleted: " + name);
            }
            else
            {
                Debug.Log(responseObj.Exception.Message);
            }
        });
    }

    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    private IAmazonS3 _s3Client;
    private AWSCredentials _credentials;

    private AWSCredentials Credentials
    {
        get
        {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
            return _credentials;
        }
    }

    private IAmazonS3 Client
    {
        get
        {
            if (_s3Client == null)
            {
                _s3Client = new AmazonS3Client(Credentials, _S3Region);
            }
            return _s3Client;
        }
    }
}