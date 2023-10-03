using ScheduleDAL.Entities;

namespace ScheduleBL.DTO
{
    public class AudienceDTO
    {
        public AudienceDTO(Audience audience)
        {
            Id = audience.Id;
            Name = audience.Name;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
