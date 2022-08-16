using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Models
{
    public class LoginViewModel : BaseViewModel
    {
        [Required(ErrorMessage ="Please enter your Username.")]
        [MaxLength(150, ErrorMessage ="Login must not exceed 150 characters.")]
        [Display(Name ="Username")]
        public string Login { get; set; }

        [Required(ErrorMessage ="Please enter your password.")]
        [Display(Name ="Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name ="Remember Me (Not Recommended)")]
        public bool RememberMe { get; set; }

    }
}
