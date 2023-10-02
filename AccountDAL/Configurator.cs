using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountDAL
{
    public static class Configurator
    {
        public static void AddContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AccountContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AccountDb")));
        }
        public static void Migrate(this WebApplication app)
        {
            var Scope = app.Services.CreateScope();
            var dbContext = Scope.ServiceProvider.GetRequiredService<AccountContext>();
            dbContext.Database.Migrate();
        }
    }
}
