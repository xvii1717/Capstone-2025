using ProductivityApp.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ProductivityApp.Services
{
    public interface IDataService
    {
        // Calendar Events
        Task<List<CalendarEvent>> GetCalendarEventsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<CalendarEvent?> GetCalendarEventAsync(int eventId);
        Task<CalendarEvent> CreateCalendarEventAsync(CalendarEvent calendarEvent);
        Task<CalendarEvent?> UpdateCalendarEventAsync(CalendarEvent calendarEvent);
        Task<bool> DeleteCalendarEventAsync(int eventId);

        // Todo Items
        Task<List<TodoItem>> GetTodoItemsAsync(int userId, bool includeCompleted = false);
        Task<TodoItem?> GetTodoItemAsync(int todoId);
        Task<TodoItem> CreateTodoItemAsync(TodoItem todoItem);
        Task<TodoItem?> UpdateTodoItemAsync(TodoItem todoItem);
        Task<bool> DeleteTodoItemAsync(int todoId);
        Task<bool> MarkTodoCompletedAsync(int todoId);

        // Data synchronization
        Task SyncTodoWithCalendarAsync(int todoId, DateTime? dueDate);
        Task RemoveTodoCalendarSyncAsync(int todoId);

        // Analytics and reporting
        Task<Dictionary<string, int>> GetProductivityStatsAsync(int userId, DateTime startDate, DateTime endDate);
        Task<List<TodoItem>> GetOverdueTasksAsync(int userId);
        Task<List<CalendarEvent>> GetUpcomingEventsAsync(int userId, int days = 7);
    }
}
