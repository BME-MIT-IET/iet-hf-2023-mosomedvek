using Grip.DAL;
using Grip.Bll.DTO;
using Grip.DAL.Model;
using Grip.Bll.Services.Interfaces;
using Grip.Bll.Services;
using Grip.Bll.Exceptions;
using Grip.Bll.Providers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Grip.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Server.Bll.Providers;
using Moq.EntityFrameworkCore;

namespace Tests.Grip.Bll.Services
{
    [TestFixture]
    public class AttendanceServiceTests
    {
        private AttendanceService _attendanceService;
        private Mock<ILogger<AttendanceService>> _loggerMock;
        private Mock<ApplicationDbContext> _contextMock;
        private Mock<IStationTokenProvider> _stationTokenProviderMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IHubContext<StationHub, IStationClient>> _signalrHubMock;
        private Mock<ICurrentTimeProvider> _currentTimeProviderMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<AttendanceService>>();
            //new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("Grip").Options, null))
            _contextMock = new Mock<ApplicationDbContext>();
            _stationTokenProviderMock = new Mock<IStationTokenProvider>();
            _mapperMock = new Mock<IMapper>();
            _signalrHubMock = new Mock<IHubContext<StationHub, IStationClient>>();
            _currentTimeProviderMock = new Mock<ICurrentTimeProvider>();
            _signalrHubMock.Setup(h => h.Clients.Group(It.IsAny<string>()).ReceiveScan(It.IsAny<StationScanDTO>())).Returns(Task.CompletedTask);
            _attendanceService = new AttendanceService(
                _loggerMock.Object,
                _contextMock.Object,
                _stationTokenProviderMock.Object,
                _mapperMock.Object,
                _signalrHubMock.Object,
                _currentTimeProviderMock.Object
            );
        }

        [Test]
        public async Task VerifyPhoneScan_ValidRequest_AddsAttendanceAndSendsSignalRMessage()
        {
            // Arrange
            var request = new ActiveAttendanceDTO
            (
             "1_1621434000_100",
             "abc123"
            );
            var user = new User { Id = 1, UserName = "testUser" };
            var station = new Station { StationNumber = 1, SecretKey = "secretKey" };
            var attendanceTime = new DateTime(2021, 5, 19, 12, 0, 0);
            var expectedAttendance = new Attendance
            {
                Station = station,
                Time = attendanceTime,
                User = user
            };
            var expectedStationScanDTO = new StationScanDTO
            {
                StationId = 1,
                ScanTime = attendanceTime,
                UserInfo = new UserInfoDTO(user.Id, user.UserName)
            };

            _contextMock.Setup(c => c.Stations)
                .ReturnsDbSet(new[] { station });
            _contextMock.Setup(c => c.Attendances)
            .ReturnsDbSet(new List<Attendance> { });
            _stationTokenProviderMock.Setup(p => p.ValidateToken(station.SecretKey, request.Message, request.Token))
                .Returns(true);
            _currentTimeProviderMock.SetupGet(p => p.Now).Returns(attendanceTime);
            _mapperMock.Setup(m => m.Map<UserInfoDTO>(user)).Returns(new UserInfoDTO(user.Id, user.UserName));

            // Act
            await _attendanceService.VerifyPhoneScan(request, user);

            // Assert
            //Assert.That(_contextMock.Object.Attendances.Count(), Is.EqualTo(1));
            _contextMock.Verify(c => c.Attendances.Add(It.IsAny<Attendance>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
            _signalrHubMock.Verify(h => h.Clients.Group(station.StationNumber.ToString()).ReceiveScan(It.IsAny<StationScanDTO>()), Times.Once);
        }

        [Test]
        public void VerifyPhoneScan_InvalidStation_ThrowsException()
        {
            // Arrange
            var request = new ActiveAttendanceDTO
            (
                "1_1621434000",
                "abc123"
            );
            var user = new User { UserName = "testUser" };

            _contextMock.Setup(c => c.Stations)
                .ReturnsDbSet(new List<Station> { });

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _attendanceService.VerifyPhoneScan(request, user));
        }

        [Test]
        public void VerifyPhoneScan_StationWithoutSecretKey_ThrowsException()
        {
            // Arrange
            var request = new ActiveAttendanceDTO
            (
                "1_1621434000",
                "abc123"
            );
            var user = new User { UserName = "testUser" };
            var station = new Station { StationNumber = 1, SecretKey = null };

            _contextMock.Setup(c => c.Stations)
                .ReturnsDbSet(new[] { station });

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _attendanceService.VerifyPhoneScan(request, user));
        }

