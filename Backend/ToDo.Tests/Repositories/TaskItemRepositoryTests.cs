using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDo.DAL;
using ToDo.DAL.Models;
using ToDo.DAL.Repositories;
using Xunit;

namespace ToDo.Api.Tests.Repositories;

public class TaskItemRepositoryTests
{
    private const string DefaultUserId = "user-1";

    private (TaskItemRepository Repository, AppDbContext Context) CreateRepository()
    {
        // Use a unique in-memory DB per test to avoid cross-test interference
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        context.Users.Add(new AppUser
        {
            Id = DefaultUserId,
            Email = "user1@example.com",
            UserName = "user1"
        });

        context.SaveChanges();

        return (new TaskItemRepository(context), context);
    }

    [Fact]
    public async Task AddAsync_AssignsId_AndStoresItem()
    {
        // Arrange
        var (repository, _) = CreateRepository();
        var task = new TaskItem
        {
            Title = "Test task",
            Description = "Test description",
            IsComplete = false,
            UserId = DefaultUserId
        };

        // Act
        var added = await repository.AddAsync(task);
        var all = (await repository.GetAllAsync(DefaultUserId)).ToList();

        // Assert
        Assert.NotEqual(0, added.Id);
        Assert.Equal(added.Id, task.Id); // Id set on original object

        Assert.Single(all);

        var stored = all.First();

        Assert.Equal("Test task", stored.Title);
        Assert.Equal("Test description", stored.Description);
        Assert.False(stored.IsComplete);

        // CreatedAt should be auto-populated (either by repo or caller)
        Assert.NotEqual(default(DateTime), stored.CreatedAt);
        Assert.True(stored.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsItem_WhenExists()
    {
        // Arrange
        var (repository, _) = CreateRepository();
        var task = new TaskItem
        {
            Title = "Existing task",
            Description = "Existing description",
            IsComplete = false,
            UserId = DefaultUserId
        };

        var added = await repository.AddAsync(task);

        // Act
        var result = await repository.GetByIdAsync(added.Id, DefaultUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(added.Id, result!.Id);
        Assert.Equal("Existing task", result.Title);
        Assert.Equal("Existing description", result.Description);
        Assert.False(result.IsComplete);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var (repository, _) = CreateRepository();

        // Act
        var result = await repository.GetByIdAsync(999, DefaultUserId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedItem_WhenExists()
    {
        // Arrange
        var (repository, _) = CreateRepository();
        var task = new TaskItem
        {
            Title = "Old title",
            Description = "Old description",
            IsComplete = false,
            UserId = DefaultUserId
        };

        var added = await repository.AddAsync(task);

        added.Title = "New title";
        added.Description = "New description";
        added.IsComplete = true;

        // Act
        var updated = await repository.UpdateAsync(added, DefaultUserId);
        var fetched = await repository.GetByIdAsync(added.Id, DefaultUserId);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("New title", updated!.Title);
        Assert.Equal("New description", updated.Description);
        Assert.True(updated.IsComplete);

        Assert.NotNull(fetched);
        Assert.Equal("New title", fetched!.Title);
        Assert.Equal("New description", fetched.Description);
        Assert.True(fetched.IsComplete);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenItemDoesNotExist()
    {
        // Arrange
        var (repository, _) = CreateRepository();
        var nonExisting = new TaskItem
        {
            Id = 123,
            Title = "Does not exist",
            Description = "Does not exist",
            IsComplete = false,
            UserId = DefaultUserId
        };

        // Act
        var result = await repository.UpdateAsync(nonExisting, DefaultUserId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesItem_AndReturnsTrue_WhenExists()
    {
        // Arrange
        var (repository, _) = CreateRepository();
        var task = new TaskItem
        {
            Title = "To delete",
            Description = "To delete desc",
            IsComplete = false,
            UserId = DefaultUserId
        };

        var added = await repository.AddAsync(task);

        // Act
        var deleted = await repository.DeleteAsync(added.Id, DefaultUserId);
        var fetched = await repository.GetByIdAsync(added.Id, DefaultUserId);

        // Assert
        Assert.True(deleted);
        Assert.Null(fetched);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenItemDoesNotExist()
    {
        // Arrange
        var (repository, _) = CreateRepository();

        // Act
        var deleted = await repository.DeleteAsync(999, DefaultUserId);

        // Assert
        Assert.False(deleted);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsItemsOrderedByCreatedAt()
    {
        // Arrange
        var (repository, _) = CreateRepository();

        var first = new TaskItem
        {
            Title = "First",
            Description = "First desc",
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            IsComplete = false,
            UserId = DefaultUserId
        };

        var second = new TaskItem
        {
            Title = "Second",
            Description = "Second desc",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            IsComplete = true,
            UserId = DefaultUserId
        };

        await repository.AddAsync(first);
        await repository.AddAsync(second);

        // Act
        var all = (await repository.GetAllAsync(DefaultUserId)).ToList();

        // Assert
        Assert.Equal(2, all.Count);
        Assert.Equal("First", all[0].Title);
        Assert.Equal("Second", all[1].Title);
    }

    [Fact]
    public async Task GetAllAsync_FiltersByUser()
    {
        // Arrange
        var (repository, _) = CreateRepository();

        await repository.AddAsync(new TaskItem
        {
            Title = "Mine",
            UserId = DefaultUserId
        });

        await repository.AddAsync(new TaskItem
        {
            Title = "Theirs",
            UserId = "other-user"
        });

        // Act
        var all = (await repository.GetAllAsync(DefaultUserId)).ToList();

        // Assert
        Assert.Single(all);
        Assert.Equal("Mine", all[0].Title);
    }
}
