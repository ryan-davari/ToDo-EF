namespace ToDo.Api.Dtos;

public class UpdateTaskItemRequest
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsComplete { get; set; }
}
