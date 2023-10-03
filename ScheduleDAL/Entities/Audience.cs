using Microsoft.EntityFrameworkCore;

namespace ScheduleDAL.Entities
{
    [Index(nameof(Name), nameof(BuildingId), IsUnique = true)]
    public class Audience
    {
        public Guid Id { get; set; }
        public Guid BuildingId { get; set; }
        public Building Building { get; set; }
        public string Name { get; set; }
    }
}
