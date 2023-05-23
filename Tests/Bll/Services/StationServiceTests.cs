using Grip.Bll.DTO;
using Grip.Bll.Exceptions;
using Grip.Bll.Services;
using Grip.Bll.Services.Interfaces;
using Grip.DAL;
using Grip.DAL.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.Grip.Bll.Services
{
    /*[TestFixture]
    public class StationServiceTests
    {
        private Mock<ILogger<StationService>> _loggerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<ApplicationDbContext> _contextMock;

        private StationService _stationService;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<StationService>>();
            _configurationMock = new Mock<IConfiguration>();
            _contextMock = new Mock<ApplicationDbContext>();

            _stationService = new StationService(
                _loggerMock.Object,
                _configurationMock.Object,
                _contextMock.Object
            );
        }

        [Test]
        public async Task GetSecretKey_ExistingStation_ReturnsStationSecretKeyDTO()
        {
            // Arrange
            int stationNumber = 123;
            var existingStation = new Station
            {
                StationNumber = stationNumber,
                SecretKey = "existing-secret-key"
            };

            _contextMock.Setup(c => c.Stations)
                .ReturnsDbSet(new[] { existingStation });

            // Act
            var result = await _stationService.GetSecretKey(stationNumber);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(existingStation.SecretKey, result.SecretKey);
        }

        [Test]
        public async Task GetSecretKey_NonExistingStation_CreateDbEntryOnKeyRequestEnabled_ReturnsStationSecretKeyDTO()
        {
            // Arrange
            int stationNumber = 123;

            _contextMock.Setup(c => c.Stations)
                .ReturnsDbSet(Enumerable.Empty<Station>());

            _configurationMock.Setup(config => config["Station:CreateDbEntryOnKeyRequest"])
                .Returns("True");

            // Act            
            var result = await _stationService.GetSecretKey(stationNumber);

            // Assert            
            Assert.IsNotNull(result);

            _contextMock.Verify(c => c.Stations.Add(It.IsAny<Station>()), Times.Once);
            _contextMock.Verify(c => c.SaveChanges(), Times.Once);

        }

        [Test]
        public void GetSecretKey_NonExistingStation_CreateDbEntryOnKeyRequestDisabled_ThrowsBadRequestException()
        {
            // Arrange
            int stationNumber = 123;

            _contextMock.Setup(c => c.Stations)
                .ReturnsDbSet(Enumerable.Empty<Station>());

            _configurationMock.Setup(config => config["Station:CreateDbEntryOnKeyRequest"])
                .Returns("False");

            // Act & Assert
            Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await _stationService.GetSecretKey(stationNumber);
            });

        }
    }*/
}
