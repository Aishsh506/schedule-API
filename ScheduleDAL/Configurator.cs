using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ScheduleDAL
{
    public static class Configurator
    {
        public static void AddContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ScheduleContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ScheduleDb")));
        }
        public static void Migrate(this WebApplication app)
        {
            var Scope = app.Services.CreateScope();
            var dbContext = Scope.ServiceProvider.GetRequiredService<ScheduleContext>();
            dbContext.Database.Migrate();
        }
    }
}
