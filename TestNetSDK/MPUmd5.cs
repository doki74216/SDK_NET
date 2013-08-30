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
    class MPUmd5
    {
        public static void mpuSerial(String fPath, String bucketname1, String bucketname2)
        {
            System.Console.WriteLine("\nMPU-md5 Serial testing!!");
            NameValueCollection appConfig = ConfigurationManager.AppSettings;

            AmazonS3Config config = new AmazonS3Config();
            config.CommunicationProtocol = Protocol.HTTP;
            config.ServiceURL = "s3.hicloud.net.tw";
            
            AmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(appConfig["AWSAccessKey"], appConfig["AWSSecretKey"],config);
            String bucketName = bucketname1;
            String objectNameA = "hello";
            String objectNameB = "bonjour";

            //PutBucket
            PutBucketResponse response = s3Client.PutBucket(new PutBucketRequest().WithBucketName(bucketName));

            //Initial MPU * 2
            InitiateMultipartUploadResponse InitialResultA = s3Client.InitiateMultipartUpload(new InitiateMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameA));
            InitiateMultipartUploadResponse InitialResultB = s3Client.InitiateMultipartUpload(new InitiateMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameB));
            String objectAUID = InitialResultA.UploadId;
            String objectBUID = InitialResultB.UploadId;
            //System.Console.WriteLine("\nInitial MPU:{0},uploadID:{1}", objectNameA, objectAUID);
            //System.Console.WriteLine("Initial MPU:{0},uploadID:{1}", objectNameB, objectBUID);

            //Upload Part * 2
            UploadPartResponse part1Result = s3Client.UploadPart(new UploadPartRequest().WithBucketName(bucketName).WithKey(objectNameA).WithPartNumber(1).WithUploadId(objectAUID).WithFilePath(fPath).WithFilePosition(10).WithGenerateChecksum(true));
            UploadPartResponse part2Result = s3Client.UploadPart(new UploadPartRequest().WithBucketName(bucketName).WithKey(objectNameA).WithPartNumber(2).WithUploadId(objectAUID).WithFilePath(fPath).WithFilePosition(10).WithMD5Digest("9JM2qNaZV8d15yZCyHFvTg=="));
            //System.Console.WriteLine("\nUpload Part Result: part 1 requestID:{0} & part 2 requestID:{1}", part1Result.RequestId, part2Result.RequestId);

            //List MPUs
            ListMultipartUploadsResponse listMPUsResult = s3Client.ListMultipartUploads(new ListMultipartUploadsRequest().WithBucketName(bucketName));
            //System.Console.WriteLine("\nList MPUs Result:\n{0}", listMPUsResult.ResponseXml);

            //Complete MPU
            List<UploadPartResponse> uploadResponses = new List<UploadPartResponse>();
            uploadResponses.Add(part1Result);
            uploadResponses.Add(part2Result);
            CompleteMultipartUploadResponse completeResult = s3Client.CompleteMultipartUpload(new CompleteMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameA).WithUploadId(objectAUID).WithPartETags(uploadResponses));
            //System.Console.WriteLine("\nComplete MPUs Result:\n{0}", completeResult.ResponseXml);

            //Upload Part Copy
            CopyPartResponse copyPartResult = s3Client.CopyPart(new CopyPartRequest().WithSourceBucket(bucketName).WithDestinationBucket(bucketName)
                .WithSourceKey(objectNameA).WithDestinationKey(objectNameB).WithPartNumber(1).WithUploadID(objectBUID));
            //System.Console.WriteLine("\nCopy Part Result: requestID:{0}", copyPartResult.RequestId);

            //List Upload Parts
            ListPartsResponse listPartsResult = s3Client.ListParts(new ListPartsRequest().WithBucketName(bucketName).WithKey(objectNameB).WithUploadId(objectBUID));
            //System.Console.WriteLine("\nListParts Result:\n{0}", listMPUsResult.ResponseXml);

            //Abort MPU
            AbortMultipartUploadResponse abortResult = s3Client.AbortMultipartUpload(new AbortMultipartUploadRequest().WithBucketName(bucketName).WithKey(objectNameB).WithUploadId(objectBUID));
            //System.Console.WriteLine("\nAbortMPU Result, requestID:{0}", abortResult.RequestId);

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
