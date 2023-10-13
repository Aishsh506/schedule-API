using ScheduleDAL.Entities;
using Common.DTO;

namespace ScheduleBL
{
    public static class EntityToDTOMapper
    {
        public static LessonDTO ToDTO(this Lesson lesson)
        {
            return new LessonDTO
            {
                Id = lesson.Id,
                DayOfWeek = lesson.DayOfWeek,
                StartDate = lesson.StartDate,
                EndDate = lesson.EndDate,
                Timeslot = lesson.Timeslot,
                LessonType = (Common.Enums.LessonType)lesson.LessonType,
                Professor = lesson.Professor.ToDTO(),
                Audience = lesson.Audience.ToDTO(),
                Subject = lesson.Subject.ToDTO(),
                Groups = lesson.Groups.Select(g => g.ToDTO()).ToArray()
            };
        }
        public static ProfessorDTO ToDTO(this Professor professor)
        {
            return new ProfessorDTO
            {
                Id = professor.Id,
                FullName = professor.FullName,
                ShortName = professor.ShortName
            };
        }
        public static AudienceDTO ToDTO(this Audience audience)
        {
            return new AudienceDTO
            {
                Id = audience.Id,
                Name = audience.Name
            };
        }
        public static SubjectDTO ToDTO(this Subject subject)
        {
            return new SubjectDTO
            {
                Id = subject.Id,
                Name = subject.Name
            };
        }
        public static GroupDTO ToDTO(this Group group)
        {
            return new GroupDTO
            {
                Id = group.Id,
                Name = group.Name
            };
        }
    }
}
