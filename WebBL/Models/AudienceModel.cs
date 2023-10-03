using System.ComponentModel.DataAnnotations;

namespace WebBL.Models
{
    public class AudienceModel
    {
        [Required]public string Name { get; set; }
        [Required]public Guid BuildingId { get; set; }
    }
}
