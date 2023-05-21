using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grip.Bll.DTO;
using Grip.Bll.Exceptions;
using Grip.Bll.Services;
using Grip.Bll.Services.Interfaces;
using Grip.DAL;
using Grip.DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests.Grip.Bll.Services
{
    [TestFixture]
    public class ExemptServiceTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<ApplicationDbContext> _contextMock;
        private Mock<UserManager<User>> _userManagerMock;
        private IExemptService _exemptService;

        [SetUp]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _contextMock = new Mock<ApplicationDbContext>();
            _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _exemptService = new ExemptService(_mapperMock.Object, _contextMock.Object, _userManagerMock.Object);
        }


        //Creates don't work with mock dbsets because they cannot read back supposedly saved records
        /*
                [Test]
                public async Task Create_ValidDtoAndTeacher_ReturnsCreatedExempt()
                {
                    // Arrange
                    var dto = new CreateExemptDTO(1, new DateTime(2021, 1, 1), new DateTime(2021, 1, 2));
                    var student = new User() { Id = 1, UserName = "student" };
                    var teacher = new User() { Id = 2, UserName = "teacher" };
                    Exempt exemptModel = new Exempt() { Id = 1, IssuedById = teacher.Id, IssuedToId = dto.IssuedToId, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo };

                    _mapperMock.Setup(m => m.Map<Exempt>(dto)).Returns(new Exempt() { IssuedById = teacher.Id, IssuedToId = dto.IssuedToId, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
                    _contextMock.Setup(c => c.Exempt).ReturnsDbSet(new List<Exempt>());
                    _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
                    _contextMock.Setup(c => c.Users).ReturnsDbSet(new List<User>() { teacher, student });
                    _mapperMock.Setup(m => m.Map<ExemptDTO>(It.IsAny<Exempt>())).Returns(new ExemptDTO(exemptModel.Id, new UserInfoDTO(teacher.Id, teacher.UserName), new UserInfoDTO(dto.IssuedToId, "student"), dto.ValidFrom, dto.ValidTo));

                    // Act
                    var result = await _exemptService.Create(dto, teacher);

                    // Assert
                    Assert.That(result.IssuedBy.Id, Is.EqualTo(teacher.Id));
                    Assert.That(result.IssuedTo.Id, Is.EqualTo(dto.IssuedToId));
                    Assert.That(result.ValidFrom, Is.EqualTo(dto.ValidFrom));
                    Assert.That(result.ValidTo, Is.EqualTo(dto.ValidTo));
                    _contextMock.Verify(c => c.Exempt.Add(It.IsAny<Exempt>()), Times.Once);
                    _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
                }*/

        [Test]
        public void Create_NotImplemented_ThrowsNotImplementedException()
        {
            // Arrange
            var dto = new CreateExemptDTO(1, new System.DateTime(2021, 1, 1), new System.DateTime(2021, 1, 2));

            // Act & Assert
            Assert.ThrowsAsync<NotImplementedException>(async () => await _exemptService.Create(dto));
        }

        [Test]
        public async Task Delete_ValidId_DeletesExempt()
        {
            // Arrange
            var id = 1;
            var exempt = new Exempt() { Id = id };

            _contextMock.Setup(c => c.Exempt).ReturnsDbSet(new List<Exempt>() { exempt });
            _contextMock.Setup(c => c.Exempt.FindAsync(id)).ReturnsAsync(exempt);
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _exemptService.Delete(id);

            // Assert
            _contextMock.Verify(c => c.Exempt.FindAsync(id), Times.Once);
            _contextMock.Verify(c => c.Exempt.Remove(exempt), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public void Delete_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var id = 1;

            _contextMock.Setup(c => c.Exempt.FindAsync(id)).ReturnsAsync((Exempt)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _exemptService.Delete(id));
        }

        [Test]
        public async Task Get_ValidId_ReturnsExempt()
        {
            // Arrange
            var id = 1;
            var exemptModel = new Exempt() { Id = id };
            var exemptDTO = new ExemptDTO(exemptModel.Id, new UserInfoDTO(1, "teacher"), new UserInfoDTO(2, "student"), new System.DateTime(2021, 1, 1), new System.DateTime(2021, 1, 2));

            _contextMock.Setup(c => c.Exempts.FindAsync(It.IsAny<int>())).ReturnsAsync(exemptModel);
            /*_contextMock.Setup(c => c.Users).ReturnsDbSet(new List<User>() { new User() { Id = 1, UserName = "teacher" }, new User() { Id = 2, UserName = "student" } });
            _contextMock.Setup(c => c.Exempt).ReturnsDbSet(new List<Exempt>() { exemptModel });*/
            _mapperMock.Setup(m => m.Map<ExemptDTO>(It.IsAny<Exempt>())).Returns(exemptDTO);

            // Act
            var result = await _exemptService.Get(id);

            // Assert
            Assert.AreEqual(exemptDTO, result);
            _contextMock.Verify(c => c.Exempts.FindAsync(id), Times.Once);
        }

        [Test]
        public async Task GetAll_ReturnsAllExempts()
        {
            // Arrange
            var exempt1 = new Exempt() { Id = 1, IssuedById = 1, IssuedToId = 2, ValidFrom = new DateTime(2021, 1, 1), ValidTo = new DateTime(2021, 1, 2) };
            var exempt2 = new Exempt() { Id = 2, IssuedById = 1, IssuedToId = 2, ValidFrom = new DateTime(2021, 1, 1), ValidTo = new DateTime(2021, 1, 2) };
            var exempts = new List<Exempt> { exempt1, exempt2 };

            _contextMock.Setup(c => c.Exempt)
                .ReturnsDbSet(exempts);
            _mapperMock.Setup(m => m.Map<ExemptDTO>(It.IsAny<Exempt>()))
                .Returns<Exempt>(e => new ExemptDTO(e.Id, new UserInfoDTO(e.IssuedById, "teacher"), new UserInfoDTO(e.IssuedToId, "student"), e.ValidFrom, e.ValidTo));

            // Act
            var result = await _exemptService.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task Delete_ExistingExempt_ReturnsDeletedExempt()
        {
            // Arrange
            var id = 1;
            var exempt = new Exempt();

            _contextMock.Setup(c => c.Exempt.FindAsync(id)).ReturnsAsync(exempt);
            _contextMock.Setup(c => c.Exempt.Remove(exempt));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _exemptService.Delete(id);

            // Assert
            _contextMock.Verify(c => c.Exempt.FindAsync(id), Times.Once);
            _contextMock.Verify(c => c.Exempt.Remove(exempt), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public void Delete_NonExistingExempt_ThrowsNotFoundException()
        {
            // Arrange
            var id = 1;

            _contextMock.Setup(c => c.Exempt.FindAsync(id)).ReturnsAsync((Exempt)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _exemptService.Delete(id));
            _contextMock.Verify(c => c.Exempt.FindAsync(id), Times.Once);
            _contextMock.Verify(c => c.Exempt.Remove(It.IsAny<Exempt>()), Times.Never);
            _contextMock.Verify(c => c.SaveChangesAsync(default(CancellationToken)), Times.Never);
        }
    }
}