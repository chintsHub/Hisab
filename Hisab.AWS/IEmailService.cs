using System.Net;
using System.Threading.Tasks;

namespace Hisab.AWS
{
    public interface IEmailService
    {
        Task<HttpStatusCode> SendRegistrationEmail(string toEmail, string name, string registerLink);

        Task<HttpStatusCode> SendForgotPasswordLink(string toEmail, string name, string forgotPasswordLink);
    }
}