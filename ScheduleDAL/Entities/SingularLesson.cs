namespace ScheduleDAL.Entities
{
    public class SingularLesson
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}
