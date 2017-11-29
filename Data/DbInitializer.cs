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
                new Role("Administrator"),
                new Role("Manager"),
                new Role("Cashier"),
                new Role("User")
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
                        }
                        else
                        {
                            await userManager.AddToRoleAsync(user, "Administrator");
                        }
                    }
                }
            }
            Cinema[] cinemas = new Cinema[]
            {
                new Cinema
                {
                    Name = "CGV Indochina Plaza",
                    Address = "Tầng 4, Indochina Plaza, 239-241 Xuân Thủy, Hà Nội",
                    Rooms = new List<Room>
                    {
                        new Room("Phòng 1"),
                        new Room("Phòng 2"),
                        new Room("Phòng 3")
                    }
                },
                new Cinema
                {
                    Name = "CGV Mipec Tower",
                    Address = "MIPEC Tower, 229 Tây Sơn, Ngã Tư Sở, Đống Đa, Hà Nội",
                    Rooms = new List<Room>
                    {
                        new Room("Phòng 1"),
                        new Room("Phòng 2"),
                        new Room("Phòng 3")
                    }
                }
            };
            if (!context.Cinemas.Any())
            {
                context.Cinemas.AddRange(cinemas);
            }
            Movie[] movies = new Movie[]
            {
                new Movie
                {
                    Name = "The Avenger",
                    Poster = "http://www.ldssmile.com/wp-content/uploads/2014/09/the-avengers-1235-wallmages.jpg",
                    Trailer = "https://www.youtube.com/watch?v=eOrNdBpGMv8&t=2s",
                    Genre = Genres.Action,
                    Duration = TimeSpan.Parse("02:10:00")
                },
                new Movie
                {
                    Name = "Thor 3",
                    Poster = "http://2.bp.blogspot.com/-3xR9_6goCGM/WgaL_G-RbQI/AAAAAAAEjXc/EPzWEe6Kbhw_rkin6N5X84wg3Eco2rEZgCHMYCw/s0/t%E1%BA%A3i_xu%E1%BB%91ng.jpg",
                    Trailer = "https://www.youtube.com/watch?v=F7ayGFHGqeQ",
                    Genre = Genres.Action,
                    Duration = TimeSpan.Parse("02:53:00")
                },
            };
            if (!context.Movies.Any())
            {
                context.Movies.AddRange(movies);
            }
            context.SaveChanges();
        }
    }
}
