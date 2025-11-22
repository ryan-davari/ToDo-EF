using AutoMapper;
using Moq;
using ToDo.Api.Dtos;
using ToDo.DAL.Models;
using ToDo.Api.Repositories;
using ToDo.Api.Services;
using Xunit;

namespace ToDo.Tests.Services
{
    public class TaskItemServiceTests
    {
        private readonly Mock<ITaskItemRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TaskItemService _sut;

        public TaskItemServiceTests()
        {
            _repositoryMock = new Mock<ITaskItemRepository>(); // Loose behavior is fine here
            _mapperMock = new Mock<IMapper>();

            _sut = new TaskItemService(_repositoryMock.Object, _mapperMock.Object);
        }

        /// <summary>
        /// Simple happy-path test to ensure we map everything the repository returns into DTOs.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            // Arrange
            var items = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Task 1" },
                new TaskItem { Id = 2, Title = "Task 2" }
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(items);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<TaskItemDto>>(It.IsAny<IEnumerable<TaskItem>>()))
                .Returns((IEnumerable<TaskItem> src) =>
                    src.Select(t => new TaskItemDto
                    {
                        Id = t.Id,
                        Title = t.Title
                    }).ToList());

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0].Id);
            Assert.Equal("Task 1", list[0].Title);
        }

        /// <summary>
        /// Checks that we correctly return a single DTO when the task exists.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ItemExists_ReturnsDto()
        {
            // Arrange
            var entity = new TaskItem { Id = 10, Title = "Test task" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(entity);

            _mapperMock
                .Setup(m => m.Map<TaskItemDto>(entity))
                .Returns(new TaskItemDto { Id = entity.Id, Title = entity.Title });

            // Act
            var result = await _sut.GetByIdAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result!.Id);
            Assert.Equal("Test task", result.Title);
        }

        /// <summary>
        /// If the repository can’t find a task, we simply return null from the service.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ItemDoesNotExist_ReturnsNull()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _sut.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Creates a new task, trims the title and sets sensible defaults before saving.
        /// </summary>
        [Fact]
        public async Task CreateAsync_ValidRequest_TrimsTitleAndSetsDefaults()
        {
            // Arrange
            var request = new CreateTaskItemRequest
            {
                Title = "  New task  ",
                Description = "Some desc"
            };

            _mapperMock
                .Setup(m => m.Map<TaskItem>(request))
                .Returns((CreateTaskItemRequest src) => new TaskItem
                {
                    Title = src.Title,
                    Description = src.Description
                });

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync((TaskItem e) =>
                {
                    e.Id = 123;
                    return e;
                });

            _mapperMock
                .Setup(m => m.Map<TaskItemDto>(It.IsAny<TaskItem>()))
                .Returns((TaskItem src) => new TaskItemDto
                {
                    Id = src.Id,
                    Title = src.Title,
                    Description = src.Description,
                    IsComplete = src.IsComplete,
                    CreatedAt = src.CreatedAt
                });

            // Act
            var result = await _sut.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(123, result.Id);
            Assert.Equal("New task", result.Title); // trimmed
            Assert.False(result.IsComplete);
            Assert.NotEqual(default, result.CreatedAt);
        }

        /// <summary>
        /// Guard rail: we don’t allow empty or whitespace-only titles when creating a task.
        /// </summary>
        [Fact]
        public async Task CreateAsync_EmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateTaskItemRequest
            {
                Title = "   ",
                Description = "Whatever"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(request));
        }

        /// <summary>
        /// Updates an existing task, keeps CreatedAt, and trims the new title.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ItemExists_UpdatesAndTrimsTitle()
        {
            // Arrange
            var existing = new TaskItem
            {
                Id = 5,
                Title = " Old title ",
                Description = "Old",
                IsComplete = false,
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            };

            var request = new UpdateTaskItemRequest
            {
                Title = "  New title  ",
                Description = "New",
                IsComplete = true
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(existing);

            _mapperMock
                .Setup(m => m.Map(request, existing))
                .Callback<UpdateTaskItemRequest, TaskItem>((src, dest) =>
                {
                    dest.Title = src.Title;
                    dest.Description = src.Description;
                    dest.IsComplete = src.IsComplete;
                });

            _repositoryMock
                .Setup(r => r.UpdateAsync(existing))
                .ReturnsAsync(existing);

            _mapperMock
                .Setup(m => m.Map<TaskItemDto>(existing))
                .Returns((TaskItem src) => new TaskItemDto
                {
                    Id = src.Id,
                    Title = src.Title,
                    Description = src.Description,
                    IsComplete = src.IsComplete,
                    CreatedAt = src.CreatedAt
                });

            var originalCreatedAt = existing.CreatedAt;

            // Act
            var result = await _sut.UpdateAsync(5, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result!.Id);
            Assert.Equal("New title", result.Title); // trimmed
            Assert.True(result.IsComplete);
            Assert.Equal(originalCreatedAt, result.CreatedAt); // unchanged
        }

        /// <summary>
        /// If you try to update a task that doesn’t exist, the service simply returns null.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ItemDoesNotExist_ReturnsNull()
        {
            // Arrange
            var request = new UpdateTaskItemRequest
            {
                Title = "Does not matter",
                Description = "x",
                IsComplete = false
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _sut.UpdateAsync(999, request);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Tiny sanity check that delete simply forwards the call to the repository.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_DeletesItemThroughRepository()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.DeleteAsync(7))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.DeleteAsync(7);

            // Assert
            Assert.True(result);
        }
    }
}
