using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Api.Dtos;
using ToDo.Api.Services;

namespace ToDo.Api.Controllers
{
    /// <summary>
    /// Exposes CRUD endpoints for managing task items.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemController : ControllerBase
    {
        private readonly ITaskItemService _taskItemService;

        public TaskItemController(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        /// <summary>
        /// Returns all tasks currently stored in the system.
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAll()
        {
            var items = await _taskItemService.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Returns a single task by its Id.
        /// </summary>
        [Authorize]
        [HttpGet("{id:int}", Name = "GetTaskItemById")]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskItemDto>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id");
            }

            var item = await _taskItemService.GetByIdAsync(id);
            if (item is null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        /// <summary>
        /// Creates a new task.
        /// </summary>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskItemDto>> Create([FromBody] CreateTaskItemRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var created = await _taskItemService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        /// <summary>
        /// Updates an existing task.
        /// Returns 200 with DTO if updated, 204 if not found, 400 if invalid input.
        /// </summary>
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskItemRequest request)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = await _taskItemService.UpdateAsync(id, request);

            // Spec: "TaskItemDto or 204 NoContent"
            if (updated is null)
            {
                return NoContent();
            }

            return Ok(updated);
        }

        /// <summary>
        /// Deletes a task by its Id.
        /// </summary>
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id");
            }

            var deleted = await _taskItemService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
