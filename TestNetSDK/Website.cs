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
    class Website
    {
        public static void WebsiteSerial(String bucketname1, String bucketname2)
        {
            System.Console.WriteLine("\nWebsiteSerial testing!!");
            NameValueCollection appConfig = ConfigurationManager.AppSettings;

            AmazonS3Config config1 = new AmazonS3Config();
            config1.CommunicationProtocol = Protocol.HTTP;
            config1.ServiceURL = "s3.hicloud.net.tw";

            AmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(appConfig["AWSAccessKey"], appConfig["AWSSecretKey"],config1);
            String bucketName = bucketname1;
            String bucketName2 = bucketname2;
            
            //PutBucket
            PutBucketResponse response = s3Client.PutBucket(new PutBucketRequest().WithBucketName(bucketName));
            PutBucketResponse response2 = s3Client.PutBucket(new PutBucketRequest().WithBucketName(bucketName2));

            //PutBucketWebsite，已修正此問題，待新版 code release 即可正常使用
            WebsiteConfiguration config = new WebsiteConfiguration();
            config.WithErrorDocument("404Test.html");
            config.WithIndexDocumentSuffix("indexTest.html");
            PutBucketWebsiteResponse putBucketWebsiteResult = s3Client.PutBucketWebsite(new PutBucketWebsiteRequest().WithBucketName(bucketName).WithWebsiteConfiguration(config));
            //System.Console.WriteLine("\nPutBucketWebsite, requestID:{0}",putBucketWebsiteResult.RequestId);
            List<RoutingRule> Rules = new List<RoutingRule>();
            Rules.Add(new RoutingRule().WithCondition(new RoutingRuleCondition().WithHttpErrorCodeReturnedEquals("404"))
                                       .WithRedirect(new RoutingRuleRedirect().WithHostName("http://hicloud.net.tw")));
            Rules.Add(new RoutingRule().WithCondition(new RoutingRuleCondition().WithKeyPrefixEquals("index"))
                                       .WithRedirect(new RoutingRuleRedirect().WithHttpRedirectCode("123").WithProtocol("http").WithReplaceKeyWith("404Test.html")));
            Rules.Add(new RoutingRule().WithCondition(new RoutingRuleCondition().WithKeyPrefixEquals("indexTest"))
                                       .WithRedirect(new RoutingRuleRedirect().WithHttpRedirectCode("123").WithProtocol("http").WithReplaceKeyPrefixWith("404Test")));
            
            WebsiteConfiguration config2 = new WebsiteConfiguration();
            config2.RoutingRules = Rules;
            PutBucketWebsiteResponse putBucketWebsiteResult2 = s3Client.PutBucketWebsite(new PutBucketWebsiteRequest().WithBucketName(bucketName).WithWebsiteConfiguration(config2));
            
            //GetBucketWebsite
            GetBucketWebsiteResponse getBucketWebsiteResult = s3Client.GetBucketWebsite(new GetBucketWebsiteRequest().WithBucketName(bucketName));
            //System.Console.WriteLine("\nGetBucketWebsite Result:\n{0}",getBucketWebsiteResult.ResponseXml);

           
            WebsiteConfiguration config3 = new WebsiteConfiguration();
            config3.WithErrorDocument("404Test.html");
            config3.WithIndexDocumentSuffix("indexTest.html");
 //BUG----> //config3.WithRedirectAllRequestsTo(new RoutingRuleRedirect().WithHostName("http://hicloud.net.tw"));
            s3Client.PutBucketWebsite(new PutBucketWebsiteRequest().WithBucketName(bucketName).WithWebsiteConfiguration(config3));
            
            //DeleteBucketWebsite
            DeleteBucketWebsiteResponse deleteBucketWebsiteResult = s3Client.DeleteBucketWebsite(new DeleteBucketWebsiteRequest().WithBucketName(bucketName));
            DeleteBucketWebsiteResponse deleteBucketWebsiteResult2 = s3Client.DeleteBucketWebsite(new DeleteBucketWebsiteRequest().WithBucketName(bucketName2));
            //System.Console.WriteLine("\nDeleteBucketWebsite, requestID:{0}",deleteBucketWebsiteResult.RequestId);


            //DeleteBucket
            //System.Console.WriteLine("Delete Bucket!");
            s3Client.DeleteBucket(new DeleteBucketRequest().WithBucketName(bucketName));
            s3Client.DeleteBucket(new DeleteBucketRequest().WithBucketName(bucketName2));
            //System.Console.WriteLine("END!");
        }
    }
}
