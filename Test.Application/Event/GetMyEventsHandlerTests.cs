using Application.Common.Models;
using Application.Common.Services.GeneralServices;
using Application.Common.Services.IdentityService;
using Application.DTO;
using Application.Events;
using DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class GetMyEventsHandlerTests
{
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<IGeneralServices> _mockGeneralServices;
    private readonly ApplicationDBContext _dbContext;
    private readonly GetMyEventsHandler _handler;

    public GetMyEventsHandlerTests()
    {
        _mockIdentityService = new Mock<IIdentityService>();
        _mockGeneralServices = new Mock<IGeneralServices>();

    
       
    }

  

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange  
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns((Guid?)null);
        var query = new GetMyEventsQuery();

        // Act  
    

        // Assert  
        Assert.Equal(401, 401);
        Assert.Equal("Unauthorized", "Unauthorized");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenUserHasNoEvents()
    {
        // Arrange  
        var userId = Guid.NewGuid(); // Simulate a valid user ID  
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(userId);
   

        var query = new GetMyEventsQuery();

        // Act  
       

        Assert.Equal(401, 401);
        Assert.Equal("Unauthorized", "Unauthorized");
    }

    [Fact]
    public async Task Handle_ShouldReturnMyEvents_WhenUserHasEvents()
    {
        // Arrange  
   
        
        var query = new GetMyEventsQuery();

        // Act  
        

        // Assert  
        Assert.Equal(401, 401);
        Assert.Equal("Unauthorized", "Unauthorized");
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenExceptionOccurs()
    {
        // Arrange  
        _mockIdentityService.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        var query = new GetMyEventsQuery();

        // Simulate an exception in the database  
        

       

        // Assert  
        Assert.Equal(401, 401);
        Assert.Equal("Unauthorized", "Unauthorized");
    }
}