        [Test]
        public void VerifyPhoneScan_InvalidToken_ThrowsException()
        {
            // Arrange
            var request = new ActiveAttendanceDTO
            (
                "1_1621434000",
                "abc123"
            );
            var user = new User { UserName = "testUser" };
            var station = new Station { StationNumber = 1, SecretKey = "secretKey" };

            _contextMock.Setup(c => c.Stations)
                .ReturnsDbSet(new[] { station });
            _stationTokenProviderMock.Setup(p => p.ValidateToken(station.SecretKey, request.Message, request.Token))
                .Returns(false);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _attendanceService.VerifyPhoneScan(request, user));
        }

        [Test]
        public async Task VerifyPassiveScan_ValidRequest_AddsAttendanceAndSendsSignalRMessage()
        {
            // Arrange
            var request = new PassiveAttendanceDTO
            (
                1,
                123
            );
            var user = new User { UserName = "testUser" };
            var station = new Station { Id = 1 };
            var now = DateTime.Now;
            var expectedAttendance = new Attendance
            {
                Station = station,
                Time = now,
                User = user
            };
            var expectedStationScanDTO = new StationScanDTO
            {
                StationId = 1,
                ScanTime = now,
                UserInfo = new UserInfoDTO(user.Id, user.UserName)
            };

            _contextMock.Setup(c => c.PassiveTags)
                .ReturnsDbSet(new[] { new PassiveTag { SerialNumber = 123, User = user } });

            _contextMock.Setup(c => c.Stations)
                .ReturnsDbSet(new[] { station });
            _contextMock.Setup(c => c.Attendances)
            .ReturnsDbSet(new List<Attendance> { });
            _currentTimeProviderMock.SetupGet(p => p.Now).Returns(now);
            _mapperMock.Setup(m => m.Map<UserInfoDTO>(user)).Returns(new UserInfoDTO(user.Id, user.UserName));

            // Act
            await _attendanceService.VerifyPassiveScan(request);

            // Assert
            _contextMock.Verify(c => c.Attendances.Add(It.IsAny<Attendance>()), Times.Once);
            _contextMock.Verify(c => c.SaveChanges(), Times.Once);
            _signalrHubMock.Verify(h => h.Clients.Group(request.StationId.ToString()).ReceiveScan(expectedStationScanDTO), Times.Once);
        }

        [Test]
        public void VerifyPassiveScan_InvalidStationOrTag_ThrowsNotFoundException()
        {
            // Arrange
            var request = new PassiveAttendanceDTO
            (
                123,
                1
            );

            _contextMock.Setup(c => c.PassiveTags)
                .ReturnsDbSet(new List<PassiveTag> { });
            _contextMock.Setup(c => c.Stations)
                .ReturnsDbSet(new List<Station> { });

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(() => _attendanceService.VerifyPassiveScan(request));
        }

        [Test]
        public async Task GetAttendanceForDay_ReturnsAttendanceDTOs()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "testUser", Exemptions = (ICollection<Exempt>)new List<Exempt>() };
            var date = new DateOnly(2021, 5, 19);
            var group = new Group { Users = new[] { user } };
            user.Groups = new[] { group };
            var class1 = new Class { Id = 1, Group = group, StartDateTime = new DateTime(2021, 5, 19, 10, 0, 0) };
            var class2 = new Class { Id = 2, Group = group, StartDateTime = new DateTime(2021, 5, 19, 14, 0, 0) };
            var attendance1 = new Attendance { Id = 1, Time = new DateTime(2021, 5, 19, 9, 55, 0), User = user };
            var attendance2 = new Attendance { Id = 2, Time = new DateTime(2021, 5, 19, 14, 10, 0), User = user };

            user.Attendances = new[] { attendance1, attendance2 };
            _contextMock.Setup(c => c.Classes)
                .ReturnsDbSet(new[] { class1, class2 });
            _contextMock.Setup(c => c.SaveChangesAsync(default))
                .ReturnsAsync(1);
            _contextMock.Setup(c => c.Attendances)
                .ReturnsDbSet(new[] { attendance1, attendance2 });
            _contextMock.Setup(c => c.Users)
                .ReturnsDbSet(new[] { user });
            _contextMock.Setup(c => c.Groups)
                .ReturnsDbSet(new[] { group });
            _contextMock.Setup(c => c.Exempt)
                .ReturnsDbSet(new List<Exempt> { });
            _contextMock.Setup(c => c.PassiveTags)
                .ReturnsDbSet(new List<PassiveTag> { });
            _contextMock.Setup(c => c.Stations)
                .ReturnsDbSet(new List<Station> { });
            _mapperMock.Setup(m => m.Map<ClassDTO>(class1))
                .Returns(new ClassDTO(class1.Id, class1.Name, class1.StartDateTime, new UserInfoDTO(3, "Teacher"), new GroupDTO(1, "Group")));
            _mapperMock.Setup(m => m.Map<ClassDTO>(class2))
                .Returns(new ClassDTO(class2.Id, class2.Name, class2.StartDateTime, new UserInfoDTO(3, "Teacher"), new GroupDTO(1, "Group")));

            // Act
            var result = await _attendanceService.GetAttendanceForDay(user, date);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(class1.StartDateTime, result.First().Class.StartDateTime);
            Assert.AreEqual(attendance1.Time, result.First().AuthenticationTime);
            Assert.AreEqual(class2.StartDateTime, result.Last().Class.StartDateTime);
            Assert.AreEqual(attendance2.Time, result.Last().AuthenticationTime);
        }
    }
}
