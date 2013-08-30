using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading;
using System.Globalization;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;



namespace TestNetSDK
{
    class Program
    {
        static void Main(string[] args)
        {
            String filePath = "./pic.jpg";
            //更改測試 Bucket,bucketname1為一般用(put object,src,copyfrom),bucketname2為(versioning,destination bucket...)
            String bucketname1 = "allentest1";
            String bucketname2 = "allentest2";
           
            System.Console.WriteLine("S3 .NET SDK Serial Test-\nbucketname1:{0},bucketname2:{1}", bucketname1, bucketname2);
            
            Bucket.bucketSerial(filePath,bucketname1,bucketname2);

            Logging.loggingSerial(bucketname1, bucketname2);

            Object.objectSerial(filePath, bucketname1, bucketname2);

            Policy.policySerial(bucketname1, bucketname2);

            Website.WebsiteSerial(bucketname1, bucketname2);

            Lifecycle.lifecycleSerial(bucketname1, bucketname2);

            Versioning.VersioningSerial(filePath, bucketname1, bucketname2);
            
            ACL.ACLSerial(filePath,bucketname1,bucketname2);
            
            MPU.mpuSerial(filePath,bucketname1,bucketname2);

            MPUmd5.mpuSerial(filePath,bucketname1,bucketname2);
          
            System.Console.WriteLine("\nS3 .NET SDK Serial Test Done!\n Press any Key to continue...");
            
            Console.ReadLine();
        }
    }
}
