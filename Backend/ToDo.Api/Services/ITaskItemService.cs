using ToDo.Api.Dtos;

namespace ToDo.Api.Services;

public interface ITaskItemService
{
    /// <summary>
    /// Gets the full list of tasks from the system.
    /// </summary>
    /// <returns>
    /// A collection of task items in DTO form.
    /// </returns>
    Task<IEnumerable<TaskItemDto>> GetAllAsync();

    /// <summary>
    /// Retrieves a single task by its Id.
    /// </summary>
    /// <param name="id">The Id of the task to retrieve.</param>
    /// <returns>
    /// The task item if found, otherwise null.
    /// </returns>
    Task<TaskItemDto?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new task using the provided details.
    /// </summary>
    /// <param name="request">The title and description of the new task.</param>
    /// <returns>
    /// The newly created task item.
    /// </returns>
    Task<TaskItemDto> CreateAsync(CreateTaskItemRequest request);

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The Id of the task to update.</param>
    /// <param name="request">The updated values for the task.</param>
    /// <returns>
    /// The updated task item if successful, otherwise null if it doesn't exist.
    /// </returns>
    Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskItemRequest request);

    /// <summary>
    /// Deletes an existing task by its Id.
    /// </summary>
    /// <param name="id">The Id of the task to delete.</param>
    /// <returns>
    /// True if the task was removed, false if not found.
    /// </returns>
    Task<bool> DeleteAsync(int id);
}
