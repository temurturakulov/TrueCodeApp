using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrueCodeApp.Core.Domain.Extensions;
using TrueCodeApp.Core.Domain.Services;
using TrueCodeApp.Currency.Domain.Repositories;
using TrueCodeApp.Currency.Infrastructure.Dtos;
using TrueCodeApp.Currency.Web.Controllers;

namespace TrueCodeApp.Currency.Tests
{
    [TestFixture]
    public class CurrencyControllerTests
    {
        private Mock<ICurrencyRepository> _currencyRepositoryMock = null!;
        private Mock<IUserIdentityService> _userIdentityServiceMock = null!;
        private CurrencyController _controller = null!;
        private const long TestUserId = 42;

        [SetUp]
        public void SetUp()
        {
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _userIdentityServiceMock = new Mock<IUserIdentityService>();
            _userIdentityServiceMock.Setup(x => x.UserId).Returns(TestUserId);

            _controller = new CurrencyController(
                _currencyRepositoryMock.Object,
                _userIdentityServiceMock.Object);
        }

        [Test]
        public async Task GetUserCurrencies_ReturnsOkWithUserCurrencies()
        {
            // Arrange
            var expectedCurrencies = new List<CurrencyDto>
            {
                new() { Id = 1, Name = "USD", Rate = 100 },
                new() { Id = 2, Name = "EUR", Rate = 110 }
            };

            _currencyRepositoryMock
                .Setup(x => x.GetCurrencyByUserAsync(TestUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCurrencies);

            // Act
            var result = await _controller.GetUserCurrencies(CancellationToken.None) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            var returnedCurrencies = result.Value as List<CurrencyDto>;
            Assert.That(returnedCurrencies, Is.Not.Null);
            Assert.That(returnedCurrencies!, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task AddFavorite_Success_ReturnsOk()
        {
            // Arrange
            const long currencyId = 10;

            _currencyRepositoryMock
                .Setup(x => x.AddCurrencyAsync(TestUserId, currencyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult.Success())
                .Verifiable();

            // Act
            var result = await _controller.AddFavorite(currencyId, CancellationToken.None);

            // Assert
            _currencyRepositoryMock.Verify();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task AddFavorite_Failure_ReturnsErrorStatus()
        {
            // Arrange
            const long currencyId = 99;
            var errorResult = ServiceResult.Failure("Currency not found", HttpStatusCode.NotFound);

            _currencyRepositoryMock
                .Setup(x => x.AddCurrencyAsync(TestUserId, currencyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(errorResult);

            // Act
            var result = await _controller.AddFavorite(currencyId, CancellationToken.None);

            // Assert
            var statusResult = result as ObjectResult;
            Assert.That(statusResult, Is.Not.Null);
            Assert.That(statusResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteFavorite_Success_ReturnsOk()
        {
            // Arrange
            const long currencyId = 11;

            _currencyRepositoryMock
                .Setup(x => x.DeleteCurrencyAsync(TestUserId, currencyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult.Success())
                .Verifiable();

            // Act
            var result = await _controller.DeleteFavorite(currencyId, CancellationToken.None);

            // Assert
            _currencyRepositoryMock.Verify();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task DeleteFavorite_Failure_ReturnsErrorStatus()
        {
            // Arrange
            const long currencyId = 12;
            var failure = ServiceResult.Failure("Not found", HttpStatusCode.NotFound);

            _currencyRepositoryMock
                .Setup(x => x.DeleteCurrencyAsync(TestUserId, currencyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(failure);

            // Act
            var result = await _controller.DeleteFavorite(currencyId, CancellationToken.None);

            // Assert
            var statusResult = result as ObjectResult;
            Assert.That(statusResult, Is.Not.Null);
            Assert.That(statusResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetCurrencies_ReturnsOkWithAllCurrencies()
        {
            // Arrange
            var expectedCurrencies = new List<CurrencyDto>
            {
                new() { Id = 1, Name = "USD", Rate = 100 },
                new() { Id = 2, Name = "EUR", Rate = 110 },
                new() { Id = 3, Name = "CNY", Rate = 12 }
            };

            _currencyRepositoryMock
                .Setup(x => x.GetCurrencies(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCurrencies);

            // Act
            var result = await _controller.GetCurrencies(CancellationToken.None) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            var returnedCurrencies = result.Value as List<CurrencyDto>;
            Assert.That(returnedCurrencies, Is.Not.Null);
            Assert.That(returnedCurrencies!, Has.Count.EqualTo(3));
        }
    }
}