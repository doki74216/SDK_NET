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
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello, World!");
            String filePath = "D:\\pic.jpg";

            //******************************************************************************************
            //
            //
            //
            //
            //lifecycle：netlifecycle
            //******************************************************************************************
            
            //Bucket.bucketSerial(filePath);

            //Object.objectSerial(filePath);

            //Policy.policySerial();
            
            //Logging.loggingSerial();

            //Versioning.VersioningSerial();

            //Website.WebsiteSerial();
            
            //ACL.ACLSerial(filePath);

            MPU.mpuSerial(filePath);

            //Lifecycle.lifecycleSerial();

            //MPUmd5.mpuSerial(filePath);
            
            Console.ReadLine();
        }
    }
}
