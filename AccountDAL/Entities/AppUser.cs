using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountDAL.Entities
{
    [Index(nameof(ProfessorId), IsUnique = true)]
    public class AppUser : IdentityUser
    {
        public Guid? GroupId { get; set; }
        public Guid? ProfessorId { get; set; }
    }
}
