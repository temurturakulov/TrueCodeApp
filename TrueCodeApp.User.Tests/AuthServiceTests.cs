using System.Net;
using FluentAssertions;
using Moq;
using TrueCodeApp.Core.Domain.Models;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.User.Domain.Repositories;
using TrueCodeApp.User.Infrastructure.Services;

namespace TrueCodeApp.User.Tests;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserRepository> _userRepoMock;
    private Mock<IJwtTokenService> _jwtMock;
    private AuthService _service;

    [SetUp]
    public void Setup()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _jwtMock = new Mock<IJwtTokenService>();
        _service = new AuthService(_userRepoMock.Object, _jwtMock.Object);
    }

    [Test]
    public async Task RegisterAsync_Should_Return_Conflict_When_User_Exists()
    {
        var user = new Core.Domain.Entities.User
        {
            Id = 1,
            Login = "alice",
            Password = "test123"
        };

        _userRepoMock.Setup(r => r.GetUserByLogin(user.Login, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.RegisterAsync(user.Login, user.Password, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("User already exists");
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task RegisterAsync_Should_Create_User_When_Not_Exists()
    {
        var user = new Core.Domain.Entities.User
        {
            Id = 1,
            Login = "alice",
            Password = "test123"
        };

        _userRepoMock.Setup(r => r.GetUserByLogin(user.Login, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Core.Domain.Entities.User?)null);

        _userRepoMock.Setup(r => r.AddAsync(
                It.IsAny<Core.Domain.Entities.User>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.RegisterAsync(user.Login, user.Password, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        _userRepoMock.Verify(
            r => r.AddAsync(
                It.Is<Core.Domain.Entities.User>(u => u.Login == user.Login),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task LoginAsync_Should_Return_NotFound_When_User_Not_Exists()
    {
        _userRepoMock.Setup(r => r.GetUserByLogin("alice", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Core.Domain.Entities.User?)null);

        var result = await _service.LoginAsync("alice", "pass", CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.ErrorMessage.Should().Be("User not found");
    }

    [Test]
    public async Task LoginAsync_Should_Return_Unauthorized_When_Password_Wrong()
    {
        var user = new Core.Domain.Entities.User { Login = "bob", Password = "wronghash" };
        _userRepoMock.Setup(r => r.GetUserByLogin("bob", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.LoginAsync(user.Login, "pass", CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        result.ErrorMessage.Should().Be("Invalid password"); 
    }

    [Test]
    public async Task LoginAsync_Should_Return_Token_When_Success()
    {
        var password = "pass";
        var user = new Core.Domain.Entities.User { Login = "bob", Password = AuthService.Hash(password) };
        _userRepoMock.Setup(r => r.GetUserByLogin(user.Login, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var tokenModel = new TokenModel
        {
            Token = "token",
            ExpirationDate = DateTimeOffset.UtcNow.AddHours(1),
            IssuedDate = DateTimeOffset.UtcNow
        };
        _jwtMock.Setup(j => j.GenerateAccessToken(user)).Returns(tokenModel);

        var result = await _service.LoginAsync(user.Login, password, CancellationToken.None);
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.IsSuccess.Should().BeTrue();
        result.Data!.Token.Should().Be(tokenModel.Token); 
    }
}