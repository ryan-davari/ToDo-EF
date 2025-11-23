using ToDo.DAL.Models;

namespace ToDo.DAL.Repositories;

public interface ITaskItemRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync(string userId);
    Task<TaskItem?> GetByIdAsync(int id, string userId);
    Task<TaskItem> AddAsync(TaskItem taskItem);
    Task<TaskItem?> UpdateAsync(TaskItem taskItem, string userId);
    Task<bool> DeleteAsync(int id, string userId);
}
