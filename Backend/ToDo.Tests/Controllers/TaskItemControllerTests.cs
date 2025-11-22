using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDo.Api.Controllers;
using ToDo.Api.Dtos;
using ToDo.Api.Services;
using Xunit;

namespace ToDo.Tests.Controllers
{
    public class TaskItemControllerTests
    {
        private readonly Mock<ITaskItemService> _taskItemServiceMock;
        private readonly TaskItemController _controller;

        public TaskItemControllerTests()
        {
            _taskItemServiceMock = new Mock<ITaskItemService>(MockBehavior.Strict);
            _controller = new TaskItemController(_taskItemServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithItems()
        {
            // Arrange
            var items = new List<TaskItemDto>
            {
                new TaskItemDto { Id = 1, Title = "Task 1", IsComplete = false },
                new TaskItemDto { Id = 2, Title = "Task 2", IsComplete = true }
            };

            _taskItemServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(items);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedItems = Assert.IsAssignableFrom<IEnumerable<TaskItemDto>>(okResult.Value);
            Assert.Equal(2, returnedItems.Count());

            _taskItemServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsBadRequestValidationProblem()
        {
            // Arrange
            var request = new CreateTaskItemRequest { Title = string.Empty };
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.Create(request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var details = Assert.IsType<ValidationProblemDetails>(objectResult.Value);

            Assert.Contains("Title", details.Errors.Keys);
            Assert.Contains("Title is required", details.Errors["Title"]);

            _taskItemServiceMock.Verify(
                s => s.CreateAsync(It.IsAny<CreateTaskItemRequest>()),
                Times.Never);
        }

        [Fact]
        public async Task Create_ValidRequest_ReturnsCreatedAtActionWithItem()
        {
            // Arrange
            var request = new CreateTaskItemRequest
            {
                Title = "New task",
                Description = "Test description"
            };

            var created = new TaskItemDto
            {
                Id = 10,
                Title = request.Title,
                Description = request.Description,
                IsComplete = false
            };

            _taskItemServiceMock
                .Setup(s => s.CreateAsync(request))
                .ReturnsAsync(created);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(TaskItemController.GetById), createdResult.ActionName);
            Assert.Equal(10, createdResult.RouteValues?["id"]);
            var returned = Assert.IsType<TaskItemDto>(createdResult.Value);
            Assert.Equal(created.Id, returned.Id);
            Assert.Equal(created.Title, returned.Title);

            _taskItemServiceMock.Verify(s => s.CreateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Delete_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Delete(0);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid Id", badRequest.Value);

            _taskItemServiceMock.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_NotFound_ReturnsNotFound()
        {
            // Arrange
            _taskItemServiceMock
                .Setup(s => s.DeleteAsync(1))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _taskItemServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task Delete_ExistingItem_ReturnsNoContent()
        {
            // Arrange
            _taskItemServiceMock
                .Setup(s => s.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _taskItemServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task Update_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpdateTaskItemRequest
            {
                Title = "Updated",
                Description = "Desc",
                IsComplete = true
            };

            // Act
            var result = await _controller.Update(0, request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid Id", badRequest.Value);

            _taskItemServiceMock.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateTaskItemRequest>()), Times.Never);
        }

        [Fact]
        public async Task Update_InvalidModel_ReturnsBadRequestValidationProblem()
        {
            // Arrange
            var request = new UpdateTaskItemRequest
            {
                Title = string.Empty,
                Description = "Desc",
                IsComplete = false
            };
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.Update(1, request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var details = Assert.IsType<ValidationProblemDetails>(objectResult.Value);

            Assert.Contains("Title", details.Errors.Keys);
            Assert.Contains("Title is required", details.Errors["Title"]);

            _taskItemServiceMock.Verify(
                s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateTaskItemRequest>()),
                Times.Never);
        }

        [Fact]
        public async Task Update_NotFound_ReturnsNoContent()
        {
            // Arrange
            var request = new UpdateTaskItemRequest
            {
                Title = "Updated",
                Description = "Desc",
                IsComplete = true
            };

            _taskItemServiceMock
                .Setup(s => s.UpdateAsync(1, request))
                .ReturnsAsync((TaskItemDto?)null);

            // Act
            var result = await _controller.Update(1, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _taskItemServiceMock.Verify(s => s.UpdateAsync(1, request), Times.Once);
        }

        [Fact]
        public async Task Update_ExistingItem_ReturnsOkWithUpdatedItem()
        {
            // Arrange
            var request = new UpdateTaskItemRequest
            {
                Title = "Updated",
                Description = "Desc",
                IsComplete = true
            };

            var updated = new TaskItemDto
            {
                Id = 1,
                Title = request.Title,
                Description = request.Description,
                IsComplete = request.IsComplete
            };

            _taskItemServiceMock
                .Setup(s => s.UpdateAsync(1, request))
                .ReturnsAsync(updated);

            // Act
            var result = await _controller.Update(1, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<TaskItemDto>(okResult.Value);
            Assert.Equal(updated.Id, returned.Id);
            Assert.Equal(updated.Title, returned.Title);
            Assert.Equal(updated.IsComplete, returned.IsComplete);

            _taskItemServiceMock.Verify(s => s.UpdateAsync(1, request), Times.Once);
        }
    }
}
