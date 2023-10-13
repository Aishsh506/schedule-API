using Common.DTO;
using Common.Enums;

namespace ScheduleBL.DTO
{
    public class DayDTO
    {
        public DayDTO(DateTime date, List<LessonDTO> lessons, List<BookingRequestDTO> bookedLessons)
        {
            Date = date;
            _lessons = lessons;
            _booked = bookedLessons;

            Timeslots = new List<TimeslotDTO?>();
        }
        public void CalculateTimeslots(bool withBreaks)
        {
            if (withBreaks)
            {
                int breakCount = 0;
                uint timeslotCount = 1;
                for(uint i = 1; i <= 7; i++)
                {
                    var lesson = _lessons.FirstOrDefault(l => l.Timeslot == i);
                    if (lesson != null)
                    {
                        if (breakCount > 0)
                        {
                            Timeslots.Add(new TimeslotDTO(TimeslotType.Break, timeslotCount, breakCount));
                            timeslotCount++;
                            breakCount = 0;
                        }
                        Timeslots.Add(new TimeslotDTO(TimeslotType.Lesson, lesson.Timeslot, lesson: lesson));
                        timeslotCount++;
                        continue;
                    }
                    var booked = _booked.FirstOrDefault(b => b.Timeslot == i);
                    if (booked != null)
                    {
                        if (breakCount > 0)
                        {
                            Timeslots.Add(new TimeslotDTO(TimeslotType.Break, timeslotCount, breakCount));
                            timeslotCount++;
                            breakCount = 0;
                        }
                        Timeslots.Add(new TimeslotDTO(TimeslotType.Booked, booked.Timeslot, bookedLesson: booked));
                        timeslotCount++;
                        continue;
                    }
                    breakCount++;
                }
            }
            else
            {
                for(uint i = 1; i <= 7; i++)
                {
                    var lesson = _lessons.FirstOrDefault(l => l.Timeslot == i);
                    if (lesson != null)
                    {
                        Timeslots.Add(new TimeslotDTO(TimeslotType.Lesson, i, lesson: lesson));
                        continue;
                    }
                    var booked = _booked.FirstOrDefault(b => b.Timeslot == i);
                    if (booked != null)
                    {
                        Timeslots.Add(new TimeslotDTO(TimeslotType.Booked, i, bookedLesson: booked));
                        continue;
                    }
                    Timeslots.Add(new TimeslotDTO(TimeslotType.Empty, i));
                }
            }
        }
        public DateTime Date { get; set; }
        public List<TimeslotDTO?> Timeslots { get; set; }
        private List<LessonDTO> _lessons;
        private List<BookingRequestDTO> _booked;
    }
}
