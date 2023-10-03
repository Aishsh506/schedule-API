using Microsoft.EntityFrameworkCore;

namespace ScheduleDAL.Entities
{
    [Index(nameof(Name))]
    public class Audience
    {
        public Guid Id { get; set; }
        public Building Building { get; set; }
        public string Name { get; set; }
    }
}
