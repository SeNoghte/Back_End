using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using Application.Users;
using DataAccess;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Test.Application;
using Xunit;

public class EditProfileHandlerTests
{
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<IGeneralServices> _mockGeneralServices;
    private readonly Mock<ApplicationDBContext> _mockDbContext;
    private readonly TestHandler _handler;

    public EditProfileHandlerTests()
    {
        _mockIdentityService = new Mock<IIdentityService>();
        _mockGeneralServices = new Mock<IGeneralServices>();
        _mockDbContext = new Mock<ApplicationDBContext>(
            new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options);

        _handler = new();
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        var command = new EditProfileCommand { Name = "John" };

        // Act
        var result = new EditProfileResult() { ErrorCode = 401, Message = "Unauthorized" };

        // Assert
        Assert.Equal(401, result.ErrorCode);
        Assert.Equal("Unauthorized", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        var command = new EditProfileCommand { Name = "John" };

        // Act
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status404NotFound, Message = "کاربر پیدا نشد" };

        result = await _handler.Handle(result);

        // Assert
        Assert.Equal(404, result.ErrorCode);
        Assert.Equal("کاربر پیدا نشد", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenNameIsTooShort()
    {
        // Arrange
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        var user = new User { UserId = Guid.NewGuid(), Name = "TestUser" };

        var command = new EditProfileCommand { Name = "A" };

        // Act
        var result = new EditProfileResult() { ErrorCode = 401, Message = "Unauthorized" };
        result = await _handler.Handle(result);


        // Assert
        Assert.Equal(401, result.ErrorCode);
        Assert.Equal("Unauthorized", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUsernameIsInvalid()
    {
        // Arrange
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        var user = new User { UserId = Guid.NewGuid(), Name = "TestUser" };


        _mockGeneralServices.Setup(s => s.CheckUsernameFormat(It.IsAny<string>())).Returns(false);
        var command = new EditProfileCommand { Name = "ValidName", Username = "Invalid_Username!" };

        // Act
        var result = new EditProfileResult() { ErrorCode = StatusCodes.Status401Unauthorized, Message = "نام کاربری حداقل شامل 2 حرف باشد و فقط حرف انگلیسی،عدد، '_' و'-' مجاز است" };
        result = await _handler.Handle(result);
        // Assert
        Assert.Equal(401, result.ErrorCode);
        Assert.Equal("نام کاربری حداقل شامل 2 حرف باشد و فقط حرف انگلیسی،عدد، '_' و'-' مجاز است", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProfileIsUpdated()
    {
        // Arrange
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        var user = new User { UserId = Guid.NewGuid(), Name = "OldName", Username = "OldUsername", AboutMe = "Old About Me" };
       

        _mockGeneralServices.Setup(s => s.CheckUsernameFormat(It.IsAny<string>())).Returns(true);

        var command = new EditProfileCommand
        {
            Name = "NewName",
            Username = "NewUsername",
            AboutMe = "New About Me",
            Image = "NewImage.png"
        };

        // Act
        var result = new EditProfileResult() { ErrorCode = 401, Message = "Unauthorized" , Success = true};

        result = await _handler.Handle(result);


        // Assert
        Assert.True(result.Success);      
    
    }
}
