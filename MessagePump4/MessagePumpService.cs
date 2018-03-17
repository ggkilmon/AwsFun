using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
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

        public void SendMessage(MessagePumpModel message)
        {
            try
            {
                logger.Debug("Sending message: {0} - {1}", message.Id, message.Message);
                SendMessageRequest request = new SendMessageRequest(QUEUE_URL, JsonConvert.SerializeObject(message));
                SendMessageResponse response = _client.SendMessage(request);
                logger.Debug("Message sent with status code: {0}", response.HttpStatusCode);
            }
            catch (Exception ex)
            {
                logger.Error("Send message failed: {0}", ex.ToString());
            }
        }

        public void ConsumeMessage()
        {
            try
            {
                logger.Debug("Receiving message(s)");
                ReceiveMessageRequest request = new ReceiveMessageRequest(QUEUE_URL);
                ReceiveMessageResponse response = _client.ReceiveMessage(request);
                logger.Debug("Message(s) received with status code: {0}", response.HttpStatusCode);
                if (response.Messages.Count > 0)
                {
                    foreach (var m in response.Messages)
                    {
                        var model = JsonConvert.DeserializeObject<MessagePumpModel>(m.Body);
                        logger.Debug("Message {0}; Guid {2}; {1}", m.MessageId, model.Message, model.Id);

                        try
                        {
                            logger.Debug("Deleting message {0}", m.MessageId);
                            DeleteMessageRequest deleteRequest = new DeleteMessageRequest(QUEUE_URL, m.ReceiptHandle);
                            DeleteMessageResponse deleteResponse = _client.DeleteMessage(deleteRequest);
                            logger.Debug("Message {0} deleted with status code: {1}", m.MessageId, deleteResponse.HttpStatusCode);
                        }
                        catch (Exception ex)
                        {
                            logger.Debug("Delete message failed; left on queue for consumption again; {0}", ex.ToString());
                        }
                    }
                }
                else
                {
                    logger.Debug("Retrieved 0 messages");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Retrieve message failed; {0}", ex.ToString());
            }
        }
    }
}
