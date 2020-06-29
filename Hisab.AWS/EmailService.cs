using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task<HttpStatusCode> SendRegistrationEmail(string toEmail, string name, string registerLink)
        {
            AWSCredentials credentials = new BasicAWSCredentials(_accessKey,_secretKey);
            
            using (var client = new AmazonSimpleEmailServiceClient(credentials,RegionEndpoint.USEast1))
            {
                var destination = new Destination(new List<string>{toEmail});
                var message = new Message();
                message.Subject = new Content("Registration: Welcome to Hisaab.io");
                //message.Body = new Body(new Content($"Welcome to hisab. Please click on this link to finish registration. {registerLink}"));

                message.Body = new Body();
                message.Body.Html = new Content(GetRegisterEmailBody(registerLink,name));

                var sendRequest = new SendEmailRequest("register@hisaab.io", destination, message);

                var response = await client.SendEmailAsync(sendRequest);

                return response.HttpStatusCode;

                

            }

           
        }

        private string GetRegisterEmailBody(string registerLink, string name)
        {
            string body =
                "<div style=\"background-color:#008da8;color:#ffffff; padding: 1rem 2%; \">" +
                  "<div> <h1 style=\"font-size: 2rem;font-weight: 300; letter-spacing: 10px; \"> Hisaab </h1> <h2 style=\"font-size: 1rem;font-weight: 300; \">Managing Trip Expenses, Simplified</h2></div>" +
                     
                "</div>" +

                "<body>" +
                  $"<div style=\"background-color: #F4F5F7; padding: 15px 32px;   \"> " +
                  $"Hey {name}, <br/><br/>" +
                  $"Welcome to Hisaab. Please click on <i>Confirm Email</i> button to finish the registration process. <br/><br/>" +
                  $"<a style=\"background-color: #e2ed07;color:#003B5C;padding: 15px 32px; text-align: center;text-decoration: none; display: inline-block; font-size: 16px; \" href={registerLink}>Confirm Email</a> </div>" +
                 "</body>";

            return body;
        }

        public async Task<HttpStatusCode> SendForgotPasswordLink(string toEmail, string name, string forgotPasswordLink)
        {
            AWSCredentials credentials = new BasicAWSCredentials(_accessKey, _secretKey);

            using (var client = new AmazonSimpleEmailServiceClient(credentials, RegionEndpoint.USEast1))
            {
                var destination = new Destination(new List<string> { toEmail });
                var message = new Message();
                message.Subject = new Content("Re-set password link");

                message.Body = new Body();
                message.Body.Html = new Content(GetResetPasswordEmailBody(forgotPasswordLink, name));

                

                var sendRequest = new SendEmailRequest("admin@hisaab.io", destination, message);

                var response = await client.SendEmailAsync(sendRequest);

                return response.HttpStatusCode;



            }
        }

        private string GetResetPasswordEmailBody(string forgotPasswordLink, string name)
        {
            string body =
                "<div style=\"background-color:#008da8;color:#ffffff; padding: 1rem 2%; \">" +
                  "<div> <h1 style=\"font-size: 2rem;font-weight: 300; letter-spacing: 10px; \"> Hisaab </h1> <h2 style=\"font-size: 1rem;font-weight: 300; \">Managing Trip Expenses, Simplified</h2></div>" +

                "</div>" +

                "<body>" +
                  $"<div style=\"background-color: #F4F5F7; padding: 15px 32px;   \"> " +
                  $"Hey {name}, <br/><br/>" +
                  $"Please click on <i>Reset Password</i> button to reset your password. <br/><br/>" +
                  $"<a style=\"background-color: #e2ed07;color:#003B5C;padding: 15px 32px; text-align: center;text-decoration: none; display: inline-block; font-size: 16px; \" href={forgotPasswordLink}>Reset Password</a> </div>" +
                 "</body>";

            return body;
        }
    }
}
