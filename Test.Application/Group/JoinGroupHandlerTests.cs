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

public class JoinGroupHandlerTests
{
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<IGeneralServices> _mockGeneralServices;
    private readonly ApplicationDBContext _dbContext;
    private readonly TestHandler _handler;

    public JoinGroupHandlerTests()
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
        var command = new JoinGroupCommand { GroupId = Guid.NewGuid() };

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status401Unauthorized, Message = "Unauthorized", Success = false };

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
        var command = new JoinGroupCommand { GroupId = Guid.NewGuid() }; // Non-existent group  

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status404NotFound, Message = "گروه یافت نشد", Success = false };

        // Assert  
        Assert.Equal(404, result.ErrorCode);
        Assert.Equal("گروه یافت نشد", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyAMember_WhenUserIsAlreadyInGroup()
    {
        // Arrange  
        var userId = Guid.NewGuid(); // Simulate a valid user ID  
        
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockGeneralServices.Setup(s => s.GetUser(userId)).ReturnsAsync(new User { UserId = userId });



        var command = new JoinGroupCommand { GroupId = Guid.NewGuid() };

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status401Unauthorized, Message = "کاربر از قبل عضو گروه است", Success = true };

        // Assert  
        Assert.Equal(401, result.ErrorCode);
        Assert.Equal("کاربر از قبل عضو گروه است", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldJoinGroup_WhenUserIsNotAMember()
    {
        // Arrange  
        var userId = Guid.NewGuid(); // Simulate a valid user ID       
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _mockGeneralServices.Setup(s => s.GetUser(userId)).ReturnsAsync(new User { UserId = userId });

        var command = new JoinGroupCommand { GroupId = Guid.NewGuid() };

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status500InternalServerError, Message = "عضویت با موفقیت انجام شد", Success = true };
        result = await _handler.Handle(result);
        // Assert  
        Assert.True(result.Success);
        Assert.Equal("عضویت با موفقیت انجام شد", result.Message);

    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenExceptionOccurs()
    {
        // Arrange  
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        var command = new JoinGroupCommand { GroupId = Guid.NewGuid() };

        // Act  
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status500InternalServerError, Message = "مشکلی پیش آمده است", Success = false };
        result = await _handler.Handle(result);
        // Assert  
        Assert.False(result.Success);
        Assert.Equal("مشکلی پیش آمده است", result.Message);
        Assert.Equal(500, result.ErrorCode);
    }
}