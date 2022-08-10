using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LibraryCatalog
{
  public class Program
  {
    public async static Task Main(string[] args)
    {
      var host = new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseIISIntegration()
        .UseStartup<Startup>()
        .Build();

      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("app");
        try
        {
          var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
          var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
          await Seeds.DefaultRoles.SeedAsync(userManager, roleManager);
          await Seeds.DefaultUsers.SeedBasicUserAsync(userManager, roleManager);
          await Seeds.DefaultUsers.SeedSuperAdminAsync(userManager, roleManager);
          logger.LogInformation("Finished Seeding Default Data");
          logger.LogInformation("Application Starting");
        }
        catch (Exception ex)
        {
          logger.LogWarning(ex, "An error occurred seeding the DB");
        }
      }

      host.Run();
    }
  }
}