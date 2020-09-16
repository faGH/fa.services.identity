using System.ComponentModel.DataAnnotations;

namespace FrostAura.Services.Identity.Models
{
    /// <summary>
    /// Registration request structure.
    /// </summary>
    public class RegistrationViewModel : LoginViewModel
    {
        /// <summary>
        /// Confirmation of password.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "A valid confirm password is required that matches the password.")]
        public string ConfirmPassword { get; set; }
    }
}
