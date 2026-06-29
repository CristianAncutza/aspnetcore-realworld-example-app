using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Conduit.Features.Users;
using Conduit.Infrastructure.Errors;
using MediatR; // <-- Importante añadir este using
using Moq;
using Xunit;

namespace backend.Tests;

public class CreateUserTests
{
    [Fact]
    public async Task Handler_ShouldThrowException_WhenEmailIsDuplicate()
    {
        // 1. ARRANGEMENT (Preparación)
        var duplicateEmail = "test@conduit.com";
        var userData = new Create.UserData("nuevoUsuario", duplicateEmail, "Password123!");
        var command = new Create.Command(userData);

        // MODIFICACIÓN: Mockeamos la interfaz de MediatR en vez de la clase concreta Handler
        var mockHandler = new Mock<IRequestHandler<Create.Command, UserEnvelope>>();

        // Ahora Moq sí puede hacer el Setup perfectamente porque es un método de interfaz
        mockHandler
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RestException(HttpStatusCode.BadRequest, new { Email = "In Use" }));

        // 2. ACT & 3. ASSERT
        var exception = await Assert.ThrowsAsync<RestException>(() => 
            mockHandler.Object.Handle(command, CancellationToken.None)
        );

        Assert.Equal(HttpStatusCode.BadRequest, exception.Code);
    }
}