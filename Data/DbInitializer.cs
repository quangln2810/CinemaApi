using CinemaApi.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Data
{
    public class DbInitializer
    {
        private CinemaContext context; private UserManager<User> userManager; private RoleManager<Role> roleManager;
        public DbInitializer(CinemaContext _context, UserManager<User> _userManager, RoleManager<Role> _roleManager)
        {
            context = _context;
            userManager = _userManager;
            roleManager = _roleManager;
        }
        public async Task Initialize()
        {
            context.Database.EnsureCreated();

            Role[] roles = new Role[]
            {
                new Role
                {
                    Name = "Administrator"
                },
                new Role
                {
                    Name = "Manager"
                },
                new Role
                {
                    Name = "Cashier"
                },
                new Role
                {
                    Name = "User"
                }
            };
            if (!context.Roles.Any())
            {
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
            }
            User[] users = new User[]
            {
                new User
                {
                    Name = "Admin",
                    Email = "admin@mail.com"
                },
                new User
                {
                    Name = "Manager",
                    Email = "manager@mail.com"
                },
                new User
                {
                    Name = "Quang",
                    Email = "quang@mail.com"
                },
                new User
                {
                    Name = "Nguyen",
                    Email = "nguyen@mail.com"
                }
            };
            if (!context.Users.Any())
            {
                foreach (var user in users)
                {
                    user.UserName = user.Email;
                    var result = await userManager.CreateAsync(user, "111111");
                    if (result.Succeeded)
                    {
                        if (user.Name == "Manager")
                        {
                            await userManager.AddToRoleAsync(user, "Manager");
                        } else
                        {
                            await userManager.AddToRoleAsync(user, "Administrator");
                        }
                    }
                }
            }
            context.SaveChanges();
        }
    }
}
