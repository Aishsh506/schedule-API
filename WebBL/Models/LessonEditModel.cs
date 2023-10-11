using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace WebBL.Models
{
    public class LessonEditModel
    {
        [Range(0,5)] public DayOfWeek? DayOfWeek { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Range(1, 7)] public uint? Timeslot { get; set; }
        public Guid? AudienceId { get; set; }
        public LessonType? LessonType { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? ProfessorId { get; set; }
        [MinLength(1)]public Guid[]? GroupIds { get; set; }
    }
}
