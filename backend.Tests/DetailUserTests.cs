using Xunit;
using MediatR;
using Moq;
using Conduit.Features.Users;
using Microsoft.EntityFrameworkCore;
using Conduit.Infrastructure.Security;

namespace backend.Tests;

public class DetailUserTests
{
    [Fact]
    public async Task Handler_Should_Return_User_Details()
    {
        // Arrange
        /*
        
        var mockHandler = new Mock<IRequestHandler<Details.Query, UserEnvelope>>();
        mockHandler
            .Setup(x => x.Handle(It.IsAny<Details.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserEnvelope(new User
            {
                Username = person.Username,
                Email = person.Email,
                Token = "mocked_token"
            }));
        */

        var options = new DbContextOptionsBuilder<Conduit.Infrastructure.ConduitContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using var context = new Conduit.Infrastructure.ConduitContext(options);

        var person = new Conduit.Domain.Person
        {
            Username = "testuser",
            Email = "testuser@example.com",
        };

        await context.Persons.AddAsync(person);
        await context.SaveChangesAsync();

        
        var mockTokenGen = new Mock<IJwtTokenGenerator>();
        mockTokenGen.Setup(t => t.CreateToken(person.Username)).Returns("mocked_token");

        var mockMapper = new Mock<AutoMapper.IMapper>();
        mockMapper.Setup(m => m.Map<Conduit.Domain.Person, User>(It.IsAny<Conduit.Domain.Person>()))
            .Returns((Conduit.Domain.Person p) => new User
            {
                Username = p.Username,
                Email = p.Email,
                Token = "mocked_token"
            });

        var sut = new Details.QueryHandler(context, mockTokenGen.Object, mockMapper.Object);

        // Act & Assert
        var result = await sut.Handle(new Details.Query(person.Username), CancellationToken.None);
        //var result = await mockHandler.Object.Handle(new Details.Query(person.Username), CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(person.Username, result.User.Username);
        Assert.Equal(person.Email, result.User.Email);
        Assert.Equal("mocked_token", result.User.Token);
    }
}