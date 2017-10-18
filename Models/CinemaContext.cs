using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CinemaApi.Models
{
    public partial class CinemaContext : DbContext
    {
        public CinemaContext(DbContextOptions<CinemaContext> options)
            : base(options)
        { }

        public virtual DbSet<Cinema> Cinemas { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}