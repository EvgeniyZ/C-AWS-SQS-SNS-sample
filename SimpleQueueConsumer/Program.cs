using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;

namespace SimpleQueueConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"aws.{environment}.json", optional: false, reloadOnChange: true);

            MainAsync(builder.Build()).GetAwaiter().GetResult();

            Console.ReadKey();
        }

        static async Task MainAsync(IConfigurationRoot configuration)
        {
            var serviceUrl = configuration.GetConnectionString("");
            var queueName = configuration.GetSection("").Value;
            var sqsConfig = new AmazonSQSConfig {ServiceURL = serviceUrl};
            AmazonSQSClient sqsClient = new AmazonSQSClient(sqsConfig);
            var request = new CreateQueueRequest
            {
                QueueName = queueName,
                Attributes = new Dictionary<string, string>
                {
                    {"ReceiveMessageWaitTimeSeconds", "20"},
                }
            };
            var createQueueResponse = await sqsClient.CreateQueueAsync(request);
            if (createQueueResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                var setQueueAttributeRequest = new SetQueueAttributesRequest
                {
                    Attributes = new Dictionary<string, string>
                    {
                        {
                            "RedrivePolicy",
                            @"{ ""deadLetterTargetArn"" : ""DEAD_LETTER_QUEUE_ARN"", ""maxReceiveCount"" : ""10""}"
                        }
                    },
                    QueueUrl = createQueueResponse.QueueUrl
                };
                await sqsClient.SetQueueAttributesAsync(setQueueAttributeRequest);
                await ConsumeQueue(sqsClient, createQueueResponse.QueueUrl);
            }
        }

        private static async Task ConsumeQueue(IAmazonSQS sqsClient, string queueUrl)
        {
            while (true)
            {
                var receiveMessageRequest = new ReceiveMessageRequest {QueueUrl = queueUrl};
                var receiveMessageResponse = await sqsClient.ReceiveMessageAsync(receiveMessageRequest);
                var entries = new List<DeleteMessageBatchRequestEntry>();
                foreach (var message in receiveMessageResponse.Messages)
                {
                    var result = await ProcessMessage(message);
                    entries.Add(new DeleteMessageBatchRequestEntry(result.messageId, result.messageReceipt));
                    //await sqsClient.DeleteMessageAsync(new DeleteMessageRequest(queueUrl, result.messageReceipt));
                }
                await sqsClient.DeleteMessageBatchAsync(queueUrl, entries);
            }
        }

        private static async Task<(string messageId, string messageReceipt)> ProcessMessage(Message message)
        {
            Console.WriteLine("START PROCESSING");
            await Task.Delay(50);
            Console.WriteLine(message.MessageId);
            Console.WriteLine(message.Body);
            Console.WriteLine(message.ReceiptHandle);
            Console.WriteLine("END");

            return (message.MessageId, message.ReceiptHandle);
        }
    }
}