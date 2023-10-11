using ScheduleDAL.Enums;

namespace ScheduleDAL.Entities
{
    public class BookedLesson
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public uint Timeslot { get; set; }
        public Guid AudienceId { get; set;}
        public Audience Audience { get; set; }
        public ICollection<Group>? Groups { get; set; }
        public string? Description { get; set; }
        public BookStatus Status { get; set; }
    }
}
