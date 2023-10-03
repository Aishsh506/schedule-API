using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using ScheduleDAL;

namespace ScheduleBL
{
    public static class Configurator
    {
        public static void ConfigureScheduleServices(this WebApplicationBuilder builder)
        {
            builder.AddContext();
        }
        public static void ScheduleDbMigrate(this WebApplication app)
        {
            app.Migrate();
        }
    }
}