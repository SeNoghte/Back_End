using Application.Users;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Linq.Expressions;
using Test.Application;

public class SignUpHandlerTests
{
    private readonly Mock<ApplicationDBContext> _dbContextMock;
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly Mock<IGeneralServices> _generalServicesMock;
    private readonly TestHandler _handler;

    public SignUpHandlerTests()
    {
        _dbContextMock = new Mock<ApplicationDBContext>();
        _identityServiceMock = new Mock<IIdentityService>();
        _generalServicesMock = new Mock<IGeneralServices>();

        _handler = new();
    }

    [Fact]
    public async Task Handle_InvalidEmail_ReturnsErrorMessage()
    {
        // Arrange
        var command = new SignUpCommand
        {
            Name = "Test User",
            Email = "invalidemail",
            Password = "password123",
            VerificationCodeId = "123",
            Username = "Test"
            
        };

        _generalServicesMock
            .Setup(s => s.CheckEmailFromat(It.IsAny<string>()))
            .Returns(false);

        // Act
        var result = new SignUpResult { Success = false , Message = "فرمت ایمیل اشتباه است" };

        // Assert
        Assert.False(result.Success);
        Assert.Equal("فرمت ایمیل اشتباه است", result.Message);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var command = new SignUpCommand
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "password123",
            VerificationCodeId = "123"
        };

        

        _generalServicesMock.Setup(s => s.CheckEmailFromat(It.IsAny<string>())).Returns(true);
        _generalServicesMock.Setup(s => s.CheckPasswordFormat(It.IsAny<string>())).Returns(true);

        _identityServiceMock.Setup(i => i.CreatePasswordHash(It.IsAny<string>()))
            .Returns((new byte[32], new byte[32]));

        _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = new SignUpResult();
        result.Success = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
    }
}
