using ToDo.DAL.Models;

namespace ToDo.Api.Repositories;

public interface ITaskItemRepository
{
    /// <summary>
    /// Returns all task items currently stored.
    /// </summary>
    /// <returns>
    /// A collection of task items ordered by creation time.
    /// </returns>
    Task<IEnumerable<TaskItem>> GetAllAsync();

    /// <summary>
    /// Retrieves a task item by its Id.
    /// </summary>
    /// <param name="id">The Id of the task to find.</param>
    /// <returns>
    /// The matching task item, or null if no item exists with the given Id.
    /// </returns>
    Task<TaskItem?> GetByIdAsync(int id);

    /// <summary>
    /// Adds a new task item to the store.
    /// The repository is responsible for generating and assigning the Id.
    /// </summary>
    /// <param name="taskItem">The task item to add.</param>
    /// <returns>
    /// The task item after it has been added and assigned an Id.
    /// </returns>
    Task<TaskItem> AddAsync(TaskItem taskItem);

    /// <summary>
    /// Updates an existing task item.
    /// </summary>
    /// <param name="taskItem">The updated task item model.</param>
    /// <returns>
    /// The updated task item if the update succeeded,
    /// or null if no task exists with the same Id.
    /// </returns>
    Task<TaskItem?> UpdateAsync(TaskItem taskItem);

    /// <summary>
    /// Removes a task item by its Id.
    /// </summary>
    /// <param name="id">The Id of the task item to delete.</param>
    /// <returns>
    /// True if the item was found and deleted, otherwise false.
    /// </returns>
    Task<bool> DeleteAsync(int id);
}
