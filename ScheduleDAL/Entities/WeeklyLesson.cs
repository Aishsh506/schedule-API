using System.ComponentModel.DataAnnotations;

namespace ScheduleDAL.Entities
{
    public class WeeklyLesson
    {
        public DayOfWeek DayOfWeek { get; set; }
        public Lesson Lesson { get; set; }
        [Key] public Guid LessonId { get; set; }
    }
}
