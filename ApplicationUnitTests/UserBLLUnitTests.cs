using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using SD_340_W22SD_2021_2022___Final_Project_2.BLL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationUnitTests
{
    [TestClass]
    public class UserBLLUnitTests
    {
        private UserBusinessLogic UserBusinessLogic;
        public readonly UserManager<ApplicationUser> UserManager;        

        public UserBLLUnitTests()
        {
            //UserData
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "Test",
                    Id = "9ac002a1-5cc3-499e-bcc7-36849706b9ff",
                    Email = "test@test.it",
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
            mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string userName) => UserManager.Users.SingleOrDefault(u => u.UserName == userName));
            mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "Admin", "Developer", "Project Manager" });
            mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(um => um.GetUsersInRoleAsync(It.IsAny<string>())).ReturnsAsync(new List<ApplicationUser> { new ApplicationUser {UserName = "Developer 1"} });
            UserManager = mockUserManager.Object;
            UserBusinessLogic = new UserBusinessLogic(UserManager);
        }

        [DataRow("9ac002a1-5cc3-499e-bcc7-36849706b9ff", 3)]
        [TestMethod]
        public async Task AssignUserToARoleAsync_ValidInput_UpdateUserRoleWithGivenArugmentAsync(string userId, int expectedCount)
        {
            ApplicationUser currUser = UserBusinessLogic.GetUserByUserId(userId);
            await UserBusinessLogic.AssignUserToARoleAsync(currUser, "Developer");
            List<string> roles = await UserBusinessLogic.GetUserRoles(currUser);
            int ActualCount = roles.Count();
            Assert.AreEqual(expectedCount, ActualCount);
        }

        [DataRow("9ac002a1-5cc3-499e-bcc7-36849706b9ff")]
        [TestMethod]
        public void AssignUserToARoleAsync_InvalidRole_ArgumentException(string userId)
        {
            ApplicationUser currUser = UserBusinessLogic.GetUserByUserId(userId);
            Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await UserBusinessLogic.AssignUserToARoleAsync(currUser, "QA");
            });
        }

        [TestMethod]
        public async Task GetAllUsersWithSpecificRoleAsync_ValidRole_ListOfUsersWithGivenRoleArgumentAsync()
        {
            ApplicationUser currUser = UserBusinessLogic.GetUserByUserId("9ac002a1-5cc3-499e-bcc7-36849706b9ff");
            await UserBusinessLogic.AssignUserToARoleAsync(currUser, "Developer");
            List<ApplicationUser> users = await UserBusinessLogic.GetAllUsersWithSpecificRoleAsync("Developer");
            Assert.IsNotNull(users);
        }

        [DataRow("QA")]
        [TestMethod]
        public void GetAllUsersWithSpecificRoleAsync_InvalidRole_ArgumentException(string invalidRole)
        {
            Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await UserBusinessLogic.GetAllUsersWithSpecificRoleAsync(invalidRole);
            });
        }

        [DataRow("Test")]
        [TestMethod]
        public async Task GetCurrentUserByNameAsync_ValidIdentity_GetUserByNameIdentityAsync(string nameIdentity)
        {
            ApplicationUser user = await UserBusinessLogic.GetCurrentUserByNameAsync(nameIdentity);
            Assert.IsNotNull(user);
        }

        [DataRow("InvalidIdentity")]
        [TestMethod]
        public void GetCurrentUserByNameAsync_InvalidIdentity_NullReferenceException(string nameIdentity)
        {
            Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await UserBusinessLogic.GetCurrentUserByNameAsync(nameIdentity);
            });
        }

        [DataRow("9ac002a1-5cc3-499e-bcc7-36849706b9ff")]
        [TestMethod]
        public void GetUserByUserId_ValidUserId_GetUserByUserId(string userId)
        {
            ApplicationUser user = UserBusinessLogic.GetUserByUserId(userId);
            Assert.IsNotNull(user);
        }

        [DataRow("123127381273")]
        [TestMethod]
        public void GetUserByUserId_InvalidUserId_NullReferenceException(string userId)
        {
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                UserBusinessLogic.GetUserByUserId(userId);
            });
        }

        [TestMethod]
        public async Task GetAllUsersWithoutRoleAsync_ValidInput_GetAllUsersWithoutRoleAsync()
        {
            List<ApplicationUser> applicationUsers = await UserBusinessLogic.GetAllUsersWithoutRoleAsync();
            Assert.IsNotNull(applicationUsers);
        }
    }
}
