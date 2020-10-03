using System.ComponentModel.DataAnnotations;

namespace FrostAura.Services.Identity.Models
{
    /// <summary>
    /// Credentials request structure.
    /// </summary>
    public class CredentialsModel
    {
        /// <summary>
        /// User email address.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "A valid email address is required.")]
        public string Email { get; set; }
        /// <summary>
        /// Account password.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "A valid password is required.")]
        public string Password { get; set; }
    }
}
