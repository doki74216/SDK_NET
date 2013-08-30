﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Threading;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;


namespace TestNetSDK
{
    class MPU
    {
        public static void mpuSerial(String fPath, String bucketname1, String bucketname2)
        {
            //語系切換到en-US，不然datetime會有中文字 by Allen
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            
            System.Console.WriteLine("\nMPUSerial testing!!");
            NameValueCollection appConfig = ConfigurationManager.AppSettings;

            AmazonS3Config config = new AmazonS3Config();
            config.CommunicationProtocol = Protocol.HTTP;
            config.ServiceURL = "s3.hicloud.net.tw";
            
            AmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(appConfig["AWSAccessKey"], appConfig["AWSSecretKey"],config);
            String bucketName = bucketname1;
            String objectName = "hello";
            String objectNameA = "photos/2006/January/sample.jpg";
            String objectNameB = "photos/2006/February/sample.jpg";
            String objectNameC = "photos/2006/March/sample.jpg";
            String objectNameD = "videos/2006/March/sample.wmv";
            String objectNameE = "sample.jpg";

            //PutBucket
            PutBucketResponse response = s3Client.PutBucket(new PutBucketRequest().WithBucketName(bucketName));
            
            //********************************************************************************************************************************
            //Initial with CannedACL 
            InitiateMultipartUploadRequest MPUCannedACL = new InitiateMultipartUploadRequest();
            MPUCannedACL.WithBucketName(bucketName);
            MPUCannedACL.WithKey(objectNameA);
            MPUCannedACL.WithCannedACL(S3CannedACL.PublicRead);
            MPUCannedACL.WithContentType("Content-Type: text/html");
            MPUCannedACL.WithStorageClass(S3StorageClass.Standard);
            //MPUCannedACL.WithServerSideEncryptionMethod(ServerSideEncryptionMethod.AES256);
            InitiateMultipartUploadResponse MPUCannedACLResult = s3Client.InitiateMultipartUpload(MPUCannedACL);
            String MPUCannedACLUID = MPUCannedACLResult.UploadId;
            //System.Console.WriteLine("\nInitial MPUCanned uploadID:{0}", MPUCannedACLUID);
            AbortMultipartUploadResponse abortCannedACLResult = s3Client.AbortMultipartUpload(new AbortMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameA).WithUploadId(MPUCannedACLUID));
            //System.Console.WriteLine("\nAbort MPUCanned uploadID:{0}", MPUCannedACLUID);

            //Initial with GrantACL
            InitiateMultipartUploadRequest MPUACL = new InitiateMultipartUploadRequest();
            MPUACL.WithBucketName(bucketName);
            MPUACL.WithKey(objectNameA);
            MPUACL.WithGrants(new S3Grant().WithGrantee(new S3Grantee().WithCanonicalUser("canonicalidhrchu", "hrchu")).WithPermission(S3Permission.FULL_CONTROL));
            MPUACL.WithGrants(new S3Grant().WithGrantee(new S3Grantee().WithCanonicalUser("canonicalidannyren", "annyren")).WithPermission(S3Permission.WRITE_ACP));
            MPUACL.WithWebsiteRedirectLocation("http://hicloud.hinet.net/");
            InitiateMultipartUploadResponse MPUACLResult = s3Client.InitiateMultipartUpload(MPUACL);
            String MPUACLUID = MPUACLResult.UploadId;
            //System.Console.WriteLine("\nInitial MPU Grants uploadID:{0}", MPUACLUID);
            AbortMultipartUploadResponse abortACLResult = s3Client.AbortMultipartUpload(new AbortMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameA).WithUploadId(MPUACLUID));
            //System.Console.WriteLine("\nAbort MPU Grants uploadID:{0}", MPUACLUID);
            //********************************************************************************************************************************
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("A", "ABC");

