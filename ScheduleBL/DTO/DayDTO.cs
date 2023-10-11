using Common.DTO;

namespace ScheduleBL.DTO
{
    public class DayDTO
    {
        public DateTime Date { get; set; }
        public LessonDTO?[] Lessons { get; set; }
    }
}
