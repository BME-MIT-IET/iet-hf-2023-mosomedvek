using System;
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
using NUnit.Framework;

namespace Tests.Grip.Bll.Services.Tests
{
    [TestFixture]
    public class ClassServiceTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<ApplicationDbContext> _contextMock;
        private Mock<UserManager<User>> _userManagerMock;
        private IClassService _classService;

        [SetUp]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _contextMock = new Mock<ApplicationDbContext>();
            _userManagerMock = MockUserManager<User>();
            _classService = new ClassService(_contextMock.Object, _mapperMock.Object, _userManagerMock.Object);
        }

        [Test]
        public async Task Create_WithValidDto_ReturnsCreatedClassDTO()
        {
            // Arrange
            var dto = new CreateClassDTO
            (
                "ClassName",
                new DateTime(2020, 1, 1, 12, 0, 0),
                1,
                1,
                1
            );

            var group = new Group { Id = dto.GroupId, Name = "Group" };
            var teacher = new User { Id = dto.TeacherId, UserName = "Teacher" };
            var newClass = new Class { Group = group, Teacher = teacher, Name = dto.Name, StartDateTime = dto.StartDateTime, };

            _contextMock.Setup(c => c.Groups.FindAsync(dto.GroupId))
                .ReturnsAsync(group);
            _userManagerMock.Setup(m => m.FindByIdAsync(dto.TeacherId.ToString()))
                .ReturnsAsync(teacher);
            _contextMock.Setup(c => c.Classes.Add(It.IsAny<Class>()));
            _mapperMock.Setup(m => m.Map<Class>(dto))
                .Returns(newClass);
            _mapperMock.Setup(m => m.Map<ClassDTO>(newClass))
                .Returns(new ClassDTO(newClass.Id, newClass.Name, dto.StartDateTime, new UserInfoDTO(teacher.Id, teacher.UserName), new GroupDTO(group.Id, group.Name)));

            // Act
            var result = await _classService.Create(dto);

            // Assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<ClassDTO>(result);

        }

        [Test]
        public void Create_WithInvalidDto_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CreateClassDTO
            (
                "ClassName",
                new DateTime(2020, 1, 1, 12, 0, 0),
                1,
                1,
                1
            );

            _contextMock.Setup(c => c.Groups.FindAsync(dto.GroupId))
                .ReturnsAsync((Group)null);
            _userManagerMock.Setup(m => m.FindByIdAsync(dto.TeacherId.ToString()))
                .ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<BadRequestException>(() => _classService.Create(dto));
        }

        [Test]
        public async Task Delete_WithExistingId_DeletesClass()
        {
            // Arrange
            var id = 1;
            var existingClass = new Class { Id = id };

            _contextMock.Setup(c => c.Classes.FindAsync(id))
                .ReturnsAsync(existingClass);
            _contextMock.Setup(c => c.Classes.Remove(existingClass));
            _contextMock.Setup(c => c.SaveChangesAsync(default))
                .ReturnsAsync(1);
            // Act
            await _classService.Delete(id);

            // Assert
            _contextMock.Verify(c => c.Classes.Remove(existingClass), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public void Delete_WithNonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            var id = 1;

            _contextMock.Setup(c => c.Classes.FindAsync(id))
                .ReturnsAsync((Class)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(() => _classService.Delete(id));
        }
        /*
                [Test]
                public async Task Get_WithExistingId_ReturnsClassDTO()
                {
                    // Arrange
                    var id = 1;
                    var existingClass = new Class { Id = id, Teacher = new User() { Id = 1, UserName = "Teacher" }, Group = new Group() { Name = "Group" } };
                    var classDto = new ClassDTO(id, "ClassName", new DateTime(2020, 1, 1, 12, 0, 0), new UserInfoDTO(1, "Teacher"), new GroupDTO(1, "Group"));

                    _contextMock.Setup(c => c.Classes.Include(c => c.Teacher).Include(c => c.Group).Where(c => c.Id == id))
                        .Returns(MockDbSet(new List<Class> { existingClass }));
                    _mapperMock.Setup(m => m.Map<ClassDTO>(existingClass))
                        .Returns(classDto);
                    _mapperMock.Setup(m => m.Map<UserInfoDTO>(existingClass.Teacher))
                        .Returns(new UserInfoDTO(existingClass.Teacher.Id, existingClass.Teacher.UserName));
                    _mapperMock.Setup(m => m.Map<GroupDTO>(existingClass.Group))
                        .Returns(new GroupDTO(existingClass.Group.Id, existingClass.Group.Name));

                    // Act
                    var result = await _classService.Get(id);

                    // Assert
                    Assert.NotNull(result);
                    Assert.IsInstanceOf<ClassDTO>(result);
                    Assert.AreSame(classDto.Id, result.Id);
                }
                */
        /*
                [Test]
                public void Get_WithNonExistingId_ThrowsNotFoundException()
                {
                    // Arrange
                    var id = 1;

                    _contextMock.Setup(c => c.Classes.Include(c => c.Teacher).Include(c => c.Group).Where(c => c.Id == id))
                        .Returns(MockDbSet(new List<Class> { }));

                    // Act & Assert
                    Assert.ThrowsAsync<NotFoundException>(() => _classService.Get(id));
                }
                */
        /*
                [Test]
                public async Task GetClassesForUserOnDay_WithValidData_ReturnsListOfClassDTOs()
                {
                    // Arrange
                    var user = new User();
                    var date = new DateOnly(2023, 5, 19);
                    var existingClasses = new List<Class> { new Class(), new Class() };

                    _contextMock.Setup(c => c.Classes.Include(c => c.Teacher).Include(c => c.Group).ThenInclude(g => g.Users).Where(c => c.StartDateTime.Year == date.Year && c.StartDateTime.Month == date.Month && c.StartDateTime.Day == date.Day))
                        .Returns(MockDbSet(existingClasses));
                    _mapperMock.Setup(m => m.Map<List<ClassDTO>>(existingClasses))
                        .Returns(existingClasses.Select(c => new ClassDTO(c.Id, c.Name, c.StartDateTime, new UserInfoDTO(c.Teacher.Id, c.Teacher?.UserName ?? "Name"), new GroupDTO(c.Group.Id, c.Group.Name))).ToList());

                    // Act
                    var result = await _classService.GetClassesForUserOnDay(user, date);

                    // Assert
                    Assert.NotNull(result);
                    Assert.IsInstanceOf<IEnumerable<ClassDTO>>(result);
                    Assert.AreEqual(existingClasses.Count, result.Count());
                }
        */
        [Test]
        public async Task Update_WithExistingDto_UpdatesClass()
        {
            // Arrange
            var existingClass = new Class();
            var dto = new ClassDTO(1, "ClassName", new DateTime(2020, 1, 1, 12, 0, 0), new UserInfoDTO(1, "Teacher"), new GroupDTO(1, "Group"));

            _contextMock.Setup(c => c.Classes.Find(dto.Id))
                .Returns(existingClass);
            _mapperMock.Setup(m => m.Map(dto, existingClass));
            _contextMock.Setup(c => c.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            await _classService.Update(dto);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public void Update_WithNonExistingDto_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new ClassDTO(1, "ClassName", new DateTime(2020, 1, 1, 12, 0, 0), new UserInfoDTO(1, "Teacher"), new GroupDTO(1, "Group"));

            _contextMock.Setup(c => c.Classes.Find(dto.Id))
                .Returns((Class)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(() => _classService.Update(dto));
        }

        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private static DbSet<TEntity> MockDbSet<TEntity>(IEnumerable<TEntity> data) where TEntity : class
        {
            var queryableData = data.AsQueryable();
            var mockDbSet = new Mock<DbSet<TEntity>>();
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());
            return mockDbSet.Object;
        }
    }
}
