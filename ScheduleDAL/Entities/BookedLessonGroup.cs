namespace ScheduleDAL.Entities
{
    public class BookedLessonGroup
    {
        public Guid BookedLessonId { get; set; }
        public BookedLesson BookedLesson { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }
}
