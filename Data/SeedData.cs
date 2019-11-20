using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using southosting.Logic;
using Microsoft.EntityFrameworkCore;
using southosting.Models;
using southosting.Data;
using southosting.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using MySql.Data.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;



namespace southosting.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string userPW)
        {
            using(var context = new SouthostingContext(serviceProvider.GetRequiredService<DbContextOptions<SouthostingContext>>()))
            {
                var adminId = await EnsureUser(serviceProvider, "Admin", "McAdminFace", userPW, "admin@example.com");
                await EnsureRole(serviceProvider, adminId, Constants.AdministratorRole);

                var officerId = await EnsureUser(serviceProvider, "Officer", "McOfficerFace", userPW, "officer@example.com");
                await EnsureRole(serviceProvider, officerId, Constants.AccommodationOfficerRole);

                var landlordId = await EnsureUser(serviceProvider, "Landlord", "McLandlordFace", userPW, "landlord@example.com");
                await EnsureRole(serviceProvider, landlordId, Constants.LandlordRole);

                var studentId = await EnsureUser(serviceProvider, "Student", "McStudentFace", userPW, "student@example.com");
                await EnsureRole(serviceProvider, studentId, Constants.StudentRole);

                SeedDb(context);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string Fname, string Lname, string Pw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<SouthostingUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new SouthostingUser { FirstName = Fname,
                                             LastName = Lname,
                                             UserName = UserName,
                                             Email = UserName };
                await userManager.CreateAsync(user, Pw);
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null) throw new Exception("Role manager is null.");

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<SouthostingUser>>();
            var user = await userManager.FindByIdAsync(uid);
            IR = await userManager.AddToRoleAsync(user, role);
            return IR;
        }

        public static void SeedDb(SouthostingContext context)
        {
            if (context.Advert.Any()) return;
        }
    }
}