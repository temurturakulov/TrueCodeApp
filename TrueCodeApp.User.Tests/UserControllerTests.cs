using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrueCodeApp.Core.Domain.Extensions;
using TrueCodeApp.Core.Domain.Models;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.User.Domain.Services;
using TrueCodeApp.User.Infrastructure.Models;
using TrueCodeApp.User.Web.Controllers;

namespace TrueCodeApp.User.Tests;

[TestFixture]
public class UserControllerTests
{
    private Mock<IAuthService> _authMock;
    private Mock<ITokenRevocationService> _revocationMock;
    private Mock<IUserIdentityService> _identityMock;
    private UserController _controller;

    [SetUp]
    public void Setup()
    {
        _authMock = new Mock<IAuthService>();
        _revocationMock = new Mock<ITokenRevocationService>();
        _identityMock = new Mock<IUserIdentityService>();

        _controller = new UserController(_authMock.Object, _revocationMock.Object, _identityMock.Object);
    }

    [Test]
    public async Task Register_Should_Return_Ok_When_Success()
    {
        _authMock.Setup(a => a.RegisterAsync("alice", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult.Success());

        var result = await _controller.Register(new RegisterRequest ("alice", "pass" ),
            CancellationToken.None);
        
        Assert.That(result, Is.InstanceOf<OkResult>());
    }

    [Test]
    public async Task Register_Should_Return_Conflict_When_Failure()
    {
        _authMock.Setup(a => a.RegisterAsync("bob", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult.Failure("User exists", HttpStatusCode.Conflict));

        var result = await _controller.Register(new RegisterRequest ("bob", "pass" ),
            CancellationToken.None);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.Conflict));
    }

    [Test]
    public async Task Login_Should_Return_Ok_When_Success()
    {
        var tokenModel = new TokenModel { Token = "token", ExpirationDate =  DateTimeOffset.UtcNow.AddDays(1) , IssuedDate =  DateTimeOffset.UtcNow };
        _authMock.Setup(a => a.LoginAsync("alice", "pass", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TokenModel>.Success(tokenModel));

        var result = await _controller.Login(new LoginRequest ("alice", "pass"),
            CancellationToken.None);

        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo(tokenModel));
    }

    [Test]
    public async Task Login_Should_Return_Unauthorized_When_Failure()
    {
        _authMock.Setup(a => a.LoginAsync("bob", "wrong", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TokenModel>.Failure("Invalid", HttpStatusCode.Unauthorized));

        var result = await _controller.Login(new LoginRequest ("bob", "wrong"),
            CancellationToken.None);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Logout_Should_Revoke_Token_When_Success()
    {
        _identityMock.Setup(i => i.GetToken)
            .Returns(ServiceResult<string>.Success("token"));
        _identityMock.Setup(i => i.UserId).Returns(1);
        _revocationMock.Setup(r =>
                r.RevokeAsync("token", It.IsAny<System.DateTimeOffset>(), 1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Logout(CancellationToken.None);

        var okResult = result as OkObjectResult;
        Assert.That(okResult!.Value, Is.EqualTo("Token revoked"));
    }
}