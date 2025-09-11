using Microsoft.EntityFrameworkCore;
using ProductivityApp.Models;
using System;
using System.IO;
using System.Linq;

namespace ProductivityApp.Data
{
    public class ProductivityDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<SecurityLog> SecurityLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "productivity.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            
            // Enable logging in debug mode
            #if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.LogTo(Console.WriteLine);
            #endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
            });

            // CalendarEvent entity configuration
            modelBuilder.Entity<CalendarEvent>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.CalendarEvents)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.EventDate });
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.Priority).HasDefaultValue("Medium");
            });

            // TodoItem entity configuration
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.TodoItems)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.RelatedEvent)
                    .WithOne(p => p.RelatedTodo)
                    .HasForeignKey<TodoItem>(d => d.RelatedEventId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => new { e.UserId, e.Status });
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.Priority).HasDefaultValue("Medium");
                entity.Property(e => e.Status).HasDefaultValue("Pending");
            });

            // UserSession entity configuration
            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.SessionToken).IsUnique();
                entity.HasIndex(e => new { e.UserId, e.IsActive });
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
            });

            // SecurityLog entity configuration
            modelBuilder.Entity<SecurityLog>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.SecurityLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.Timestamp });
                entity.HasIndex(e => e.EventType);
                entity.Property(e => e.Timestamp).HasDefaultValueSql("datetime('now')");
            });
        }
    }
}
