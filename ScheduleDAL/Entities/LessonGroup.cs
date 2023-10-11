namespace ScheduleDAL.Entities
{
    public class LessonGroup
    {
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }
}
