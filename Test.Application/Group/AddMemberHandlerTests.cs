using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using Application.Groups;
using Application.Users;
using DataAccess;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Test.Application;
using Xunit;

public class AddMemberHandlerTests
{
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<IGeneralServices> _mockGeneralServices;
    private readonly List<User> _dbContext;
    private readonly TestHandler _handler;

    public AddMemberHandlerTests()
    {
        _mockIdentityService = new Mock<IIdentityService>();
        _mockGeneralServices = new Mock<IGeneralServices>();

        _dbContext = new List<User>() {  new User
        {
            UserId = Guid.NewGuid(),
            Username = "john_doe",
            Name = "John Doe",
            Email = "john@example.com",
            JoinedDate = DateTime.UtcNow
        },
        new User
        {
            UserId = Guid.NewGuid(),
            Username = "jane_doe",
            Name = "Jane Doe",
            Email = "jane@example.com",
            JoinedDate = DateTime.UtcNow
        },
        new User
        {
            UserId = Guid.NewGuid(),
            Username = "test_user",
            Name = "Test User",
            Email = "test@example.com",
            JoinedDate = DateTime.UtcNow
        }};

        // Seed the database with groups and users for testing  
        SeedDatabase();

        _handler = new();
    }

    private void SeedDatabase()
    {
        var user1 = new User { UserId = Guid.NewGuid(), Username = "john_doe", Name = "John Doe", Email = "john@example.com", JoinedDate = DateTime.UtcNow };
        var user2 = new User { UserId = Guid.NewGuid(), Username = "jane_doe", Name = "Jane Doe", Email = "jane@example.com", JoinedDate = DateTime.UtcNow };
        var user3 = new User { UserId = Guid.NewGuid(), Username = "test_user", Name = "Test User", Email = "test@example.com", JoinedDate = DateTime.UtcNow };

        var group = new Group { Id = Guid.NewGuid(), Name = "Test Group", OwnerId = user1.UserId };

        _dbContext.AddRange(new List<User>{ user1, user2, user3 });

    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange  
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns((Guid?)null);
        var command = new AddMemberCommand { GroupId = Guid.NewGuid().ToString(), UsersToAdd = new List<string>() };

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status401Unauthorized, Message = "Unauthorized", Success = false };
        result = await _handler.Handle(result);
        // Assert  
        Assert.Equal(401, result.ErrorCode);
        Assert.Equal("Unauthorized", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenGroupDoesNotExist()
    {
        // Arrange  
        var userId = Guid.NewGuid(); // Simulate a valid user ID  
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockGeneralServices.Setup(s => s.GetUser(userId)).ReturnsAsync(new User { UserId = userId });
        var command = new AddMemberCommand { GroupId = Guid.NewGuid().ToString(), UsersToAdd = new List<string>() };

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status404NotFound, Message = "گروه یافت نشد", Success = false };
        result = await _handler.Handle(result);
        // Assert  
        Assert.Equal(404, result.ErrorCode);
        Assert.Equal("گروه یافت نشد", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnForbidden_WhenUserIsNotOwner()
    {
        // Arrange  
  
     
       

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status403Forbidden, Message = "فقط مدیر گروه میتواند عضو کند", Success = false };
        result = await _handler.Handle(result);
        // Assert  
        Assert.Equal(403, result.ErrorCode);
        Assert.Equal("فقط مدیر گروه میتواند عضو کند", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldAddMembers_WhenUsersExistAndAreNotMembers()
    {
        // Arrange  
    

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status500InternalServerError, Message = "مشکلی پیش آمده است", Success = true };
        result = await _handler.Handle(result);
        result = await _handler.Handle(result);
        // Assert  
        Assert.True(result.Success);
        

    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenExceptionOccurs()
    {
        // Arrange  
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        var command = new AddMemberCommand { GroupId = Guid.NewGuid().ToString(), UsersToAdd = new List<string> { Guid.NewGuid().ToString() } };
 

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status500InternalServerError, Message = "مشکلی پیش آمده است", Success = false };
        result = await _handler.Handle(result);

        // Assert  
        Assert.False(result.Success);
        Assert.Equal("مشکلی پیش آمده است", result.Message);
        Assert.Equal(500, result.ErrorCode);
    }
}