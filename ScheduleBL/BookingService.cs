using Common.DTO;
using Common.Enums;
using Common.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ScheduleBL.DTO;
using ScheduleBL.Models;
using ScheduleDAL;
using ScheduleDAL.Entities;

namespace ScheduleBL
{
    public interface IBookingService
    {
        BookingRequestDTO[] GetUserRequests(Guid userId);
        BookingRequestDTO[] GetNewRequests();
        AudienceDTO[] GetAvailableAudiences(DateTime date, uint timeslot);
        Task<IdDTO> CreateRequest(Guid userId, BookingModel bookingModel);
        Task CancelRequest(Guid userId, Guid bookedLessonId);
        Task ModerateRequest(Guid bookedLessonId, bool approved);
    }
    public class BookingService : IBookingService
    {
        private readonly ScheduleContext _context;
        public BookingService(ScheduleContext context)
        {
            _context = context;
        }
        public BookingRequestDTO[] GetUserRequests(Guid userId)
        {
            var bookedLessons = _context.BookedLessons
                .Where(l => l.UserId == userId)
                .Include(l => l.Groups)
                .Include(l => l.Audience)
                .OrderBy(l => l.Status)
                .ToArray();
            return bookedLessons.Select(l => new BookingRequestDTO
            {
                Id = l.Id,
                UserId = userId,
                Date = l.Date,
                Timeslot = l.Timeslot,
                Audience = l.Audience.ToDTO(),
                Status = (BookStatus)l.Status,
                Description = l.Description,
                Groups = l.Groups != null ? l.Groups.Select(g => g.ToDTO()).ToArray() : Array.Empty<GroupDTO>()
            }).ToArray();
        }
        public BookingRequestDTO[] GetNewRequests()
        {
            var bookedLessons = _context.BookedLessons
                .Where(l => l.Status == ScheduleDAL.Enums.BookStatus.New)
                .Include(l => l.Groups)
                .Include(l => l.Audience)
                .OrderBy(l => l.Status)
                .ToArray();
            return bookedLessons.Select(l => new BookingRequestDTO
            {
                Id = l.Id,
                UserId = l.UserId,
                Date = l.Date,
                Timeslot = l.Timeslot,
                Audience = l.Audience.ToDTO(),
                Status = (BookStatus)l.Status,
                Description = l.Description,
                Groups = l.Groups != null ? l.Groups.Select(g => g.ToDTO()).ToArray() : Array.Empty<GroupDTO>()
            }).ToArray();
        }
        public AudienceDTO[] GetAvailableAudiences(DateTime date, uint timeslot)
        {
            if (timeslot < 1 && timeslot > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(timeslot));
            }
            var available = _context.Audiences.Where(a =>
                !_context.Lessons.Any(l =>
                    l.Audience == a &&
                    l.StartDate <= date &&
                    l.EndDate >= date &&
                    l.DayOfWeek == date.DayOfWeek &&
                    l.Timeslot == timeslot) &&
                !_context.BookedLessons.Any(b =>
                    b.Date == date &&
                    b.Timeslot == timeslot &&
                    b.Status == ScheduleDAL.Enums.BookStatus.Approved &&
                    b.Audience == a)).ToArray();
            return available.Select(a => a.ToDTO()).ToArray();
        }
        public async Task<IdDTO> CreateRequest(Guid userId, BookingModel model)
        {
            if (_context.Lessons.Any(l =>
                    l.StartDate <= model.Date &&
                    l.EndDate >= model.Date &&
                    l.DayOfWeek == model.Date.DayOfWeek &&
                    l.Timeslot == model.Timeslot &&
                    l.AudienceId == model.AudienceId))
            {
                throw new DataConflictException(ScheduleItems.Audience);
            }

            if (_context.BookedLessons.Any(l =>
                l.Status == ScheduleDAL.Enums.BookStatus.Approved &&
                l.Date == model.Date &&
                l.Timeslot == model.Timeslot &&
                l.AudienceId == model.AudienceId))
            {
                throw new DataConflictException();
            }

            var bookedLesson = new BookedLesson
            {
                UserId = userId,
                Date = model.Date,
                Timeslot = model.Timeslot,
                Description = model.Description,
                AudienceId = model.AudienceId
            };

            await _context.BookedLessons.AddAsync(bookedLesson);
            if (model.GroupIds != null)
            {
                foreach (var groupId in model.GroupIds)
                {
                    await _context.BookedLessonGroups.AddAsync(new BookedLessonGroup
                    {
                        BookedLesson = bookedLesson,
                        GroupId = groupId
                    });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e) when (e.InnerException is SqlException)
            {
                var sqlEx = (SqlException)e.InnerException;
                if (sqlEx.Number != 547)
                {
                    throw;
                }
                var items = new string[] { "Audience", "Group" };
                foreach (var item in items)
                {
                    if (sqlEx.Message.Contains(item))
                    {
                        throw new KeyNotFoundException(item.ToLower());
                    }
                }
                throw;
            }
            return new IdDTO(bookedLesson.Id);
        }
        public async Task CancelRequest(Guid userId, Guid bookedLessonId)
        {
            var bookedLesson = _context.BookedLessons.Find(bookedLessonId);
            if (bookedLesson == null)
            {
                throw new KeyNotFoundException();
            }
            if (bookedLesson.UserId != userId)
            {
                throw new AccessDeniedException();
            }
            bookedLesson.Status = ScheduleDAL.Enums.BookStatus.Canceled;
            await _context.SaveChangesAsync();
        }
        public async Task ModerateRequest(Guid bookedLessonId, bool approved)
        {
            var bookedLesson = _context.BookedLessons.Find(bookedLessonId);
            if (bookedLesson == null)
            {
                throw new KeyNotFoundException();
            }
            bookedLesson.Status = approved ? ScheduleDAL.Enums.BookStatus.Approved : ScheduleDAL.Enums.BookStatus.Rejected;
            await _context.SaveChangesAsync();
        }
    }
}
