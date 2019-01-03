using System.ComponentModel.DataAnnotations;

namespace EIS.WebApp.Models
{
    public class ChangePasswordModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Please Enter Old Password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage ="Please Enter New Password")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Password not matches with New Passoword")]
        [Required(ErrorMessage ="Please Confirm your new password")]
        public string ConfirmNewPassword { get; set; }
    }
}
