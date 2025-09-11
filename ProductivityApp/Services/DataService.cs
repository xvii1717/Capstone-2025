using Microsoft.EntityFrameworkCore;
using ProductivityApp.Data;
using ProductivityApp.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductivityApp.Services
{
    public class DataService : IDataService
    {
        private readonly ProductivityDbContext _context;

        public DataService(ProductivityDbContext context)
        {
            _context = context;
        }

        // Calendar Events
        public async Task<List<CalendarEvent>> GetCalendarEventsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.CalendarEvents
                .Where(ce => ce.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(ce => ce.EventDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(ce => ce.EventDate <= endDate.Value);

            return await query
                .Include(ce => ce.RelatedTodo)
                .OrderBy(ce => ce.EventDate)
                .ThenBy(ce => ce.StartTime)
                .ToListAsync();
        }

        public async Task<CalendarEvent?> GetCalendarEventAsync(int eventId)
        {
            return await _context.CalendarEvents
                .Include(ce => ce.RelatedTodo)
                .FirstOrDefaultAsync(ce => ce.Id == eventId);
        }

        public async Task<CalendarEvent> CreateCalendarEventAsync(CalendarEvent calendarEvent)
        {
            calendarEvent.CreatedAt = DateTime.UtcNow;
            _context.CalendarEvents.Add(calendarEvent);
            await _context.SaveChangesAsync();
            return calendarEvent;
        }

        public async Task<CalendarEvent?> UpdateCalendarEventAsync(CalendarEvent calendarEvent)
        {
            var existing = await _context.CalendarEvents.FindAsync(calendarEvent.Id);
            if (existing == null) return null;

            existing.Title = calendarEvent.Title;
            existing.Description = calendarEvent.Description;
            existing.EventDate = calendarEvent.EventDate;
            existing.StartTime = calendarEvent.StartTime;
            existing.EndTime = calendarEvent.EndTime;
            existing.Category = calendarEvent.Category;
            existing.Priority = calendarEvent.Priority;
            existing.IsCompleted = calendarEvent.IsCompleted;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteCalendarEventAsync(int eventId)
        {
            var calendarEvent = await _context.CalendarEvents.FindAsync(eventId);
            if (calendarEvent == null) return false;

            _context.CalendarEvents.Remove(calendarEvent);
            await _context.SaveChangesAsync();
            return true;
        }

        // Todo Items
        public async Task<List<TodoItem>> GetTodoItemsAsync(int userId, bool includeCompleted = false)
        {
            var query = _context.TodoItems
                .Where(ti => ti.UserId == userId);

            if (!includeCompleted)
                query = query.Where(ti => !ti.IsCompleted);

            return await query
                .Include(ti => ti.RelatedEvent)
                .OrderBy(ti => ti.DueDate)
                .ThenBy(ti => ti.Priority)
                .ToListAsync();
        }

        public async Task<TodoItem?> GetTodoItemAsync(int todoId)
        {
            return await _context.TodoItems
                .Include(ti => ti.RelatedEvent)
                .FirstOrDefaultAsync(ti => ti.Id == todoId);
        }

        public async Task<TodoItem> CreateTodoItemAsync(TodoItem todoItem)
        {
            todoItem.CreatedAt = DateTime.UtcNow;
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return todoItem;
        }

        public async Task<TodoItem?> UpdateTodoItemAsync(TodoItem todoItem)
        {
            var existing = await _context.TodoItems.FindAsync(todoItem.Id);
            if (existing == null) return null;

            existing.Title = todoItem.Title;
            existing.Description = todoItem.Description;
            existing.DueDate = todoItem.DueDate;
            existing.Priority = todoItem.Priority;
            existing.Status = todoItem.Status;
            existing.Category = todoItem.Category;
            existing.IsCompleted = todoItem.IsCompleted;
            existing.EstimatedMinutes = todoItem.EstimatedMinutes;
            existing.ActualMinutes = todoItem.ActualMinutes;
            existing.UpdatedAt = DateTime.UtcNow;

            if (todoItem.IsCompleted && existing.CompletedAt == null)
                existing.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteTodoItemAsync(int todoId)
        {
            var todoItem = await _context.TodoItems.FindAsync(todoId);
            if (todoItem == null) return false;

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkTodoCompletedAsync(int todoId)
        {
            var todoItem = await _context.TodoItems.FindAsync(todoId);
            if (todoItem == null) return false;

            todoItem.IsCompleted = true;
            todoItem.CompletedAt = DateTime.UtcNow;
            todoItem.Status = "Completed";
            todoItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // Data synchronization
        public async Task SyncTodoWithCalendarAsync(int todoId, DateTime? dueDate)
        {
            var todoItem = await _context.TodoItems
                .Include(ti => ti.RelatedEvent)
                .FirstOrDefaultAsync(ti => ti.Id == todoId);

            if (todoItem == null || !dueDate.HasValue) return;

            if (todoItem.RelatedEvent != null)
            {
                // Update existing calendar event
                todoItem.RelatedEvent.Title = $"To-Do: {todoItem.Title}";
                todoItem.RelatedEvent.EventDate = dueDate.Value.Date;
                todoItem.RelatedEvent.Description = todoItem.Description;
                todoItem.RelatedEvent.Priority = todoItem.Priority;
                todoItem.RelatedEvent.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new calendar event
                var calendarEvent = new CalendarEvent
                {
                    UserId = todoItem.UserId,
                    Title = $"To-Do: {todoItem.Title}",
                    Description = todoItem.Description,
                    EventDate = dueDate.Value.Date,
                    Priority = todoItem.Priority,
                    Category = "Todo",
                    CreatedAt = DateTime.UtcNow
                };

                _context.CalendarEvents.Add(calendarEvent);
                await _context.SaveChangesAsync();

                todoItem.RelatedEventId = calendarEvent.Id;
            }

            todoItem.DueDate = dueDate;
            todoItem.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTodoCalendarSyncAsync(int todoId)
        {
            var todoItem = await _context.TodoItems
                .Include(ti => ti.RelatedEvent)
                .FirstOrDefaultAsync(ti => ti.Id == todoId);

            if (todoItem?.RelatedEvent != null)
            {
                _context.CalendarEvents.Remove(todoItem.RelatedEvent);
                todoItem.RelatedEventId = null;
                await _context.SaveChangesAsync();
            }
        }

        // Analytics and reporting
        public async Task<Dictionary<string, int>> GetProductivityStatsAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var completedTodos = await _context.TodoItems
                .Where(ti => ti.UserId == userId 
                    && ti.IsCompleted 
                    && ti.CompletedAt >= startDate 
                    && ti.CompletedAt <= endDate)
                .CountAsync();

            var totalTodos = await _context.TodoItems
                .Where(ti => ti.UserId == userId 
                    && ti.CreatedAt >= startDate 
                    && ti.CreatedAt <= endDate)
                .CountAsync();

            var completedEvents = await _context.CalendarEvents
                .Where(ce => ce.UserId == userId 
                    && ce.IsCompleted 
                    && ce.EventDate >= startDate 
                    && ce.EventDate <= endDate)
                .CountAsync();

            var totalEvents = await _context.CalendarEvents
                .Where(ce => ce.UserId == userId 
                    && ce.EventDate >= startDate 
                    && ce.EventDate <= endDate)
                .CountAsync();

            return new Dictionary<string, int>
            {
                ["CompletedTodos"] = completedTodos,
                ["TotalTodos"] = totalTodos,
                ["CompletedEvents"] = completedEvents,
                ["TotalEvents"] = totalEvents,
                ["CompletionRate"] = totalTodos > 0 ? (completedTodos * 100 / totalTodos) : 0
            };
        }

        public async Task<List<TodoItem>> GetOverdueTasksAsync(int userId)
        {
            return await _context.TodoItems
                .Where(ti => ti.UserId == userId 
                    && !ti.IsCompleted 
                    && ti.DueDate.HasValue 
                    && ti.DueDate < DateTime.Today)
                .Include(ti => ti.RelatedEvent)
                .OrderBy(ti => ti.DueDate)
                .ToListAsync();
        }

        public async Task<List<CalendarEvent>> GetUpcomingEventsAsync(int userId, int days = 7)
        {
            var endDate = DateTime.Today.AddDays(days);
            
            return await _context.CalendarEvents
                .Where(ce => ce.UserId == userId 
                    && ce.EventDate >= DateTime.Today 
                    && ce.EventDate <= endDate
                    && !ce.IsCompleted)
                .Include(ce => ce.RelatedTodo)
                .OrderBy(ce => ce.EventDate)
                .ThenBy(ce => ce.StartTime)
                .ToListAsync();
        }
    }
}
