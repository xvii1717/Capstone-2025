using Microsoft.EntityFrameworkCore;
using ProductivityApp.Data;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace ProductivityApp.Services
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync()
        {
            using var context = new ProductivityDbContext();
            
            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();
                
                // Run any pending migrations
                if (context.Database.GetPendingMigrations().Any())
                {
                    await context.Database.MigrateAsync();
                }
                
                Console.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                throw;
            }
        }
        
        public static async Task SeedDataAsync()
        {
            using var context = new ProductivityDbContext();
            
            // Add any seed data here if needed
            // For now, we'll keep it empty since users will register themselves
            
            await context.SaveChangesAsync();
        }
    }
}
