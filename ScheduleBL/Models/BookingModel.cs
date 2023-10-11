using System.ComponentModel.DataAnnotations;

namespace ScheduleBL.Models
{
    public class BookingModel
    {
        [Required]public DateTime Date { get; set; }
        [Required]public uint Timeslot { get; set; }
        [Required]public Guid AudienceId { get; set; }
        [MinLength(1)]public Guid[]? GroupIds { get; set; }
        public string? Description { get; set; }
    }
}
