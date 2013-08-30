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
    class ACL
    {
        public static void ACLSerial(String filePath, String bucketname1, String bucketname2)
        {
            System.Console.WriteLine("\nACLSerial testing!!");
            NameValueCollection appConfig = ConfigurationManager.AppSettings;
            
            AmazonS3Config config = new AmazonS3Config();
            config.CommunicationProtocol = Protocol.HTTP;
            config.ServiceURL = "s3.hicloud.net.tw";

            AmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(appConfig["AWSAccessKey"], appConfig["AWSSecretKey"],config);
            String bucketName = bucketname1;
            String objectName = "hello";
            //versioning test
            String vbucketName = bucketname2;

            //**********************************************************************************************************************************************************
            // Set Version test 環境，目前新增一專用 bucket = netversion，內有兩筆 Object 資訊(Object 已刪除，帶有 delete marker)，為了測試方便，此環境不共用測試後手動刪除
            // 若於佈板後刪除環境，請預先建立環境，執行132~150行程式
            //**********************************************************************************************************************************************************
           
                //PutBucket
                ////System.Console.WriteLine("PutBucket-version: {0}\n", vbucketName);
                s3Client.PutBucket(new PutBucketRequest().WithBucketName(vbucketName));

                //PutBucketVersioning
                SetBucketVersioningResponse putVersioningResult = s3Client.SetBucketVersioning(new SetBucketVersioningRequest().WithBucketName(vbucketName).WithVersioningConfig(new S3BucketVersioningConfig().WithStatus("Enabled")));
                ////System.Console.WriteLine("PutBucketVersioning, requestID:{0}\n", putVersioningResult.RequestId);

                //PutObject
                ////System.Console.WriteLine("PutObject!");
                PutObjectRequest objectVersionRequest = new PutObjectRequest();
                objectVersionRequest.WithBucketName(vbucketName);
                objectVersionRequest.WithKey(objectName);
                objectVersionRequest.WithFilePath(filePath);
                PutObjectResponse objectVersionResult = s3Client.PutObject(objectVersionRequest);
                ////System.Console.WriteLine("Uploaded Object Etag: {0}\n", objectVersionResult.ETag);
                
            //PutBucket
            ////System.Console.WriteLine("PutBucket: {0}\n", bucketName);
            s3Client.PutBucket(new PutBucketRequest().WithBucketName(bucketName));

            //PutObject
            ////System.Console.WriteLine("PutObject!");
            PutObjectRequest request = new PutObjectRequest();
            request.WithBucketName(bucketName);
            request.WithKey(objectName);
            request.WithFilePath(filePath);
            PutObjectResponse PutResult = s3Client.PutObject(request);
            ////System.Console.WriteLine("Uploaded Object Etag: {0}\n", PutResult.ETag);

            //PutBucketACL
            SetACLRequest aclRequest = new SetACLRequest();
            S3AccessControlList aclConfig = new S3AccessControlList();
            aclConfig.AddGrant(new S3Grantee().WithCanonicalUser("canonicalidhrchu", "hrchu"), S3Permission.FULL_CONTROL);
            aclConfig.WithOwner(new Owner().WithDisplayName("hrchu").WithId("canonicalidhrchu"));
            aclConfig.WithGrants(new S3Grant().WithGrantee(new S3Grantee().WithCanonicalUser("canonicalidannyren", "annyren")).WithPermission(S3Permission.READ_ACP));
            aclConfig.RemoveGrant(new S3Grantee().WithCanonicalUser("canonicalidhrchu", "hrchu"), S3Permission.FULL_CONTROL);
            aclRequest.WithBucketName(bucketName);
            aclRequest.WithACL(aclConfig);

            SetACLResponse putBucketACLResult = s3Client.SetACL(aclRequest);
            ////System.Console.WriteLine("\nPutBucketACL, requestID:{0}",putBucketACLResult.RequestId);

            //GetBucketACL
            GetACLResponse getBucketACLResult = s3Client.GetACL(new GetACLRequest().WithBucketName(bucketName));
            ////System.Console.WriteLine("\nGetBucketACL Result:\n{0}\n",getBucketACLResult.ResponseXml);

            //**********************************************************************************************************************************************************

            //PutBucketACL (cannedacl)
            SetACLRequest cannedaclRequest = new SetACLRequest();
            cannedaclRequest.WithBucketName(bucketName);
            cannedaclRequest.WithCannedACL(S3CannedACL.PublicRead);

            SetACLResponse putBucketCannedACLResult = s3Client.SetACL(cannedaclRequest);
            ////System.Console.WriteLine("\nPutBucketCannedACL, requestID:{0}", putBucketCannedACLResult.RequestId);

            //GetBucketACL
            GetACLResponse getBucketCannedACLResult = s3Client.GetACL(new GetACLRequest().WithBucketName(bucketName));
            ////System.Console.WriteLine("\nGetBucketCannedACL Result:\n{0}\n", getBucketCannedACLResult.ResponseXml);

            //**********************************************************************************************************************************************************

            //PutObjectACL
            SetACLRequest objectACLRequest = new SetACLRequest();
            S3AccessControlList objectACLConfig = new S3AccessControlList();
            objectACLConfig.WithOwner(new Owner().WithDisplayName("hrchu").WithId("canonicalidhrchu"));
            objectACLConfig.WithGrants(new S3Grant().WithGrantee(new S3Grantee().WithCanonicalUser("canonicalidhrchu", "hrchu")).WithPermission(S3Permission.FULL_CONTROL));
            objectACLConfig.WithGrants(new S3Grant().WithGrantee(new S3Grantee().WithCanonicalUser("canonicalidannyren", "annyren")).WithPermission(S3Permission.WRITE_ACP));

            objectACLRequest.WithBucketName(bucketName);
            objectACLRequest.WithKey(objectName);
            objectACLRequest.WithACL(objectACLConfig);

            SetACLResponse putObjectACLResult = s3Client.SetACL(objectACLRequest);
            ////System.Console.WriteLine("\nPutObjectACL, requestID:{0}", putObjectACLResult.RequestId);

            //GetObjectACl
            GetACLResponse getObjectACLResult = s3Client.GetACL(new GetACLRequest().WithBucketName(bucketName).WithKey(objectName));
            ////System.Console.WriteLine("\nGetObjectACL Result:\n{0}\n", getObjectACLResult.ResponseXml);

            //**********************************************************************************************************************************************************

            //PutObjectACL (cannedacl)
            SetACLRequest objectCannedACLRequest = new SetACLRequest();

            objectCannedACLRequest.WithBucketName(bucketName);
            objectCannedACLRequest.WithKey(objectName);
            objectCannedACLRequest.WithCannedACL(S3CannedACL.PublicRead);

            SetACLResponse putObjectCannedACLResult = s3Client.SetACL(objectCannedACLRequest);
            ////System.Console.WriteLine("\nPutObjectCannedACL, requestID:{0}", putObjectCannedACLResult.RequestId);

            //GetObjectACl
            GetACLResponse getObjectCannedACLResult = s3Client.GetACL(new GetACLRequest().WithBucketName(bucketName).WithKey(objectName));
            ////System.Console.WriteLine("\nGetObjectCannedACL Result:\n{0}\n", getObjectCannedACLResult.ResponseXml);

            //**********************************************************************************************************************************************************

           

            //PutObjectACL-version test*********************************************************************************************************************************
            String versionid = s3Client.GetObject(new GetObjectRequest().WithBucketName(vbucketName).WithKey(objectName)).VersionId;
             ////System.Console.WriteLine("vid {0}\n",versionid);
            //PutObjectACL-versionid
              SetACLRequest objectVersionACLRequest = new SetACLRequest();
              S3AccessControlList objectVersionACLConfig = new S3AccessControlList();
              objectVersionACLConfig.WithOwner(new Owner().WithDisplayName("hrchu").WithId("canonicalidhrchu"));
              objectVersionACLConfig.WithGrants(new S3Grant().WithGrantee(new S3Grantee().WithCanonicalUser("canonicalidhrchu", "hrchu")).WithPermission(S3Permission.FULL_CONTROL));
              objectVersionACLConfig.WithGrants(new S3Grant().WithGrantee(new S3Grantee().WithCanonicalUser("canonicalidannyren", "annyren")).WithPermission(S3Permission.WRITE_ACP));

              objectVersionACLRequest.WithBucketName(vbucketName);
              objectVersionACLRequest.WithKey(objectName);
              objectVersionACLRequest.WithVersionId(versionid);
              objectVersionACLRequest.WithACL(objectVersionACLConfig);

              SetACLResponse putObjectVersionACLResult = s3Client.SetACL(objectVersionACLRequest);
              ////System.Console.WriteLine("\nPutObjectACL Version, requestID:{0}", putObjectVersionACLResult.RequestId);

              //GetObjectACl-versionid
              GetACLResponse getObjectVersionACLResult = s3Client.GetACL(new GetACLRequest().WithBucketName(vbucketName).WithKey(objectName).WithVersionId(versionid));
              ////System.Console.WriteLine("\nGetObjectACL Version Result:\n{0}\n", getObjectVersionACLResult.ResponseXml);

              //PutObjectACL (cannedacl)-versionid
              SetACLRequest objectVersionCannedACLRequest = new SetACLRequest();

              objectVersionCannedACLRequest.WithBucketName(vbucketName);
              objectVersionCannedACLRequest.WithKey(objectName);
              objectVersionCannedACLRequest.WithVersionId(versionid);
              objectVersionCannedACLRequest.WithCannedACL(S3CannedACL.PublicRead);

              SetACLResponse putObjectVersionCannedACLResult = s3Client.SetACL(objectVersionCannedACLRequest);
              ////System.Console.WriteLine("\nPutObjectCannedACL Version, requestID:{0}", putObjectVersionCannedACLResult.RequestId);

              //GetObjectACl(cannedacl)-versionid
              GetACLResponse getObjectVersionCannedACLResult = s3Client.GetACL(new GetACLRequest().WithBucketName(vbucketName).WithKey(objectName).WithVersionId(versionid));
              ////System.Console.WriteLine("\nGetObjectCannedACL Version Result:\n{0}\n", getObjectVersionCannedACLResult.ResponseXml);
                
              
  
              //SuspendVersioning
              s3Client.SetBucketVersioning(new SetBucketVersioningRequest().WithBucketName(vbucketName).WithVersioningConfig(new S3BucketVersioningConfig().WithStatus("Suspended")));


              ListVersionsResponse result = s3Client.ListVersions(new ListVersionsRequest().WithBucketName(vbucketName));
              foreach(S3ObjectVersion v in result.Versions)            
              {
                    //DeleteObjects
                    ////System.Console.WriteLine("Delete {0} Objects!",result.Versions.Count);
                    s3Client.DeleteObject(new DeleteObjectRequest().WithBucketName(v.BucketName).WithKey(v.Key).WithVersionId(v.VersionId));
              }


              //DeleteVersionBucket
              ////System.Console.WriteLine("Delete Bucket!");
              s3Client.DeleteBucket(new DeleteBucketRequest().WithBucketName(vbucketName));
              //DeleteObject
              ////System.Console.WriteLine("Delete Object!");
              s3Client.DeleteObject(new DeleteObjectRequest().WithBucketName(bucketName).WithKey(objectName));
              //DeleteBucket
              ////System.Console.WriteLine("Delete Bucket!");
              s3Client.DeleteBucket(new DeleteBucketRequest().WithBucketName(bucketName));
            
            ////System.Console.WriteLine("END!");
        }
    }
}
