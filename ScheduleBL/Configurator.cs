using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ScheduleDAL;

namespace ScheduleBL
{
    public static class Configurator
    {
        public static void ConfigureScheduleServices(this WebApplicationBuilder builder)
        {
            builder.AddContext();
            builder.Services.AddScoped<IScheduleItemsService, ScheduleItemsService>();
        }
        public static void ScheduleDbMigrate(this WebApplication app)
        {
            app.Migrate();
        }
    }
}