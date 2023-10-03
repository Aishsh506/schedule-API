using Microsoft.EntityFrameworkCore;

namespace ScheduleDAL.Entities
{
    [Index(nameof(FullName))]
    public class Professor
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
    }
}
