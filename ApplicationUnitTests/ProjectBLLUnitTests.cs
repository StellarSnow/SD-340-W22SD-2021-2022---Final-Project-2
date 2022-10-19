using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using SD_340_W22SD_2021_2022___Final_Project_2.BLL;
using SD_340_W22SD_2021_2022___Final_Project_2.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ApplicationUnitTests
{
    [TestClass]
    public class ProjectBLLUnitTests
    {
        private ProjectBusinessLogic ProjectBusinessLogic;
        private UserBusinessLogic UserBusinessLogic;
        public readonly UserManager<ApplicationUser> UserManager;

        public ProjectBLLUnitTests()
        {
            //UserData
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "Test",
                    Id = "9ac002a1-5cc3-499e-bcc7-36849706b9ff",
                    Email = "test@test.it"
                }
            }.AsQueryable();

            var mockUserManager = new Mock<MockUserManager>();

            mockUserManager.Setup(x => x.Users)
                .Returns(users);
            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => UserManager.Users.SingleOrDefault(u => u.Id == userId));

            UserManager = mockUserManager.Object;
            UserBusinessLogic = new UserBusinessLogic(UserManager);

            // Project DbSet
            var projData = new List<Project>
            {
                new Project{Id = 1, Name = "Project 1", Developers = UserManager.Users.ToList()},
                new Project{Id = 2, Name = "Project 2", Developers = UserManager.Users.ToList()},
                new Project{Id = 3, Name = "Project 3", Developers = UserManager.Users.ToList()},
            }.AsQueryable();

            var ProjMockDbSet = new Mock<DbSet<Project>>();

            ProjMockDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projData.Provider);
            ProjMockDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projData.Expression);
            ProjMockDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projData.ElementType);
            ProjMockDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projData.GetEnumerator());

            var projMockContext = new Mock<ApplicationDbContext>();
            projMockContext.Setup(m => m.Project).Returns(ProjMockDbSet.Object);

            ProjectBusinessLogic = new ProjectBusinessLogic(new ProjectRepository(projMockContext.Object), UserManager);
            
        }

        [DataRow("9ac002a1-5cc3-499e-bcc7-36849706b9ff", 3)]
        [TestMethod]
        public async Task GetAllProjectsByDeveloperAsync_ValidInput_ReturnsListOfProjectsByDeveloperAsync(string devId, int expectedProjCount)
        {
            ApplicationUser currUser = UserBusinessLogic.GetUserByUserId(devId);

            List<Project> allProjByDev = await ProjectBusinessLogic.GetAllProjectsByDeveloperAsync(currUser.Id);

            Assert.AreEqual(expectedProjCount, allProjByDev.Count);
        }

        [DataRow("sadocsdkjcapwvqie")]
        [TestMethod]
        public async Task GetAllProjectsByDeveloperAsync_UserNotFound_ThrowsNullReferenceException(string devId)
        {
            Assert.ThrowsExceptionAsync<NullReferenceException>(async () =>
            {
                await ProjectBusinessLogic.GetAllProjectsByDeveloperAsync(devId);
            });
        }

        [DataRow(3)]
        [TestMethod]
        public void GetAllProjects_Valid_ReturnsAllProjects(int expectedCount)
        {
            int actualCount = ProjectBusinessLogic.GetAllProjects().Count;

            Assert.AreEqual(expectedCount, actualCount);
        }

        [DataRow(1)]
        [TestMethod]
        public void GetProjectDetails_ValidInput_ReturnsAProjectByProjectId(int projId)
        {
            Project currProj = ProjectBusinessLogic.GetProjectDetails(projId);
            Assert.IsTrue(currProj != null);
        }

        [DataRow(5)]
        [TestMethod]
        public void GetProjectDetails_ProjectNotFound_ThrowsNullReferenceException(int projId)
        {
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                ProjectBusinessLogic.GetProjectDetails(projId);
            });
        }

        [DataRow(3)]
        [TestMethod]
        public void CreateProject_ValidInput_CreatesNewProjectAndAddsToProjects(int expectedCount)
        {
            ProjectBusinessLogic.CreateProject(new Project { Name = "New proj", Developers = UserManager.Users.ToList(), Id = 4 });
            int actualCount = ProjectBusinessLogic.GetAllProjects().Count;

            Assert.AreEqual(expectedCount, actualCount);

        }

    }
}
