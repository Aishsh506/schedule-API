using Common.DTO;
using Common.Enums;
using Common.Exceptions;
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
        Task CreateRequest(Guid userId, BookingModel bookingModel);
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
                .ToList();
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
                .ToList();
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
        public async Task CreateRequest(Guid userId, BookingModel model)
        {
            var matchingLessons = _context.Lessons
                .Where(l =>
                    l.StartDate <= model.Date &&
                    l.EndDate >= model.Date &&
                    l.DayOfWeek == model.Date.DayOfWeek &&
                    l.Timeslot == model.Timeslot)
            .Include(l => l.Groups)
            .ToList();

            var sameAud = matchingLessons.FirstOrDefault(l => l.AudienceId == model.AudienceId);
            if (sameAud != null)
            {
                throw new DataConflictException(ScheduleItems.Audience, sameAud.Id);
            }

            if (_context.BookedLessons.Any(l =>
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

            if (model.GroupIds != null)
            {
                foreach (var groupId in model.GroupIds)
                {
                    var sameGroup = matchingLessons.FirstOrDefault(l =>
                        l.Groups.Select(g => g.Id).Contains(groupId));
                    if (sameGroup != null)
                    {
                        throw new DataConflictException(ScheduleItems.Group, sameGroup.Id);
                    }
                    await _context.BookedLessonGroups.AddAsync(new BookedLessonGroup
                    {
                        BookedLesson = bookedLesson,
                        GroupId = groupId
                    });
                }
            }
            await _context.BookedLessons.AddAsync(bookedLesson);
            await _context.SaveChangesAsync();
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
