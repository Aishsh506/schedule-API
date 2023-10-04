using Microsoft.EntityFrameworkCore;

namespace ScheduleDAL.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
    }
}
