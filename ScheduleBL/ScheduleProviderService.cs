using Common.DTO;
using Common.Enums;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using ScheduleBL.DTO;
using ScheduleDAL;
using ScheduleDAL.Entities;
using System.Runtime.CompilerServices;

namespace ScheduleBL
{
    public interface IScheduleProviderService
    {
        DayDTO GetDaySchedule(DateTime day, Guid filterItemId, ScheduleFilter filter);
        DayDTO[] GetWeekSchedule(DateTime weekDate, Guid filterItemId, ScheduleFilter filter);
    }
    public class ScheduleProviderService : IScheduleProviderService
    {
        private readonly ScheduleContext _context;
        public ScheduleProviderService(ScheduleContext context)
        {
            _context = context;
        }
        public DayDTO GetDaySchedule(DateTime day, Guid filterItemId, ScheduleFilter filter)
        {
            var lessons = GetLessons(day, filterItemId, filter, false);

            var DTO = new DayDTO {
                Date = day,
                Lessons = new LessonDTO?[7]
            };

            foreach ( var lesson in lessons )
            {
                DTO.Lessons[lesson.Timeslot - 1] = lesson.ToDTO();
            }
            return DTO;
        }
        public DayDTO[] GetWeekSchedule(DateTime weekDate, Guid filterItemId, ScheduleFilter filter)
        {
            var lessons = GetLessons(weekDate, filterItemId, filter, true);
            var weekStart = DayOfWeek.Monday.ToDateTime(weekDate);

            var WeekDTO = new DayDTO[6];
            for (var i = 0; i < 6; i++)
            {
                WeekDTO[i] = new DayDTO
                {
                    Date = weekStart.AddDays(i),
                    Lessons = new LessonDTO?[7]
                };
            }

            foreach ( var lesson in lessons )
            {
                WeekDTO[(int)lesson.DayOfWeek - 1].Lessons[lesson.Timeslot - 1] = lesson.ToDTO();
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
                .Include(l => l.Audience).ThenInclude(a => a.Building)
                .ToList();
            if (!getWeek)
            {
                return lessons;
            }

            return lessons.Where(l =>
                l.StartDate <= l.DayOfWeek.ToDateTime(day) &&
                l.EndDate >= l.DayOfWeek.ToDateTime(day)).ToList();
        }
    }
}

