# C-AWS-SQS-SNS-sample
A sample application on C# that use AWS SQS, SNS and Lambda features.
guide: 
    http://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-netcore.html
    http://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/Welcome.html
    http://docs.aws.amazon.com/sns/latest/dg/welcome.html

video: 
    https://www.youtube.com/watch?v=UesxWuZMZqI

Install dotnet new -i Amazon.Lambda.Templates::* - templates for aws asp net core lambda projects

SAMPLE steps:

dotnet new lambda.AspNetCoreWebAPI -n SimpleProducer
dotnet new lambda.S3 -n SimpleConsumer

Lambda supports only LTS .NET CORE version (current, 1.0)
also, register SNS topic and SQS standard queue with dead-letter-queue