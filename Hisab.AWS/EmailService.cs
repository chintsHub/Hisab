using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Hisab.AWS
{
    public class EmailService : IEmailService
    {
        private string _accessKey;
        private string _secretKey;

        public EmailService(string accessKey, string secretKey)
        {
            _accessKey = accessKey;
            _secretKey = secretKey;
        }

        public async Task<HttpStatusCode> SendRegistrationEmail(string toEmail, string registerLink)
        {
            AWSCredentials credentials = new BasicAWSCredentials(_accessKey,_secretKey);
            
            using (var client = new AmazonSimpleEmailServiceClient(credentials,RegionEndpoint.USEast1))
            {
                var destination = new Destination(new List<string>{toEmail});
                var message = new Message();
                message.Subject = new Content("Registration: Welcome to Hisab.io");
                message.Body = new Body(new Content($"Welcome to hisab. Please click on this link to finish registration. {registerLink}"));

                var sendRequest = new SendEmailRequest("register@hisab.io", destination, message);

                var response = await client.SendEmailAsync(sendRequest);

                return response.HttpStatusCode;

               

            }

           
        }

        public async Task<HttpStatusCode> SendForgotPasswordLink(string toEmail, string forgotPasswordLink)
        {
            AWSCredentials credentials = new BasicAWSCredentials(_accessKey, _secretKey);

            using (var client = new AmazonSimpleEmailServiceClient(credentials, RegionEndpoint.USEast1))
            {
                var destination = new Destination(new List<string> { toEmail });
                var message = new Message();
                message.Subject = new Content("Re-set password link");
                message.Body = new Body(new Content($"Please click on this link to reset your password. {forgotPasswordLink}"));

                var sendRequest = new SendEmailRequest("admin@hisab.io", destination, message);

                var response = await client.SendEmailAsync(sendRequest);

                return response.HttpStatusCode;



            }
        }
    }
}
