using Exam_System.Models;
using Exam_System.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Exam_System.Services.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;

        public DbInitializer(UserManager<ApplicationUser> userManager ,
            RoleManager<IdentityRole> roleManager,
            AppDbContext db)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _db=db;
        }
        public async Task Initialize()
        {
            //migartions if they aren't applied

            if ((await _db.Database.GetPendingMigrationsAsync()).Any())
                await _db.Database.MigrateAsync();

            //create teacher and student roles if it is not created
            if (!await _roleManager.RoleExistsAsync(StaticDetails.TeacherRole))
            {

                //if role isn't created , then we will create teacher user as well
                await _roleManager.CreateAsync(new IdentityRole(StaticDetails.TeacherRole));
                await _roleManager.CreateAsync(new IdentityRole(StaticDetails.StudentRole));

                var teacher = new ApplicationUser
                {
                    UserName = "Teacher2530",
                    Email = "Teacher2530@gmail.com",
                    PhoneNumber = "01555178340",
                    
                    EmailConfirmed = true     //important when you seed data in db during creation for first time to make the user confirmed
                                              //without it the signInManager.PasswordSignInAsync returns is not allowed
                };

                var result = await _userManager.CreateAsync(teacher, "Teacher2530@");

                if (!result.Succeeded)
                {
                    //  ❌  log or inspect the real reason
                    foreach (var error in result.Errors)
                        Console.WriteLine($"{error.Code}: {error.Description}");
                    return;     // don’t keep going if user isn’t there
                }

                await _userManager.AddToRoleAsync(teacher, StaticDetails.TeacherRole);
                return;
            }
        }

    }
}
