using ScheduleDAL.Entities;

namespace ScheduleBL.DTO
{
    public class ProfessorDTO
    {
        public ProfessorDTO(Professor professor)
        {
            Id = professor.Id;
            FullName = professor.FullName;
            ShortName = professor.ShortName;
        }
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
    }
}
