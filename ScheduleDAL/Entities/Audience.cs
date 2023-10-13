using Microsoft.EntityFrameworkCore;

namespace ScheduleDAL.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Audience
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
