namespace ToDo.Api.Dtos;

public class TaskItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } = null!;
}