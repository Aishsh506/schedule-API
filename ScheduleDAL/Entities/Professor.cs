using Microsoft.EntityFrameworkCore;

namespace ScheduleDAL.Entities
{
    [Index(nameof(FullName))]
    [Index(nameof(AccountId), IsUnique = true)]
    public class Professor
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public Guid? AccountId { get; set; }
    }
}
