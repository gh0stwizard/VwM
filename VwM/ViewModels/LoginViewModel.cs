using System.ComponentModel.DataAnnotations;

namespace VwM.ViewModels
{
    public class LoginViewModel
    {
        [Required(
            ErrorMessageResourceType = typeof(Resources.ViewError),
            ErrorMessageResourceName = "RequiredString")]
        [Display(Name = "Login")]
        public string Username { get; set; }


        [Required(
            ErrorMessageResourceType = typeof(Resources.ViewError),
            ErrorMessageResourceName = "RequiredString")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}
