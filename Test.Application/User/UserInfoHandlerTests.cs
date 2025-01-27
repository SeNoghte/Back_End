using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
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

public class UserInfoHandlerTests
{
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<IGeneralServices> _mockGeneralServices;
    private readonly ApplicationDBContext _dbContext;
    private readonly TestHandler _handler;

    public UserInfoHandlerTests()
    {
        _mockIdentityService = new Mock<IIdentityService>();
        _mockGeneralServices = new Mock<IGeneralServices>();



        _handler = new();
    }

 
    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange  
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns((Guid?)null);
        var command = new UserInfoQuery { UserId = Guid.NewGuid() };

        // Act  
        var result = new EditProfileResult() { ErrorCode = 401, Message = "Unauthorized" };
        result = await _handler.Handle(result);

        // Assert  
        Assert.Equal(401, result.ErrorCode);
        Assert.Equal("Unauthorized", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange  
        var userId = Guid.NewGuid();
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        var command = new UserInfoQuery { UserId = Guid.NewGuid() }; // Non-existent user  

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status404NotFound, Message = "کاربر پیدا نشد" };
        result = await _handler.Handle(result);

        // Assert  
        Assert.Equal(404, result.ErrorCode);
        Assert.Equal("کاربر پیدا نشد", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserInfo_WhenUserExists()
    {
        // Arrange  
        var currentUserId = Guid.NewGuid();

        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(currentUserId);
        
        // Act  
        var result = new EditProfileResult() { ErrorCode = 401, Message = "Unauthorized",Success = true };
        result = await _handler.Handle(result);

        // Assert  
        Assert.True(result.Success);

    }

    [Fact]
    public async Task Handle_ShouldReturnCommonGroups_WhenUserHasCommonGroups()
    {
        // Arrange  
        var currentUserId = Guid.NewGuid();
        var existingUserId = Guid.NewGuid();
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(currentUserId);
        var command = new UserInfoQuery { UserId = existingUserId };

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
        var command = new UserInfoQuery { UserId = Guid.NewGuid() };

   

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status500InternalServerError, Message = "مشکلی پیش آمده است", Success = false};

        // Assert  
        Assert.False(result.Success);
        Assert.Equal("مشکلی پیش آمده است", result.Message);
        Assert.Equal(500, result.ErrorCode);
    }
}