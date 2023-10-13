using System.ComponentModel.DataAnnotations;

namespace ScheduleBL.Models
{
    public class BookingModel
    {
        [Required] public DateTime Date { get; set; }
        [Required][Range(1, 7)] public uint Timeslot { get; set; }
        [Required] public Guid AudienceId { get; set; }
        [MinLength(1)] public Guid[]? GroupIds { get; set; }
        public string? Description { get; set; }
    }
}
