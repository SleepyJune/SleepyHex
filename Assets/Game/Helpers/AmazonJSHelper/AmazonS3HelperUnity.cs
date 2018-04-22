using System.Collections;
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

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;

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

    string DynamoRegion = RegionEndpoint.USEast1.SystemName;
    private RegionEndpoint _DynamoRegion
    {
        get { return RegionEndpoint.GetBySystemName(DynamoRegion); }
    }

    void Start()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            UnityInitializer.AttachToGameObject(this.gameObject);

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

    public void PostObject(string fileName, string s, IDictionary<string, string> metadata)
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
            Metadata = metadata,
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

    public void ListLevelVersions(AmazonS3Helper.ListVersionCallback callback)
    {
        //new ScanCondition("version", ScanOperator.GreaterThanOrEqual, 0));
        /*var search = Context.ScanAsync<LevelVersion>();
        
        search.GetRemainingAsync((result) => {
            if (result.Exception == null)
            {
                callback(result.Result);
            }
        });*/

        var request = new ScanRequest
        {
            TableName = "LevelVersion",
        };

        dynamoDBClient.ScanAsync(request, (result) => {
            if(result.Exception == null)
            {
                List<LevelVersion> versions = new List<LevelVersion>();

                foreach(var item in result.Response.Items)
                {
                    var newVersion = new LevelVersion()
                    {
                        //category = item["levelName"].S,
                        levelName = item["levelName"].S,
                        levelID = Int32.Parse(item["levelID"].N),
                        version = Int32.Parse(item["version"].N),
                        solved = item["version"].BOOL,
                        dateModified = item["dateModified"].S,
                        dateCreated = item["dateCreated"].S,
                        difficulty = item.ContainsKey("difficulty") ? float.Parse(item["difficulty"].N) : 0,
                        //timestamp = Int32.Parse(item["version"].N),                        
                    };

                    versions.Add(newVersion);
                }

                callback(versions);
            }
            else
            {
                Debug.Log(result.Exception);
            }
        });
    }

    public void UploadLevelVersion(LevelVersion version)
    {
        Context.SaveAsync(version, (result) => {
            if (result.Exception == null)
            {
                Debug.Log("Uploaded version: " + version.levelName);
            }
        });
    }

    public void DeleteLevelVersion(string key)
    {
        Context.DeleteAsync<LevelVersion>(key, (result) => {
            if (result.Exception == null)
            {
                Debug.Log("Deleted version: " + key);
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
    private DynamoDBContext _context;

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

    private DynamoDBContext Context
    {
        get
        {
            if (_context == null)
                _context = new DynamoDBContext(dynamoDBClient);

            return _context;
        }
    }

    private static IAmazonDynamoDB _ddbClient;

    protected IAmazonDynamoDB dynamoDBClient
    {
        get
        {
            if (_ddbClient == null)
            {
                _ddbClient = new AmazonDynamoDBClient(Credentials, _DynamoRegion);
            }

            return _ddbClient;
        }
    }
}