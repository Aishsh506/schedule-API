using Common.DTO;
using Common.Enums;
using Common.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ScheduleDAL;
using ScheduleDAL.Entities;
using ScheduleDAL.Enums;
using WebBL.Models;

namespace ScheduleBL
{
    public interface IScheduleEditorService
    {
        LessonDTO GetLesson(Guid lessonId);
        Task<IdDTO> CreateLesson(LessonModel model);
        Task EditLesson(Guid lessonId, LessonEditModel model);
        Task DeleteLesson(Guid lessonId);
    }
    public class ScheduleEditorService : IScheduleEditorService
    {
        private readonly ScheduleContext _context;
        public ScheduleEditorService(ScheduleContext context)
        {
            _context = context;
        }
        public LessonDTO GetLesson(Guid lessonId)
        {
            var lesson = _context.Lessons.Find(lessonId);
            if (lesson == null)
            {
                throw new KeyNotFoundException();
            }
            _context.Entry(lesson).Reference(l => l.Professor).Load();
            _context.Entry(lesson).Reference(l => l.Audience).Load();
            _context.Entry(lesson).Reference(l => l.Subject).Load();
            _context.Entry(lesson).Collection(l => l.Groups).Load();
            return new LessonDTO
            {
                Id = lesson.Id,
                DayOfWeek = lesson.DayOfWeek,
                StartDate = lesson.StartDate,
                EndDate = lesson.EndDate,
                Timeslot = lesson.Timeslot,
                LessonType = (Common.Enums.LessonType)lesson.LessonType,
                Professor = new ProfessorDTO
                {
                    Id = lesson.Professor.Id,
                    FullName = lesson.Professor.FullName,
                    ShortName = lesson.Professor.ShortName
                },
                Audience = new AudienceDTO
                {
                    Id = lesson.Audience.Id,
                    Name = lesson.Audience.Name
                },
                Subject = new SubjectDTO
                {
                    Id = lesson.Subject.Id,
                    Name = lesson.Subject.Name
                },
                Groups = lesson.Groups.Select(g => new GroupDTO
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToArray()
            };
        }
        public async Task<IdDTO> CreateLesson(LessonModel model)
        {
            CheckForOverlay(
                model.StartDate,
                model.EndDate,
                model.DayOfWeek,
                model.Timeslot,
                model.AudienceId,
                model.ProfessorId,
                model.GroupIds);

            var lesson = new Lesson
            {
                DayOfWeek = model.DayOfWeek,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Timeslot = model.Timeslot,
                AudienceId = model.AudienceId,
                SubjectId = model.SubjectId,
                ProfessorId = model.ProfessorId,
                LessonType = (ScheduleDAL.Enums.LessonType)model.LessonType
            };
            try
            {
                await _context.AddAsync(lesson);
                await _context.AddRangeAsync(model.GroupIds.Select(id => new LessonGroup
                {
                    LessonId = lesson.Id,
                    GroupId = id
                }));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e) when(e.InnerException is SqlException)
            {
                var sqlEx = (SqlException)e.InnerException;
                if (sqlEx.Number != 547)
                {
                    throw;
                }
                var items = new string[] { "Professor", "Audience", "Subject", "Group" };
                foreach (var item in items)
                {
                    if (sqlEx.Message.Contains(item))
                    {
                        throw new KeyNotFoundException(item.ToLower());
                    }
                }
                if (sqlEx.Message.Contains("CHECK"))
                {
                    throw new ArgumentException();
                }
                throw;
            }
            return new IdDTO(lesson.Id);
        }
        public async Task EditLesson(Guid lessonId, LessonEditModel model)
        {
            var lesson = _context.Lessons.Find(lessonId);
            if (lesson == null)
            {
                throw new KeyNotFoundException();
            }
            _context.Entry(lesson).Collection(l => l.Groups).Load();

            CheckForOverlay(
                model.StartDate ?? lesson.StartDate,
                model.EndDate ?? lesson.EndDate,
                model.DayOfWeek ?? lesson.DayOfWeek,
                model.Timeslot ?? lesson.Timeslot,
                model.AudienceId ?? lesson.AudienceId,
                model.ProfessorId ?? lesson.ProfessorId,
                model.GroupIds ?? lesson.Groups.Select(g => g.Id).ToArray(),
                lessonId);

            lesson.StartDate = model.StartDate ?? lesson.StartDate;
            lesson.EndDate = model.EndDate ?? lesson.EndDate;
            lesson.DayOfWeek = model.DayOfWeek ?? lesson.DayOfWeek;
            lesson.Timeslot = model.Timeslot ?? lesson.Timeslot;
            lesson.AudienceId = model.AudienceId ?? lesson.AudienceId;
            lesson.ProfessorId = model.ProfessorId ?? lesson.ProfessorId;
            lesson.SubjectId = model.SubjectId ?? lesson.SubjectId;
            lesson.LessonType = model.LessonType != null ? (ScheduleDAL.Enums.LessonType)model.LessonType : lesson.LessonType;

            if (model.GroupIds != null)
            {
                _context.LessonGroups.RemoveRange(
                    _context.LessonGroups.Where(lg => lg.LessonId == lesson.Id));
                await _context.AddRangeAsync(model.GroupIds.Select(id => new LessonGroup
                {
                    LessonId = lesson.Id,
                    GroupId = id
                }));
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
                var items = new string[] { "Professor", "Audience", "Subject", "Group" };
                foreach (var item in items)
                {
                    if (sqlEx.Message.Contains(item))
                    {
                        throw new KeyNotFoundException(item.ToLower());
                    }
                }
                if (sqlEx.Message.Contains("CHECK"))
                {
                    throw new ArgumentException();
                }
                throw;
            }
        }
        public async Task DeleteLesson(Guid lessonId)
        {
            var lesson = _context.Lessons.Find(lessonId);
            if(lesson == null)
            {
                throw new KeyNotFoundException();
            }
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }
        private void CheckForOverlay(
            DateTime startDate,
            DateTime endDate,
            DayOfWeek dayOfWeek,
            uint timeslot,
            Guid audienceId,
            Guid professorId,
            Guid[] groupIds,
            Guid? lessonId = null)
        {
            var matchingLessons = _context.Lessons
                .Where(l =>
                    l.StartDate <= endDate &&
                    startDate <= l.EndDate &&
                    l.DayOfWeek == dayOfWeek &&
                    l.Timeslot == timeslot &&
                    l.Id != lessonId)
                .Include(l => l.Groups)
                .ToList();

            var sameAud = matchingLessons.FirstOrDefault(l => l.AudienceId == audienceId);
            if (sameAud != null)
            {
                throw new DataConflictException(ScheduleItems.Audience, sameAud.Id);
            }

            var sameProf = matchingLessons.FirstOrDefault(l => l.ProfessorId == professorId);
            if (sameProf != null)
            {
                throw new DataConflictException(ScheduleItems.Professor, sameProf.Id);
            }

            foreach (var groupId in groupIds)
            {
                var sameGroup = matchingLessons.FirstOrDefault(l =>
                    l.Groups.Select(g => g.Id).Contains(groupId));
                if (sameGroup != null)
                {
                    throw new DataConflictException(ScheduleItems.Group, sameGroup.Id);
                }
            }

            /*var matchingBookedLessons = _context.BookedLessons
                .Where(b =>
                    (b.Status == ScheduleDAL.Enums.BookStatus.New || b.Status == ScheduleDAL.Enums.BookStatus.Approved) &&
                    b.Date <= endDate &&
                    startDate <= b.Date &&
                    b.Date.DayOfWeek == dayOfWeek &&
                    b.Timeslot == timeslot &&
                    b.AudienceId == audienceId)
                .ToList();

            foreach(var booked in matchingBookedLessons)
            {
                booked.Status = ScheduleDAL.Enums.BookStatus.Canceled;
            }*/
        }
    }
}
