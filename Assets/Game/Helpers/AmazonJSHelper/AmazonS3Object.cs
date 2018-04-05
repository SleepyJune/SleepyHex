using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class AmazonS3Object
{
    //public string ETag;
    public string Key;
    public DateTime LastModified;
    public long LastModifiedTime;
    //public Owner Owner { get; set; }
    public long Size;
    //public S3StorageClass StorageClass { get; set; }
}
