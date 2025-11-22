namespace ToDo.Api.Dtos;

public class CreateTaskItemRequest
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}