            //Initial MPU * 5
            InitiateMultipartUploadResponse InitialResultA = s3Client.InitiateMultipartUpload(new InitiateMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameA).WithMetaData(nvc));
            InitiateMultipartUploadResponse InitialResultB = s3Client.InitiateMultipartUpload(new InitiateMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameB));
            InitiateMultipartUploadResponse InitialResultC = s3Client.InitiateMultipartUpload(new InitiateMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameC));
            InitiateMultipartUploadResponse InitialResultD = s3Client.InitiateMultipartUpload(new InitiateMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameD));
            InitiateMultipartUploadResponse InitialResultE = s3Client.InitiateMultipartUpload(new InitiateMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameE));
            String objectAUID = InitialResultA.UploadId;
            String objectBUID = InitialResultB.UploadId;
            String objectCUID = InitialResultC.UploadId;
            String objectDUID = InitialResultD.UploadId;
            String objectEUID = InitialResultE.UploadId;
            //System.Console.WriteLine("\nInitial MPU:{0},uploadID:{1}",objectNameA,objectAUID);
            //System.Console.WriteLine("Initial MPU:{0},uploadID:{1}", objectNameB, objectBUID);
            //System.Console.WriteLine("\nInitial MPU:{0},uploadID:{1}", objectNameC, objectCUID);
            //System.Console.WriteLine("Initial MPU:{0},uploadID:{1}", objectNameD, objectDUID);
            //System.Console.WriteLine("Initial MPU:{0},uploadID:{1}", objectNameE, objectEUID);
            
            //List MPUs
            ListMultipartUploadsResponse listMPUsResult = s3Client.ListMultipartUploads(new ListMultipartUploadsRequest().WithBucketName(bucketName));
            //System.Console.WriteLine("\nList MPUs with WithMaxUploads Result:\n{0}", listMPUsResult.ResponseXml);

            //**************List MPUs with MaxUploads=1*************************************************************************************
            ListMultipartUploadsResponse listMPUsMaxUPsResult = s3Client.ListMultipartUploads(new ListMultipartUploadsRequest().WithBucketName(bucketName).WithMaxUploads(1));
            //System.Console.WriteLine("\nList MPUs with WithMaxUploads Result:\n{0}", listMPUsMaxUPsResult.ResponseXml);

            //**************List MPUs with Delimiter /, result:<Key>sample.jpg</Key> only*************************************************************************************
            ListMultipartUploadsResponse listMPUsDelimiterResult = s3Client.ListMultipartUploads(new ListMultipartUploadsRequest().WithBucketName(bucketName).WithDelimiter("/"));
            //System.Console.WriteLine("\nList MPUs with Delimiter Result:\n{0}", listMPUsDelimiterResult.ResponseXml);

            //**************List MPUs with Prefix photos/2006/, result:<Prefix>photos/2006/February/</Prefix>, <Prefix>photos/2006/January/</Prefix> and <Prefix>photos/2006/March/</Prefix>*****************
            ListMultipartUploadsResponse listMPUsPrefixResult = s3Client.ListMultipartUploads(new ListMultipartUploadsRequest().WithBucketName(bucketName).WithPrefix("photos/2006/"));
            //System.Console.WriteLine("\nList MPUs with Prefix Result:\n{0}", listMPUsPrefixResult.ResponseXml);

            //**************List MPUs with Keymarker objectNameB, result:<Prefix>photos/2006/February/</Prefix>, <Prefix>photos/2006/January/</Prefix> and <Prefix>photos/2006/March/</Prefix>*****************
            ListMultipartUploadsResponse listMPUsKeymarkerResult = s3Client.ListMultipartUploads(new ListMultipartUploadsRequest().WithBucketName(bucketName).WithKeyMarker(objectNameB));
            //System.Console.WriteLine("\nList MPUs with Keymarker Result:\n{0}", listMPUsKeymarkerResult.ResponseXml);

            //**************List MPUs with UploadIDmarker objectNameB, result:<Prefix>photos/2006/February/</Prefix>, <Prefix>photos/2006/January/</Prefix> and <Prefix>photos/2006/March/</Prefix>*****************
            ListMultipartUploadsResponse listMPUsUpIDmarkerResult = s3Client.ListMultipartUploads(new ListMultipartUploadsRequest().WithBucketName(bucketName).WithUploadIdMarker(objectBUID));
            //System.Console.WriteLine("\nList MPUs with UpIDmarker Result:\n{0}", listMPUsUpIDmarkerResult.ResponseXml);
            
            //Upload Part * 2            
            UploadPartResponse part1Result = s3Client.UploadPart(new UploadPartRequest().WithBucketName(bucketName).WithKey(objectNameA).WithPartNumber(1).WithUploadId(objectAUID).WithFilePath(fPath).WithFilePosition(10).WithPartSize(6270544));
            UploadPartResponse part2Result = s3Client.UploadPart(new UploadPartRequest().WithBucketName(bucketName).WithKey(objectNameA).WithPartNumber(2).WithUploadId(objectAUID).WithFilePath(fPath).WithFilePosition(10).WithMD5Digest("9JM2qNaZV8d15yZCyHFvTg=="));
            String part1Etag = part1Result.ETag;
            String part2Etag = part2Result.ETag;
            //System.Console.WriteLine("\nUpload Part Result: part 1 requestID:{0} & Etag:{1}", part1Result.RequestId, part1Etag);
            //System.Console.WriteLine("\nUpload Part Result: part 1 requestID:{0} & Etag:{1}", part1Result.RequestId, part2Etag); 
               
            //List Upload Parts
            ListPartsResponse listPartsResult = s3Client.ListParts(new ListPartsRequest().WithBucketName(bucketName).WithKey(objectNameA).WithUploadId(objectAUID));
            //System.Console.WriteLine("\nListParts Result:\n{0}", listPartsResult.ResponseXml);
            //**************List Upload Parts with MaxParts(1), result:只列出Part 1資訊*************************************************************************************
            ListPartsResponse listPartsMaxResult = s3Client.ListParts(new ListPartsRequest().WithBucketName(bucketName).WithKey(objectNameA).WithUploadId(objectAUID).WithMaxParts(1));
            //System.Console.WriteLine("\nListParts With Max Parts Result:\n{0}", listPartsMaxResult.ResponseXml);
            //**************List Upload Parts with PartNumberMarker("1"), result:只列出Part 2資訊*************************************************************************************
            ListPartsResponse listPartsKeymarkerResult = s3Client.ListParts(new ListPartsRequest().WithBucketName(bucketName).WithKey(objectNameA).WithUploadId(objectAUID).WithPartNumberMarker("1"));
            //System.Console.WriteLine("\nListParts With Key Marker Result:\n{0}", listPartsKeymarkerResult.ResponseXml);

            
            //Complete MPU
            List<UploadPartResponse> uploadResponses = new List<UploadPartResponse>();
            uploadResponses.Add(part1Result);
            uploadResponses.Add(part2Result);
            CompleteMultipartUploadResponse completeResult = s3Client.CompleteMultipartUpload(new CompleteMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameA).WithUploadId(objectAUID).WithPartETags(uploadResponses));
            //System.Console.WriteLine("\nComplete MPUs Result:\n{0}", completeResult.ResponseXml);
            //System.Console.WriteLine("\nComplete MPUs Etag:\n{0}", completeResult.ETag);
           
            //Upload Part Copy
            CopyPartResponse copyPartResult = s3Client.CopyPart(new CopyPartRequest().WithSourceBucket(bucketName).WithDestinationBucket(bucketName)
                .WithSourceKey(objectNameA).WithDestinationKey(objectNameB).WithPartNumber(1).WithUploadID(objectBUID));
            //System.Console.WriteLine("\nCopy Part Result: requestID:{0}", copyPartResult.RequestId);
            //**************Upload Part Copy with ETagToMatch / etagsToNotMatch****************************************************
            CopyPartResponse copyPartEtagmatchResult = s3Client.CopyPart(new CopyPartRequest().WithSourceBucket(bucketName).WithDestinationBucket(bucketName)
                .WithSourceKey(objectNameA).WithDestinationKey(objectNameB).WithPartNumber(1).WithUploadID(objectBUID).WithETagsToMatch(completeResult.ETag).WithLastByte(30000));
            //System.Console.WriteLine("\nCopy Part with ETag Match Result: requestID:{0}", copyPartEtagmatchResult.RequestId);
            CopyPartResponse copyPartEtagNOTmatchResult = s3Client.CopyPart(new CopyPartRequest().WithSourceBucket(bucketName).WithDestinationBucket(bucketName)
                .WithSourceKey(objectNameA).WithDestinationKey(objectNameB).WithPartNumber(1).WithUploadID(objectBUID).WithETagsToNotMatch(part1Etag).WithFirstByte(20));
            //System.Console.WriteLine("\nCopy Part with ETag NOT Match Result: requestID:{0}", copyPartEtagNOTmatchResult.RequestId);
            //**************Upload Part Copy with firstByte-lastByte，反向測試，<5GB檔案 error****************************************************
           /* CopyPartResponse copyPartByterangeResult = s3Client.CopyPart(new CopyPartRequest().WithSourceBucket(bucketName).WithDestinationBucket(bucketName)
                .WithSourceKey(objectNameA).WithDestinationKey(objectNameB).WithPartNumber(1).WithUploadID(objectBUID).WithFirstByte(500).WithLastByte(2000));
            //System.Console.WriteLine("\nCopy Part with ETagToMatch Result: requestID:{0}", copyPartByterangeResult.RequestId);*/
   
            //**************Upload Part Copy with modifiedSinceDate / unmodifiedSinceDate**************************************************************
            DateTime dt =DateTime.Now;
            DateTime dt100 =dt.AddDays(100); //增加100天
            DateTime dt30 = dt.AddDays(-30); //減少30天

            CopyPartResponse copyPartDateResult = s3Client.CopyPart(new CopyPartRequest().WithSourceBucket(bucketName).WithDestinationBucket(bucketName).WithSourceKey(objectNameA).WithDestinationKey(objectNameB).WithPartNumber(1).WithUploadID(objectBUID).WithUnmodifiedSinceDate(dt100).WithModifiedSinceDate(dt30));
            //System.Console.WriteLine("\nCopy Part with modifiedSinceDate Result: requestID:{0}", copyPartDateResult.RequestId);
            //**************Upload Part Copy with srcVersionId*************************************************************************************************
           //versioning test
            String vbucketName = bucketname2;

            //PutBucket
            //System.Console.WriteLine("PutBucket-version: {0}\n", vbucketName);
            s3Client.PutBucket(new PutBucketRequest().WithBucketName(vbucketName));

            //PutBucketVersioning
            SetBucketVersioningResponse putVersioningResult = s3Client.SetBucketVersioning(new SetBucketVersioningRequest().WithBucketName(vbucketName).WithVersioningConfig(new S3BucketVersioningConfig().WithStatus("Enabled")));
            //System.Console.WriteLine("PutBucketVersioning, requestID:{0}\n", putVersioningResult.RequestId);

            //PutObject
            //System.Console.WriteLine("PutObject!");
            PutObjectRequest objectVersionRequest = new PutObjectRequest();
            objectVersionRequest.WithBucketName(vbucketName);
            objectVersionRequest.WithKey(objectName);
            objectVersionRequest.WithFilePath(fPath);
            PutObjectResponse objectVersionResult = s3Client.PutObject(objectVersionRequest);
            //System.Console.WriteLine("Uploaded Object Etag: {0}\n", objectVersionResult.ETag);

            String versionid = s3Client.GetObject(new GetObjectRequest().WithBucketName(vbucketName).WithKey(objectName)).VersionId;
            
            CopyPartResponse copyPartVersionidResult = s3Client.CopyPart(new CopyPartRequest().WithSourceBucket(vbucketName).WithDestinationBucket(bucketName)
                 .WithSourceKey(objectName).WithDestinationKey(objectNameB).WithPartNumber(1).WithUploadID(objectBUID).WithSourceVersionId(versionid));
            //System.Console.WriteLine("\nCopy Part with srcVersionId Result: requestID:{0}", copyPartVersionidResult.RequestId);

            //SuspendVersioning
            s3Client.SetBucketVersioning(new SetBucketVersioningRequest().WithBucketName(vbucketName).WithVersioningConfig(new S3BucketVersioningConfig().WithStatus("Suspended")));


            ListVersionsResponse result = s3Client.ListVersions(new ListVersionsRequest().WithBucketName(vbucketName));
            foreach (S3ObjectVersion v in result.Versions)
            {
                //DeleteObjects
                //System.Console.WriteLine("Delete {0} Objects!", result.Versions.Count);
                s3Client.DeleteObject(new DeleteObjectRequest().WithBucketName(v.BucketName).WithKey(v.Key).WithVersionId(v.VersionId));
            }


            //DeleteVersionBucket
            //System.Console.WriteLine("Delete Bucket!");
            s3Client.DeleteBucket(new DeleteBucketRequest().WithBucketName(vbucketName));
            
            
            //Abort MPU
            AbortMultipartUploadResponse abortobBResult = s3Client.AbortMultipartUpload(new AbortMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameB).WithUploadId(objectBUID));
            //System.Console.WriteLine("\nAbortMPU Result, requestID:{0}", abortobBResult.RequestId);
            AbortMultipartUploadResponse abortobCResult = s3Client.AbortMultipartUpload(new AbortMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameC).WithUploadId(objectCUID));
            //System.Console.WriteLine("\nAbortMPU Result, requestID:{0}", abortobCResult.RequestId);
            AbortMultipartUploadResponse abortobDResult = s3Client.AbortMultipartUpload(new AbortMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameD).WithUploadId(objectDUID));
            //System.Console.WriteLine("\nAbortMPU Result, requestID:{0}", abortobDResult.RequestId);
            AbortMultipartUploadResponse abortobEResult = s3Client.AbortMultipartUpload(new AbortMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameE).WithUploadId(objectEUID));
            //System.Console.WriteLine("\nAbortMPU Result, requestID:{0}", abortobEResult.RequestId);
            

            //DeleteObject
            //System.Console.WriteLine("\nDelete Object!\n");
            s3Client.DeleteObject(new DeleteObjectRequest().WithBucketName(bucketName).WithKey(objectNameA));
            //DeleteBucket
            //System.Console.WriteLine("Delete Bucket!");
            s3Client.DeleteBucket(new DeleteBucketRequest().WithBucketName(bucketName));
            //System.Console.WriteLine("END!");
        }
    }
}
