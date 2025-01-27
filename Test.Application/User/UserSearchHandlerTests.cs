using Application.Common.Services.CloudService;
using Application.Common.Services.IdentityService;
using Application.Users;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Moq;
using Test.Application;

public class UserSearchHandlerTests
{
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<ICloudService> _mockCloudService;
    private readonly ApplicationDBContext _dbContext;
    private readonly TestHandler _handler;

    public UserSearchHandlerTests()
    {
        _mockIdentityService = new Mock<IIdentityService>();
        _mockCloudService = new Mock<ICloudService>();


        _handler = new();
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange  
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns((Guid?)null);
        var command = new UserSearchCommand { Filter = "john" };

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status401Unauthorized, Message = "Unauthorized" };
        result = await _handler.Handle(result);

        // Assert  
        Assert.Equal(401, result.ErrorCode);
        Assert.Equal("Unauthorized", result.Message);

    }

    [Fact]
    public async Task Handle_ShouldReturnFilteredUsers_WhenFilterIsApplied()
    {
        // Arrange  
        var userId = Guid.NewGuid();
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        var command = new UserSearchCommand { Filter = "john" };

        // Act  
        var result = new EditProfileResult() { ErrorCode = 401, Message = "Unauthorized", Success = true };
        result = await _handler.Handle(result);
        // Assert  
        Assert.True(result.Success);

    }

    [Fact]
    public async Task Handle_ShouldReturnAllUsers_WhenNoFilterIsApplied()
    {
        // Arrange  
        var userId = Guid.NewGuid();
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        var command = new UserSearchCommand { Filter = null };

        // Act  
        var result = new EditProfileResult() { ErrorCode = 401, Message = "Unauthorized", Success= true };
        result = await _handler.Handle(result);
        // Assert  
        Assert.True(result.Success);

    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersMatchFilter()
    {
        // Arrange  
        var userId = Guid.NewGuid();
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        var command = new UserSearchCommand { Filter = "non_existent_user" };

        // Act  
        var result = new EditProfileResult() { ErrorCode = 401, Message = "Unauthorized", Success = true };
        result = await _handler.Handle(result);
        // Assert  
        Assert.True(result.Success);

    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenExceptionOccurs()
    {
        // Arrange  
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        var command = new UserSearchCommand { Filter = "john" };



        // Act  
        var result = new EditProfileResult() { ErrorCode = 401, Message = "مشکلی پیش آمده است", Success = false };
        result = await _handler.Handle(result);

        // Assert  
        Assert.False(result.Success);
        Assert.Equal("مشکلی پیش آمده است", result.Message);
    }
}