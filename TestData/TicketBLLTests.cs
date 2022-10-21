using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using SD_340_W22SD_2021_2022___Final_Project_2.Models.ViewModels;
using static TestData.DeveloperTests;

namespace TestData
{
    [TestClass]
    public class TicketBLLTests
    {
        public static List<ApplicationUser> _users { get; set; }
        public static TicketBusinessLogic _ticketBLL { get; set; }
        public static FakeUserManager _userManager { get; set; }
        public static ApplicationDbContext _dbContext { get; set; }


        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                                .UseInMemoryDatabase(databaseName: "TestDatabase")
                                .Options;

            _dbContext = new ApplicationDbContext(options);


            ApplicationUser pmUser = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "pm@fp2tests.com",
                Email = "pm@fp2tests.com"
            };

            ApplicationUser devUser1 = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "dev1@fp2tests.com",
                Email = "dev1@fp2tests.com"
            };

            ApplicationUser devUser2 = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "dev2@fp2tests.com",
                Email = "dev2@fp2tests.com"
            };

            _users = new List<ApplicationUser>() { pmUser, devUser1, devUser2 };

            IdentityRole pmRole = new IdentityRole("Project Manager");
            IdentityRole devRole = new IdentityRole("Developer");

            _dbContext.Roles.Add(pmRole);
            _dbContext.Roles.Add(devRole);
            _dbContext.Users.Add(pmUser);
            _dbContext.Users.Add(devUser1);
            _dbContext.Users.Add(devUser2);

            _dbContext.SaveChanges();

            Project project = new Project()
            {
                Id = 1,
                Name = "Test Project",
                Developers = new List<ApplicationUser>() { devUser1 },
            };

            devUser1.Projects.Add(project);

            _dbContext.Project.Add(project);

            _dbContext.SaveChanges();

            IUserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(_dbContext);
            
            _userManager = new FakeUserManager(userStore);

            _userManager.AddToRoleAsync(pmUser, "Project Manager");

            _userManager.AddToRoleAsync(devUser1, "Developer");

            _userManager.AddToRoleAsync(devUser1, "Developer");


            _ticketBLL = new TicketBusinessLogic(
                new TicketRepository(_dbContext),
                new ProjectRepository(_dbContext),
                _userManager
            );
        }

        [DataRow(null, 1, new string[] { }, Priority.high)]
        [TestMethod]
        public async Task TestCreateTicketFailure(Ticket ticket, int projectId, string[] taskOwnerIds, Priority priority)
        {
            CreateTicketBLLViewModel result = await _ticketBLL.CreateTicket(ticket, projectId, taskOwnerIds, priority);

            Assert.IsFalse(result.Succeeded);
        }

        [TestMethod]
        public async Task TestCreateTicketSuccess()
        {
            var pm = await _userManager.FindByIdAsync(_users[0].Id);
            var dev1 = await _userManager.FindByIdAsync(_users[1].Id);
            var dev2 = await _userManager.FindByIdAsync(_users[2].Id);

            var pmRoles = _userManager.GetRolesAsync(pm);
            var dev1Roles = _userManager.GetRolesAsync(dev1);
            var dev2Roles = _userManager.GetRolesAsync(dev2);

            Ticket ticketInputModel = new Ticket()
            {
                Id = 1,
                Name = "Test Ticket 1",
                ProjectId = 1,
                Completed = false,
                Hours = 2,
                Priority = Priority.high
            };

            int projectId = 1;

            string[] taskOwnerIds = new string[] { _users[1].Id };

            CreateTicketBLLViewModel result = await _ticketBLL.CreateTicket(ticketInputModel, projectId, taskOwnerIds, Priority.high);

            Assert.IsTrue(result.Succeeded);
        }
    }
}
