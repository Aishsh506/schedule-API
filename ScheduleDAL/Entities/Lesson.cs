using ScheduleDAL.Enums;

namespace ScheduleDAL.Entities
{
    public class Lesson
    {
        public Guid Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public uint Timeslot { get; set; }
        public Guid AudienceId { get; set;}
        public Audience Audience { get; set; }
        public LessonType LessonType { get; set; }
        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; }
        public Guid ProfessorId { get; set; }
        public Professor Professor { get; set; }
        public ICollection<Group> Groups { get; set; }
    }
}
