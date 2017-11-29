using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CinemaApi.Models;

namespace CinemaApi.Models
{
    public partial class CinemaContext : IdentityDbContext<User, Role, long>
    {
        public CinemaContext(DbContextOptions<CinemaContext> options)
            : base(options)
        { }

        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Cinema>()
                .HasIndex(c => c.Name)
                .IsUnique();
            builder.Entity<Movie>()
                .HasIndex(m => m.Name)
                .IsUnique();
            builder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();
            builder.Entity<Room>()
                .HasIndex(r => new { r.Name, r.CinemaId })
                .IsUnique();
            builder.Entity<Ticket>()
                .HasIndex(ticket => new { ticket.IdSchedule, ticket.Seat })
                .IsUnique();
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}