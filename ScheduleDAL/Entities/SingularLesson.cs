using System.ComponentModel.DataAnnotations;

namespace ScheduleDAL.Entities
{
    public class SingularLesson
    {
        public DateTime Date { get; set; }
        public Lesson Lesson { get; set; }
        [Key] public Guid LessonId { get; set; }
    }
}
