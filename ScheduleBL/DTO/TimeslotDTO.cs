using Common.DTO;
using Common.Enums;
using Common.Dictionaries;

namespace ScheduleBL.DTO
{
    public class TimeslotDTO
    {
        public TimeslotDTO(
            TimeslotType type,
            uint slotNumber,
            int slots = 1,
            LessonDTO? lesson = null,
            BookingRequestDTO? bookedLesson = null)
        {
            Type = type;
            Starts = LessonTimes.Start[(int)slotNumber];
            Ends = LessonTimes.End[(int)slotNumber + slots - 1];
            Lesson = lesson;
            BookedLesson = bookedLesson;
            LessonNumber = slotNumber;
        }
        public TimeslotType Type { get; set; }
        public uint LessonNumber { get; set; }
        public string Starts { get; set; }
        public string Ends { get; set; }
        public LessonDTO? Lesson { get; set; }
        public BookingRequestDTO? BookedLesson { get; set; }
    }
}
