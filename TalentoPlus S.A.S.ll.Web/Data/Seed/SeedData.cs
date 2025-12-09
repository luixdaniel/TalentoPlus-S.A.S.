using Microsoft.AspNetCore.Identity;

namespace TalentoPlus_S.A.S.ll.Web.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            
            // Seed admin user
            await SeedAdminUserAsync(userManager, logger);
        }
        
        private static async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager, ILogger logger)
        {
            const string adminEmail = "admin@talento.com";
            const string adminPassword = "Admin123!";
            
            try
            {
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                
                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    
                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    
                    if (result.Succeeded)
                    {
                        logger.LogInformation("✅ Admin user created successfully: {Email}", adminEmail);
                        logger.LogInformation("   Default password: {Password}", adminPassword);
                        logger.LogWarning("⚠️  IMPORTANTE: Cambia la contraseña del administrador después del primer inicio de sesión");
                    }
                    else
                    {
                        logger.LogError("❌ Error creating admin user:");
                        foreach (var error in result.Errors)
                        {
                            logger.LogError("   - {ErrorDescription}", error.Description);
                        }
                    }
                }
                else
                {
                    logger.LogInformation("ℹ️  Admin user already exists: {Email}", adminEmail);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during database seeding");
            }
        }
    }
}

