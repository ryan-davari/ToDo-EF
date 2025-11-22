using ToDo.DAL.Models;

namespace ToDo.DAL.Repositories;

public interface ITaskItemRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> AddAsync(TaskItem taskItem);
    Task<TaskItem?> UpdateAsync(TaskItem taskItem);
    Task<bool> DeleteAsync(int id);
}
