using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using SD_340_W22SD_2021_2022___Final_Project_2.Models.ViewModels;
using System.Security.Claims;
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


        [TestInitialize]
        public void InitializeTest()
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

        [TestCleanup]
        public void CleanUpTests()
        {
            _dbContext.Database.EnsureDeleted();
            _users = null;
            _ticketBLL = null;
            _userManager = null;
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

        [DataRow(1, 1)]
        [TestMethod]
        public async Task TestAddToWatchListAsyncFailure(int projectId, int ticketId)
        {
            Ticket ticketInputModel = new Ticket()
            {
                Id = ticketId,
                Name = "Test Ticket 1",
                ProjectId = projectId,
                Completed = false,
                Hours = 2,
                Priority = Priority.high
            };

            // user at index 1 is assigned to the project 1
            string[] taskOwnerIds = new string[] { _users[1].Id };

            CreateTicketBLLViewModel createTicket = await _ticketBLL.CreateTicket(ticketInputModel, projectId, taskOwnerIds, Priority.high);

            Assert.IsTrue(createTicket.Succeeded);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, _users[2].UserName),
                new Claim(ClaimTypes.NameIdentifier, _users[2].Id),
                new Claim(ClaimTypes.Email, _users[2].Email)
            };
            
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Mock");
            ClaimsPrincipal user = new ClaimsPrincipal(identity);

            Assert.IsTrue(user.Identity.IsAuthenticated);

            AddToWatchListBLLViewModel addToWatchList = await _ticketBLL.AddToWatchListAsync(user, projectId, ticketId);

            Assert.IsTrue(addToWatchList.Unauthorized);
        }

        [DataRow(1, 1)]
        [TestMethod]
        public async Task TestAddToWatchListAsyncSuccess(int projectId, int ticketId)
        {
            Project project = await _dbContext.Project.FirstOrDefaultAsync(p => p.Id == projectId);

            Assert.IsNotNull(project);

            Ticket ticketInputModel = new Ticket()
            {
                Id = ticketId,
                Name = "Test Ticket 1",
                ProjectId = projectId,
                Completed = false,
                Hours = 2,
                Priority = Priority.high
            };

            // user at index 1 is assigned to the project 1
            string[] taskOwnerIds = new string[] { _users[1].Id };

            CreateTicketBLLViewModel createTicket = await _ticketBLL.CreateTicket(ticketInputModel, projectId, taskOwnerIds, Priority.high);

            Assert.IsTrue(createTicket.Succeeded);

            ApplicationUser userToBeAddedToWatchList = await _userManager.FindByIdAsync(_users[2].Id);

            Assert.IsNotNull(userToBeAddedToWatchList);

            project.Developers.Add(userToBeAddedToWatchList);

            userToBeAddedToWatchList.Projects.Add(project);

            await _dbContext.SaveChangesAsync();

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userToBeAddedToWatchList.UserName),
                new Claim(ClaimTypes.NameIdentifier, userToBeAddedToWatchList.Id),
                new Claim(ClaimTypes.Email, userToBeAddedToWatchList.Email)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "Mock");
            ClaimsPrincipal userClaimsPrincipal = new ClaimsPrincipal(identity);

            Assert.IsTrue(userClaimsPrincipal.Identity.IsAuthenticated);

            AddToWatchListBLLViewModel addToWatchList = await _ticketBLL.AddToWatchListAsync(userClaimsPrincipal, projectId, ticketId);

            Assert.IsTrue(addToWatchList.Succeeded);
        }

        [DataRow(1, 1)]
        [TestMethod]
        public async Task TestToggleTicketAsyncFailure(int projectId, int ticketId)
        {
            Ticket ticketInputModel = new Ticket()
            {
                Id = ticketId,
                Name = "Test Ticket 1",
                ProjectId = projectId,
                Completed = false,
                Hours = 2,
                Priority = Priority.high
            };

            // user at index 1 is assigned to the project 1
            string[] taskOwnerIds = new string[] { _users[1].Id };

            CreateTicketBLLViewModel createTicket = await _ticketBLL.CreateTicket(ticketInputModel, projectId, taskOwnerIds, Priority.high);

            Assert.IsTrue(createTicket.Succeeded);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, _users[2].UserName),
                new Claim(ClaimTypes.NameIdentifier, _users[2].Id),
                new Claim(ClaimTypes.Email, _users[2].Email)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "Mock");
            ClaimsPrincipal userClaimsPrincipal = new ClaimsPrincipal(identity);

            Assert.IsTrue(userClaimsPrincipal.Identity.IsAuthenticated);

            ToggleTicketBLLViewModel toggleTicket = await _ticketBLL.ToggleTicketAsync(userClaimsPrincipal, ticketId);

            Assert.IsTrue(toggleTicket.Unauthorized);
        }

        [DataRow(1, 1)]
        [TestMethod]
        public async Task TestToggleTicketAsyncSuccess(int projectId, int ticketId)
        {
            Ticket ticketInputModel = new Ticket()
            {
                Id = ticketId,
                Name = "Test Ticket 1",
                ProjectId = projectId,
                Completed = false,
                Hours = 2,
                Priority = Priority.high
            };

            // user at index 1 is assigned to the project 1
            string[] taskOwnerIds = new string[] { _users[1].Id };

            CreateTicketBLLViewModel createTicket = await _ticketBLL.CreateTicket(ticketInputModel, projectId, taskOwnerIds, Priority.high);

            Assert.IsTrue(createTicket.Succeeded);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, _users[1].UserName),
                new Claim(ClaimTypes.NameIdentifier, _users[1].Id),
                new Claim(ClaimTypes.Email, _users[1].Email)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "Mock");
            ClaimsPrincipal userClaimsPrincipal = new ClaimsPrincipal(identity);

            Assert.IsTrue(userClaimsPrincipal.Identity.IsAuthenticated);

            ToggleTicketBLLViewModel toggleTicket = await _ticketBLL.ToggleTicketAsync(userClaimsPrincipal, ticketId);

            Assert.IsTrue(toggleTicket.Succeeded);
        }

        [DataRow(1, 1, 1)]
        [TestMethod]
        public async Task TestChangeRequiredHoursAsyncFailure(int projectId, int ticketId, int requiredHours)
        {
            Ticket ticketInputModel = new Ticket()
            {
                Id = ticketId,
                Name = "Test Ticket 1",
                ProjectId = projectId,
                Completed = false,
                Hours = 2,
                Priority = Priority.high
            };

            // user at index 1 is assigned to the project 1
            string[] taskOwnerIds = new string[] { _users[1].Id };

            CreateTicketBLLViewModel createTicket = await _ticketBLL.CreateTicket(ticketInputModel, projectId, taskOwnerIds, Priority.high);

            Assert.IsTrue(createTicket.Succeeded);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, _users[2].UserName),
                new Claim(ClaimTypes.NameIdentifier, _users[2].Id),
                new Claim(ClaimTypes.Email, _users[2].Email)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "Mock");
            ClaimsPrincipal userClaimsPrincipal = new ClaimsPrincipal(identity);

            Assert.IsTrue(userClaimsPrincipal.Identity.IsAuthenticated);

            ChangeRequiredHoursAsyncBLLViewModel changeRequiredHours = await _ticketBLL.ChangeRequiredHoursAsync(userClaimsPrincipal, ticketId, requiredHours);

            Assert.IsTrue(changeRequiredHours.Unauthorized);
        }

        [DataRow(1, 1, 1)]
        [TestMethod]
        public async Task TestChangeRequiredHoursAsyncSuccess(int projectId, int ticketId, int requiredHours)
        {
            Ticket ticketInputModel = new Ticket()
            {
                Id = ticketId,
                Name = "Test Ticket 1",
                ProjectId = projectId,
                Completed = false,
                Hours = 2,
                Priority = Priority.high
            };

            // user at index 1 is assigned to the project 1
            string[] taskOwnerIds = new string[] { _users[1].Id };

            CreateTicketBLLViewModel createTicket = await _ticketBLL.CreateTicket(ticketInputModel, projectId, taskOwnerIds, Priority.high);

            Assert.IsTrue(createTicket.Succeeded);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, _users[1].UserName),
                new Claim(ClaimTypes.NameIdentifier, _users[1].Id),
                new Claim(ClaimTypes.Email, _users[1].Email)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "Mock");
            ClaimsPrincipal userClaimsPrincipal = new ClaimsPrincipal(identity);

            Assert.IsTrue(userClaimsPrincipal.Identity.IsAuthenticated);

            ChangeRequiredHoursAsyncBLLViewModel changeRequiredHours = await _ticketBLL.ChangeRequiredHoursAsync(userClaimsPrincipal, ticketId, requiredHours);

            Assert.IsTrue(changeRequiredHours.Succeeded);
        }

        [DataRow(1, 1)]
        [TestMethod]
        public async Task TestCommentsOnTaskSuccess(int projectId, int ticketId)
        {
            Ticket ticketInputModel = new Ticket()
            {
                Id = ticketId,
                Name = "Test Ticket 1",
                ProjectId = projectId,
                Completed = false,
                Hours = 2,
                Priority = Priority.high
            };

            // user at index 1 is assigned to the project 1
            string[] taskOwnerIds = new string[] { _users[1].Id };

            CreateTicketBLLViewModel createTicket = await _ticketBLL.CreateTicket(ticketInputModel, projectId, taskOwnerIds, Priority.high);

            Assert.IsTrue(createTicket.Succeeded);

            Ticket ticket = await _dbContext.Ticket.FirstOrDefaultAsync(t => t.Id == ticketId);

            Assert.IsNotNull(ticket);

            Comment comment = new Comment()
            {
                Id = 1,
                Content = "Test comment",
                TicketId = ticketId,
                UserId = _users[1].Id
            };


            _dbContext.Comment.Add(comment);
            ticket.Comment.Add(comment);

            await _dbContext.SaveChangesAsync();

            ICollection<Comment> comments = _ticketBLL.CommentsOnTask(ticketId);

            Assert.IsNotNull(comments);
            Assert.AreEqual(comments.Count, 1);
            Assert.AreEqual(comments.First().Id, comment.Id);
            Assert.AreEqual(comments.First().Content, comment.Content);
            Assert.AreEqual(comments.First().TicketId, comment.TicketId);
            Assert.AreEqual(comments.First().UserId, comment.UserId);
        }
    }
}
