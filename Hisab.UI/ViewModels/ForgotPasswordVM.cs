using System.ComponentModel.DataAnnotations;

namespace Hisab.UI.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required]
        public string Email { get; set; }
    }

   
}
