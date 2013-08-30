using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace TestNetSDK
{
    class Versioning
    {
        public static void VersioningSerial(String filePath,String bucketname1, String bucketname2)
        {
            System.Console.WriteLine("\nVersioningSerial testing!!");
            NameValueCollection appConfig = ConfigurationManager.AppSettings;

            AmazonS3Config config = new AmazonS3Config();
            config.CommunicationProtocol = Protocol.HTTP;
            config.ServiceURL = "s3.hicloud.net.tw";

            AmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(appConfig["AWSAccessKey"], appConfig["AWSSecretKey"],config);
            String bucketName = bucketname1;
            String objectName = "hello";

            //PutBucket
            PutBucketResponse response = s3Client.PutBucket(new PutBucketRequest().WithBucketName(bucketName));

            //PutBucketVersioning
            SetBucketVersioningResponse putVersioningResult = s3Client.SetBucketVersioning(new SetBucketVersioningRequest().WithBucketName(bucketName).WithVersioningConfig(new S3BucketVersioningConfig().WithStatus("Enabled")));
            //System.Console.WriteLine("PutBucketVersioning, requestID:{0}\n", putVersioningResult.RequestId);

            //PutObject
            PutObjectResponse po1 = s3Client.PutObject(new PutObjectRequest().WithBucketName(bucketName).WithKey(objectName).WithFilePath(filePath));
            PutObjectResponse po2 = s3Client.PutObject(new PutObjectRequest().WithBucketName(bucketName).WithKey(objectName).WithFilePath(filePath));
            PutObjectResponse po3 = s3Client.PutObject(new PutObjectRequest().WithBucketName(bucketName).WithKey(objectName).WithFilePath(filePath));
            
            //GetBucketVersioning
            GetBucketVersioningResponse getVersioningResult = s3Client.GetBucketVersioning(new GetBucketVersioningRequest().WithBucketName(bucketName));
            //System.Console.WriteLine("GetBucketVersioning Result:\n {0}\n",getVersioningResult.ResponseXml);
            
            ListVersionsResponse vresult1 = s3Client.ListVersions(new ListVersionsRequest().WithBucketName(bucketName).WithDelimiter("/").WithMaxKeys(2));
            ListVersionsResponse vresult2 = s3Client.ListVersions(new ListVersionsRequest().WithBucketName(bucketName).WithPrefix("/hel"));
            ListVersionsResponse vresult3 = s3Client.ListVersions(new ListVersionsRequest().WithBucketName(bucketName).WithKeyMarker("hello"));

            //SuspendVersioning
            s3Client.SetBucketVersioning(new SetBucketVersioningRequest().WithBucketName(bucketName).WithVersioningConfig(new S3BucketVersioningConfig().WithStatus("Suspended")));

            ListVersionsResponse result = s3Client.ListVersions(new ListVersionsRequest().WithBucketName(bucketName));
            foreach (S3ObjectVersion v in result.Versions)
            {
                //DeleteObjects
                ////System.Console.WriteLine("Delete {0} Objects!",result.Versions.Count);
                s3Client.DeleteObject(new DeleteObjectRequest().WithBucketName(v.BucketName).WithKey(v.Key).WithVersionId(v.VersionId));
            }

            //DeleteBucket
            //System.Console.WriteLine("Delete Bucket!");
            s3Client.DeleteBucket(new DeleteBucketRequest().WithBucketName(bucketName));
            //System.Console.WriteLine("END!");
        }
    }
}
