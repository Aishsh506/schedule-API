using System.ComponentModel.DataAnnotations;

namespace WebBL.Models
{
    public class ProfessorModel
    {
        [Required]public string FullName { get; set; }
        [Required]public string ShortName { get; set; }
    }
}
