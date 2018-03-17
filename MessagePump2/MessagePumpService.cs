using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using NLog;
using System;
using System.Configuration;
using System.Security;

namespace AWSFun
{
    public class MessagePumpService
    {
        public const string QUEUE_URL = "https://sqs.us-west-2.amazonaws.com/390295467717/ggkilmon_dev_queue";
        private AmazonSQSClient _client = null;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MessagePumpService()
        {
            SecureString awsAccessKeyId = ConfigurationManager.AppSettings["AWS:AccessKeyId"].ToSecureString();
            SecureString awsSecretAccessKey = ConfigurationManager.AppSettings["AWS:SecretAccessKey"].ToSecureString();

            AmazonSQSConfig config = new AmazonSQSConfig();
            config.RegionEndpoint = RegionEndpoint.USWest2;

            _client = new AmazonSQSClient(awsAccessKeyId.ToUnsecureString(), awsSecretAccessKey.ToUnsecureString(), config);
        }       
    }
}
