using ScheduleDAL.Enums;

namespace ScheduleDAL.Entities
{
    public class Lesson
    {
        public Guid Id { get; set; }
        public uint Timeslot { get; set; }
        public Audience Audience { get; set; }
        public LessonType LessonType { get; set; }
        public Subject Subject { get; set; }
        public Professor Professor { get; set; }
        public ICollection<Group> Groups { get; set; }
    }
}
