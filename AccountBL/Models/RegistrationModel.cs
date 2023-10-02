using System.ComponentModel.DataAnnotations;

namespace AccountBL.Models
{
    public class RegistrationModel
    {
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
        public string? Username { get; set; }

    }
}
