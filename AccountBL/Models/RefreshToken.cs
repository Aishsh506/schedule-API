using System.ComponentModel.DataAnnotations;

namespace AccountBL.Models
{
    public class RefreshToken
    {
        [Required] public string Token { get; set; }
    }
}
