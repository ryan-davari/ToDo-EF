using System.Collections.Concurrent;
using System.Threading;
using ToDo.DAL.Models;

namespace ToDo.Api.Repositories;

/// <summary>
/// Simple in-memory repository used to store and manage task items.
/// </summary>
public class TaskItemRepository : ITaskItemRepository
{
    private readonly ConcurrentDictionary<int, TaskItem> _taskItems = new();
    private int _nextId = 0;

    /// <summary>
    /// Returns all task items currently saved.
    /// </summary>
    public Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        var items = _taskItems.Values
            .OrderBy(t => t.CreatedAt)
            .AsEnumerable();

        return Task.FromResult(items);
    }

    /// <summary>
    /// Returns a task item by its Id, or null if it doesn't exist.
    /// </summary>
    public Task<TaskItem?> GetByIdAsync(int id)
    {
        _taskItems.TryGetValue(id, out var taskItem);
        return Task.FromResult(taskItem);
    }

    /// <summary>
    /// Adds a new task item and assigns a unique Id.
    /// </summary>
    public Task<TaskItem> AddAsync(TaskItem taskItem)
    {
        var id = Interlocked.Increment(ref _nextId);
        taskItem.Id = id;

        if (taskItem.CreatedAt == default)
        {
            taskItem.CreatedAt = DateTime.UtcNow;
        }

        _taskItems.TryAdd(taskItem.Id, taskItem);
        return Task.FromResult(taskItem);
    }

    /// <summary>
    /// Updates an existing task if it exists.
    /// </summary>
    public Task<TaskItem?> UpdateAsync(TaskItem taskItem)
    {
        if (!_taskItems.ContainsKey(taskItem.Id))
        {
            return Task.FromResult<TaskItem?>(null);
        }

        _taskItems[taskItem.Id] = taskItem;
        return Task.FromResult<TaskItem?>(taskItem);
    }

    /// <summary>
    /// Deletes a task by its Id.
    /// </summary>
    public Task<bool> DeleteAsync(int id)
    {
        var removed = _taskItems.TryRemove(id, out _);
        return Task.FromResult(removed);
    }
}
