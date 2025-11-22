using AutoMapper;
using ToDo.Api.Dtos;
using ToDo.Api.Models;
using ToDo.Api.Repositories;

namespace ToDo.Api.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _repository;
    private readonly IMapper _mapper;

    public TaskItemService(ITaskItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Returns all tasks currently stored in the system.
    /// </summary>
    public async Task<IEnumerable<TaskItemDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TaskItemDto>>(items);
    }

    /// <summary>
    /// Returns a single task by its Id, or null if it doesn't exist.
    /// </summary>
    public async Task<TaskItemDto?> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item is null ? null : _mapper.Map<TaskItemDto>(item);
    }

    /// <summary>
    /// Creates a new task and assigns the creation timestamp.
    /// </summary>
    public async Task<TaskItemDto> CreateAsync(CreateTaskItemRequest request)
    {
        ValidateTitle(request.Title);

        var taskItem = _mapper.Map<TaskItem>(request);
        taskItem.Title = taskItem.Title.Trim();
        taskItem.IsComplete = false;
        taskItem.CreatedAt = DateTime.UtcNow;

        var created = await _repository.AddAsync(taskItem);
        return _mapper.Map<TaskItemDto>(created);
    }

    /// <summary>
    /// Updates an existing task.  
    /// Returns null if the task does not exist.
    /// </summary>
    public async Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskItemRequest request)
    {
        ValidateTitle(request.Title);

        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            return null;
        }

        _mapper.Map(request, existing);
        existing.Title = existing.Title.Trim();

        var updated = await _repository.UpdateAsync(existing);
        return updated is null ? null : _mapper.Map<TaskItemDto>(updated);
    }

    /// <summary>
    /// Deletes a task by its Id.
    /// </summary>
    public Task<bool> DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Ensures the task title is valid before creating or updating.
    /// </summary>
    private static void ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }
    }
}
