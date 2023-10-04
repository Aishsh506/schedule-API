using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AccountDAL.Entities;

namespace AccountDAL
{
    public class AccountContext : IdentityDbContext<AppUser>
    {
        public AccountContext(DbContextOptions<AccountContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var roles = new List<IdentityRole>();
            foreach (var name in new[] { "Editor", "Student", "Professor" })
            {
                roles.Add(new IdentityRole { Name = name, NormalizedName = name.ToUpper() });
            }
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}