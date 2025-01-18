using Mover.Core.Entities;
using Mover.Logging;
using Microsoft.AspNetCore.Identity;
using Mover.Core.Entities.UserManagement;
using Mover.Core.Enums.Roles;
using Mover.Infrastructure.Context;

namespace Mover.SeedData
{
    public static class ApplicationDbInitializer
    {
        static string _SUPERADMINROLE = RolesEnum.Admin.ToString();
        static string _SUPERADMINUSERNAME = "admin";
        static string _CUSTOMERROLE = RolesEnum.Customer.ToString();
        static string _EMPLOYEEROLE = RolesEnum.Employee.ToString();

        static string _roleId = Guid.NewGuid().ToString();
        static string _customerRoleId = Guid.NewGuid().ToString();
        static string email = "shuvamupreti@gmail.com";
        static string phoneNumber = "1234567890";

        public static async Task SeedUsers(MoverContext context, UserManager<ApplicationUser> userManager)
        {
            var role = context.AspNetRoles.Where(a => a.Name == _SUPERADMINROLE).SingleOrDefault();

            _roleId = role.Id;
            if (!context.AspNetUsers.Any(a => a.UserName == _SUPERADMINUSERNAME))
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = _SUPERADMINUSERNAME,
                    Email = email,
                    PhoneNumber = phoneNumber,
                };

                IdentityResult result = await userManager.CreateAsync(user, "Pass@word1");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role.Name);
                }

                context.UserDetails.Add(new UserDetail()
                {
                    FullName = "Administrator",
                    AspUserId = user.Id,
                    Department = _SUPERADMINUSERNAME,
                    DateOfJoin = DateTime.Now
                });
                await context.SaveChangesAsync();
            }
        }
        public static async Task<IHost> SeedData(this IHost host)
        {
            try
            {
                using (var scope = host.Services.CreateScope())
                {

                    var context = scope.ServiceProvider.GetService<IdentityUserDbContext>();

                    var moverContext = scope.ServiceProvider.GetService<MoverContext>();
                    moverContext.Database.EnsureCreated();
                    var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                    // Check if roles exist before adding them
                    AddRoleIfNotExists(context, _SUPERADMINROLE, _customerRoleId);
                    AddRoleIfNotExists(context, _CUSTOMERROLE);
                    AddRoleIfNotExists(context, _EMPLOYEEROLE);

                    context.SaveChanges();

                    await SeedUsers(moverContext, userManager);

                }
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);

            }
            return host;

        }

        private static void AddRoleIfNotExists(IdentityUserDbContext context, string roleName, string roleId = null)
        {
            if (!context.Roles.Any(r => r.Name == roleName))
            {
                var role = new IdentityRole()
                {
                    Id = roleId ?? Guid.NewGuid().ToString(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };
                context.Roles.Add(role);
            }
        }
    }
}
