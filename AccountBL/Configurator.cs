using Microsoft.AspNetCore.Builder;
using AccountDAL;
using AccountDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AccountBL
{
    public static class Configurator
    {
        public static void ConfigureAccount(this WebApplicationBuilder builder)
        {
            builder.AddContext();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AccountContext>()
                .AddTokenProvider("App", typeof(DataProtectorTokenProvider<AppUser>));
        }
        public static void AccountDbMigrate(this WebApplication app)
        {
            app.Migrate();
        }
    }
}
