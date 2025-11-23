using AutoMapper;
using ToDo.Api.Dtos;
using ToDo.DAL.Models;
using ToDo.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ToDo.Api.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TaskItemService(ITaskItemRepository repository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Returns all tasks currently stored in the system.
    /// </summary>
    public async Task<IEnumerable<TaskItemDto>> GetAllAsync()
    {
        var userId = GetCurrentUserId();
        var items = await _repository.GetAllAsync(userId);
        return _mapper.Map<IEnumerable<TaskItemDto>>(items);
    }

    /// <summary>
    /// Returns a single task by its Id, or null if it doesn't exist.
    /// </summary>
    public async Task<TaskItemDto?> GetByIdAsync(int id)
    {
        var userId = GetCurrentUserId();
        var item = await _repository.GetByIdAsync(id, userId);
        return item is null ? null : _mapper.Map<TaskItemDto>(item);
    }

    /// <summary>
    /// Creates a new task and assigns the creation timestamp.
    /// </summary>
    public async Task<TaskItemDto> CreateAsync(CreateTaskItemRequest request)
    {
        ValidateTitle(request.Title);

        var userId = GetCurrentUserId();

        var taskItem = _mapper.Map<TaskItem>(request);
        taskItem.Title = taskItem.Title.Trim();
        taskItem.IsComplete = false;
        taskItem.CreatedAt = DateTime.UtcNow;
        taskItem.UserId = userId;

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

        var userId = GetCurrentUserId();
        var existing = await _repository.GetByIdAsync(id, userId);
        if (existing is null)
        {
            return null;
        }

        _mapper.Map(request, existing);
        existing.Title = existing.Title.Trim();

        var updated = await _repository.UpdateAsync(existing, userId);
        return updated is null ? null : _mapper.Map<TaskItemDto>(updated);
    }

    /// <summary>
    /// Deletes a task by its Id.
    /// </summary>
    public Task<bool> DeleteAsync(int id)
    {
        var userId = GetCurrentUserId();
        return _repository.DeleteAsync(id, userId);
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

    private string GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException("User context is missing.");
        }

        return userId;
    }
}
