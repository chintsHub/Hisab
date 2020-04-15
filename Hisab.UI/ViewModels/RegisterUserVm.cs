using System.ComponentModel.DataAnnotations;

namespace Hisab.UI.ViewModels
{
    public class RegisterUserVm
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }
    }

   
}
