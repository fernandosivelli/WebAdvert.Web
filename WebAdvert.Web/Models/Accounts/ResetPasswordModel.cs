using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class ResetPasswordModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(6, ErrorMessage = "New Password must be at least six characters long!")]
        [Display(Name ="New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirmation do not match!")]
        [Display(Name ="Confirm New Password")]
        public string ConfirmNewPassword { get; set; }
    }
}
