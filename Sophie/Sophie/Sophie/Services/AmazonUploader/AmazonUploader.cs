using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using App.Core.Units.Log4Net;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sophie.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace awsTestUpload
{

    public class AmazonUploader
    {
        private readonly ILog _log4net = log4net.LogManager.GetLogger(typeof(AmazonUploader));

        private readonly  IConfiguration _configuration;
        private readonly AmazonS3Client client;
        private List<string> listFakeUrl = new List<string>()
        {
            "sample/products/product_1.png",
            "sample/products/product_2.png",
            "sample/products/product_3.png",
            "sample/products/product_4.png",
            "sample/products/product_5.png",
            "sample/products/product_6.png",
            "sample/products/product_7.png",
            "sample/products/product_8.png",
            "sample/products/temp_medicine_image.jpeg",
            "sample/products/temp_medicine_video.mp4",
            "sample/promotions/promotion.mp4",
            "sample/promotions/promotion_1.png",
            "sample/promotions/promotion_2.png",
            "sample/promotions/promotion_3.png",
            "sample/promotions/promotion_4.png",
            "sample/promotions/promotion_5.png",
            "sample/promotions/promotion_6.png",
        };

        public AmazonUploader(IConfiguration configuration)
        {
            _configuration = configuration;
            client = new AmazonS3Client(_configuration["AWSOptions:AccessKey"], _configuration["AWSOptions:SecretKey"], RegionEndpoint.GetBySystemName(_configuration["AWSOptions:Region"]));
        }

        public async Task<string> SendMyFileToS3(IFormFile localFilePath, string fileNameInS3, string filePath)
        {
            try
            {
                DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
                fileNameInS3 = now.ToString("yyyyMMddHHmmssfff") + fileNameInS3;
                var BucketName = _configuration["AWSOptions:BucketName"];
                using (var ms = new MemoryStream())
                {
                    localFilePath.CopyTo(ms);
                    Stream myStream = ms;
                    PutObjectRequest putObjectRequest = new PutObjectRequest()
                    {
                        BucketName = BucketName ?? "sophies3",
                        AutoCloseStream = false,
                        InputStream = myStream,
                        Key = filePath + "/" + fileNameInS3,
                        ContentType = localFilePath.ContentType,
                        CannedACL = S3CannedACL.PublicReadWrite,
                    };
                    PutObjectResponse putObjectResponse = await client.PutObjectAsync(putObjectRequest);
                    return fileNameInS3;
                }

            }
            catch(Exception ex)
            {
                Logs.debug("[AmazonUploader] Exception SendMyFileToS3: " + ex.StackTrace);
                _log4net.Error("[AmazonUploader] Exception SendMyFileToS3: " + ex.StackTrace);
                return "";
            }
        }
        public string GetMyFileToS3(string keyName)
        {
            try
            {
                var BucketName = _configuration["AWSOptions:BucketName"];
                // Create a CopyObject request
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = BucketName,
                    Key = keyName,
                    Expires = DateTime.Now.AddDays(1)
                };

                // Get path for request
                return client.GetPreSignedURL(request);
            }
            catch (Exception ex)
            {
                Logs.debug("[AmazonUploader] Exception GetMyFileToS3: " + ex.StackTrace);
                _log4net.Error("[AmazonUploader] Exception GetMyFileToS3: " + ex.StackTrace);
                return "";
            }
        }

        public string GetUrlImage(string keyName, string filePath)
        {
            try
            {
                var BucketName = _configuration["AWSOptions:BucketName"];
                var Region = _configuration["AWSOptions:Region"];
                if (string.IsNullOrEmpty(keyName)) return "";
                if (listFakeUrl.Contains(keyName))
                    return String.Format("https://{0}.s3-{1}.amazonaws.com/{2}", BucketName, Region, keyName);

                return String.Format("https://{0}.s3-{1}.amazonaws.com{2}/{3}", BucketName, Region, filePath, keyName);
            }
            catch (Exception ex)
            {
                Logs.debug("[AmazonUploader] Exception GetUrlImage: " + ex.StackTrace);
                _log4net.Error("[AmazonUploader] Exception GetUrlImage: " + ex.StackTrace);
                return "";
            }
        }

        public void MultiObjectDeleteAsync(List<string> listKey)
        {
            try
            {
                var BucketName = _configuration["AWSOptions:BucketName"];
                DeleteObjectsRequest multiObjectDeleteRequest = new DeleteObjectsRequest
                {
                    BucketName = BucketName,
                };
                var index = 0;
                foreach (var item in listKey)
                {
                    if (!string.IsNullOrEmpty(item) && !listFakeUrl.Contains(item))
                    {
                        multiObjectDeleteRequest.AddKey(item);
                        index++;
                    }
                }
                if (index > 0)
                {
                    var a = client.DeleteObjectsAsync(multiObjectDeleteRequest).Result;
                }
            }
            catch (Exception ex)
            {
                Logs.debug("[AmazonUploader] Exception MultiObjectDeleteAsync: " + ex.StackTrace);
                _log4net.Error("[AmazonUploader] Exception MultiObjectDeleteAsync: " + ex.StackTrace);
            }
        }
    }
}