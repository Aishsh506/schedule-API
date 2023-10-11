using Common.Enums;
using Common.DTO;

namespace ScheduleBL.DTO
{
    public class BookingRequestDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public uint Timeslot { get; set; }
        public AudienceDTO Audience { get; set; }
        public GroupDTO[] Groups { get; set; }
        public string? Description { get; set; }
        public BookStatus Status { get; set; }
    }
}
