using System.Net;
using System.Threading.Tasks;

namespace Hisab.AWS
{
    public interface IEmailService
    {
        Task<HttpStatusCode> SendRegistrationEmail(string toEmail, string registerLink);

        Task<HttpStatusCode> SendForgotPasswordLink(string toEmail, string forgotPasswordLink);
    }
}