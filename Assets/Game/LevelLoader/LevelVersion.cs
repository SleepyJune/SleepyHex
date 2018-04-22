using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;

[Serializable]
[DynamoDBTable("LevelVersion")]
public class LevelVersion
{
    [DynamoDBHashKey]
    public string levelName;

    [DynamoDBProperty]
    public int levelID;

    [DynamoDBProperty]
    public int version;

    [DynamoDBProperty]
    public bool solved;

    [DynamoDBProperty]
    public string dateModified;

    [DynamoDBProperty]
    public string dateCreated;

    [DynamoDBProperty]
    public float difficulty;
}
