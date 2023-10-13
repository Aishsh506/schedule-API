using Common.DTO;
using Common.Enums;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using ScheduleBL.DTO;
using ScheduleDAL;
using ScheduleDAL.Entities;

namespace ScheduleBL
{
    public interface IScheduleProviderService
    {
        DayDTO GetDaySchedule(DateTime day, Guid filterItemId, ScheduleFilter filter, bool withBreaks = true);
        DayDTO[] GetWeekSchedule(DateTime weekDate, Guid filterItemId, ScheduleFilter filter, bool withBreaks = true);
    }
    public class ScheduleProviderService : IScheduleProviderService
    {
        private readonly ScheduleContext _context;
        public ScheduleProviderService(ScheduleContext context)
        {
            _context = context;
        }
        public DayDTO GetDaySchedule(DateTime day, Guid filterItemId, ScheduleFilter filter, bool withBreaks)
        {
            var lessons = GetLessons(day, filterItemId, filter, false);
            var booked = GetBookedLessons(day, filterItemId, filter, false);

            var DTO = new DayDTO(
                day,
                lessons.Select(l => l.ToDTO()).ToList(),
                booked.Select(b => new BookingRequestDTO
                {
                    Id = b.Id,
                    Date = b.Date,
                    Timeslot = b.Timeslot,
                    Audience = b.Audience.ToDTO(),
                    Status = (BookStatus)b.Status,
                    Description = b.Description,
                    Groups = b.Groups != null ? b.Groups.Select(g => g.ToDTO()).ToArray() : Array.Empty<GroupDTO>()
                }).ToList());
            DTO.CalculateTimeslots(withBreaks);
            return DTO;
        }
        public DayDTO[] GetWeekSchedule(DateTime weekDate, Guid filterItemId, ScheduleFilter filter, bool withBreaks)
        {
            var lessons = GetLessons(weekDate, filterItemId, filter, true);
            var booked = GetBookedLessons(weekDate, filterItemId, filter, true);
            var weekStart = DayOfWeek.Monday.ToDateTime(weekDate);

            var WeekDTO = new DayDTO[6];
            for (var i = 1; i <= 6; i++)
            {
                WeekDTO[i - 1] = new DayDTO(
                    weekStart.AddDays(i),
                    lessons.Where(l => l.DayOfWeek == (DayOfWeek)i).Select(l => l.ToDTO()).ToList(),
                    booked.Where(b => b.Date.DayOfWeek == (DayOfWeek)i).Select(b => new BookingRequestDTO
                    {
                        Id = b.Id,
                        Date = b.Date,
                        Timeslot = b.Timeslot,
                        Audience = b.Audience.ToDTO(),
                        Status = (BookStatus)b.Status,
                        Description = b.Description,
                        Groups = b.Groups != null ? b.Groups.Select(g => g.ToDTO()).ToArray() : Array.Empty<GroupDTO>()
                    }).ToList());
                WeekDTO[i - 1].CalculateTimeslots(withBreaks);
            }
            return WeekDTO;
        }
        private List<Lesson> GetLessons(DateTime day, Guid filterItemId, ScheduleFilter filter, bool getWeek)
        {
            object? filterItem = null;
            switch(filter)
            {
                case ScheduleFilter.Audience:
                    filterItem = _context.Audiences.Find(filterItemId);
                    break;
                case ScheduleFilter.Professor:
                    filterItem = _context.Professors.Find(filterItemId);
                    break;
                case ScheduleFilter.Group:
                    filterItem = _context.Groups.Find(filterItemId);
                    break;
            }
            if (filterItem == null)
            {
                throw new KeyNotFoundException(filter.ToString().ToLower());
            }

            var weekStart = getWeek ? DayOfWeek.Monday.ToDateTime(day) : (DateTime?)null;
            var weekEnd = getWeek ? DayOfWeek.Saturday.ToDateTime(day) : (DateTime?)null;

            var lessons = _context.Lessons.Where(l =>
                l.StartDate <= (weekEnd ?? day) &&
                l.EndDate >= (weekStart ?? day) &&
                (getWeek || l.DayOfWeek == day.DayOfWeek) &&
                (filter != ScheduleFilter.Audience || l.AudienceId == filterItemId) &&
                (filter != ScheduleFilter.Professor || l.ProfessorId == filterItemId) &&
                (filter != ScheduleFilter.Group || _context.LessonGroups.Any(lg =>
                    lg.LessonId == l.Id &&
                    lg.GroupId == filterItemId)))
                .Include(l => l.Groups)
                .Include(l => l.Professor)
                .Include(l => l.Subject)
                .Include(l => l.Audience)
                .ToList();
            if (!getWeek)
            {
                return lessons;
            }

            return lessons.Where(l =>
                l.StartDate <= l.DayOfWeek.ToDateTime(day) &&
                l.EndDate >= l.DayOfWeek.ToDateTime(day)).ToList();
        }
        private List<BookedLesson> GetBookedLessons(DateTime day, Guid filterItemId, ScheduleFilter filter, bool getWeek)
        {
            if (filter == ScheduleFilter.Professor)
            {
                return new List<BookedLesson>();
            }

            object? filterItem = null;
            switch (filter)
            {
                case ScheduleFilter.Audience:
                    filterItem = _context.Audiences.Find(filterItemId);
                    break;
                case ScheduleFilter.Group:
                    filterItem = _context.Groups.Find(filterItemId);
                    break;
            }
            if (filterItem == null)
            {
                throw new KeyNotFoundException(filter.ToString().ToLower());
            }

            var weekStart = getWeek ? DayOfWeek.Monday.ToDateTime(day) : (DateTime?)null;
            var weekEnd = getWeek ? DayOfWeek.Saturday.ToDateTime(day) : (DateTime?)null;

            return _context.BookedLessons.Where(b =>
                b.Status == ScheduleDAL.Enums.BookStatus.Approved &&
                b.Date <= (weekEnd ?? day) &&
                b.Date >= (weekStart ?? day) &&
                (filter != ScheduleFilter.Audience || b.AudienceId == filterItemId) &&
                (filter != ScheduleFilter.Group || _context.BookedLessonGroups.Any(bg =>
                    bg.BookedLessonId == b.Id &&
                    bg.GroupId == filterItemId)))
                .Include(l => l.Groups)
                .Include(l => l.Audience)
                .ToList();
        }
    }
}

