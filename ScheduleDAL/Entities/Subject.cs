using Microsoft.EntityFrameworkCore;

namespace ScheduleDAL.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Subject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
