using ScheduleDAL.Entities;

namespace ScheduleBL.DTO
{
    public class SubjectDTO
    {
        public SubjectDTO(Subject subject)
        {
            Id = subject.Id;
            Name = subject.Name;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
