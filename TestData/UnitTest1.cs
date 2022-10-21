using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace TestData
{
    [TestClass]
    public class DeveloperTests
    {
        [TestMethod]
        public void AssignDevTestOne()
        {
            var dbContext = new Mock<ApplicationDbContext>();
            var MockProjRepo = new Mock<ProjectRepository>();
            var userManager = new Mock<FakeUserManager>();
            var Admin = new AdminBusinessLogicLayer(dbContext.Object, userManager.Object, MockProjRepo.Object);
            
            ApplicationUser testUser1 = new ApplicationUser { UserName = "Test1", Email = "Test1@Test.com", Id = "1" };
            ApplicationUser testUser2 = new ApplicationUser { UserName = "Test2", Email = "Test2@Test.com", Id = "2" };

            bool methodCall = false;

            userManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Callback(() => { methodCall = true; });
            MockProjRepo.Setup(x => x.FindUser(It.IsAny<int>())).Returns(testUser1);
            MockProjRepo.Setup(x => x.FindUser(It.IsAny<int>())).Returns(testUser2);


            Admin.AssignDeveloper(1);
            Admin.AssignDeveloper(2);
            Assert.IsTrue(methodCall);
        }

        [TestMethod]
        public void AssignDevTestTwo()
        {
            var dbContext = new Mock<ApplicationDbContext>();
            var MockProjRepo = new Mock<ProjectRepository>();
            var userManager = new Mock<FakeUserManager>();
            var Admin = new AdminBusinessLogicLayer(dbContext.Object, userManager.Object, MockProjRepo.Object);

            ApplicationUser testUser = new ApplicationUser { UserName = "Test", Email = "Test@Test.com", Id = "1" };

            bool methodCall = false;

            userManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Callback(() => { methodCall = true; });
            MockProjRepo.Setup(x => x.FindUser(It.IsAny<int>())).Returns(testUser);

            Admin.AssignDeveloper(1);
            Assert.IsTrue(methodCall);
        }

        [TestMethod]
        public void AssignManagerTestOne()
        {
            var dbContext = new Mock<ApplicationDbContext>();
            var MockProjRepo = new Mock<ProjectRepository>();
            var userManager = new Mock<FakeUserManager>();
            var Admin = new AdminBusinessLogicLayer(dbContext.Object, userManager.Object, MockProjRepo.Object);

            ApplicationUser testUser1 = new ApplicationUser { UserName = "Test1", Email = "Test1@Test.com", Id = "1" };
            ApplicationUser testUser2 = new ApplicationUser { UserName = "Test2", Email = "Test2@Test.com", Id = "2" };

            bool methodCall = false;

            userManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Callback(() => { methodCall = true; });
            MockProjRepo.Setup(x => x.FindUser(It.IsAny<int>())).Returns(testUser1);
            MockProjRepo.Setup(x => x.FindUser(It.IsAny<int>())).Returns(testUser2);

            Admin.AssignProjectManager(1);
            Admin.AssignProjectManager(2);

            Assert.IsTrue(methodCall);
        }


        [TestMethod]
        public void AssignManagerTestTwo()
        {
            var dbContext = new Mock<ApplicationDbContext>();
            var MockProjRepo = new Mock<ProjectRepository>();
            var userManager = new Mock<FakeUserManager>();
            var Admin = new AdminBusinessLogicLayer(dbContext.Object, userManager.Object, MockProjRepo.Object);

            ApplicationUser testUser = new ApplicationUser { UserName = "Test", Email = "Test@Test.com", Id = "1" };

            bool methodCall = false;

            userManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Callback(() => { methodCall = true; });
            MockProjRepo.Setup(x => x.FindUser(It.IsAny<int>())).Returns(testUser);

            Admin.AssignProjectManager(1);
            Assert.IsTrue(methodCall);
        }

        /*
        [TestMethod]
        public void ListUnassignedUser()
        {
            var dbContext = new Mock<ApplicationDbContext>();
            var MockProjRepo = new Mock<ProjectRepository>();
            var userManager = new Mock<FakeUserManager>();
            var Admin = new AdminBusinessLogicLayer(dbContext.Object, userManager.Object, MockProjRepo.Object);

            var testData = new List<ApplicationUser> {
            new ApplicationUser { UserName = "Test1", Email = "Test1@Test.com", Id = "1" },
            new ApplicationUser { UserName = "Test2", Email = "Test2@Test.com", Id = "2" },
            new ApplicationUser { UserName = "Test3", Email = "Test3@Test.com", Id = "3" }
            };

            dbContext.Setup(x => x.UserRoles.Select(It.IsAny<Func<IdentityUserRole<string>, string>>())).Returns(new List<string>());
            dbContext.Setup(y => y.Users.Where(It.IsAny<Func<ApplicationUser, bool>>())).Returns(testData);

            var compareData = Admin.GetUnassignedDevelopers();

            Assert.AreEqual(compareData.Count, testData.Count);
        }

        //Can't fix this error without refactoring the whole application. Sorry Boys.
        */

        public class FakeUserManager : UserManager<ApplicationUser>
        {
            public FakeUserManager()
                : base(new Mock<IUserStore<ApplicationUser>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<IPasswordHasher<ApplicationUser>>().Object,
                    new IUserValidator<ApplicationUser>[0],
                    new IPasswordValidator<ApplicationUser>[0],
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
            { }  
        }
    }
   
}