namespace ScheduleDAL.Entities
{
    public class WeeklyLesson
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public Lesson Lesson { get; set; }
    }
}
