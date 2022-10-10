using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapGet("/details/{projectID}", (string projectID, ApplicationDbContext db) => {
    ProjectRepository repo = new ProjectRepository(db);

    try
    {
        return repo.Get(int.Parse(projectID));
    }
    catch (Exception e)
    {
        return null;
    }
});

app.MapGet("/Comment/CommentsForTask?ticketId={ticketID}", (string ticketID, ApplicationDbContext db) =>
{
    TicketRepository repo = new TicketRepository(db);
    try    
    {
        return repo.Get(int.Parse(ticketID));
    }
    catch (Exception e)
    {
        return null;
    }
});

app.MapGet("/Project/Create/{name}", (string name, ApplicationDbContext db) =>
{
    AccountBusinessLogic accountBusinessLogic = new AccountBusinessLogic(new ProjectRepository(db));

    try
    {
        accountBusinessLogic.CreateProject(name);
        return Results.Ok();
    }
    catch (Exception e)
    {
        return Results.Problem();
    }

});

app.MapGet("/Admin/UnassignedDeveloperCheck", async (ApplicationDbContext db) =>
{
    ProjectRepository projectRepository = new ProjectRepository(db);

    try
    {
        return projectRepository.GetAllUsers();
    }
    catch (Exception e)
    {
        return null;
    }

    

});

app.MapGet("/Admin/UnassignedDeveloperCheck/AssignDeveloper/{id}", (string id, ApplicationDbContext db) =>
{
    AccountBusinessLogic accountBusinessLogic = new AccountBusinessLogic(new ProjectRepository(db));

    try
    {
        accountBusinessLogic.AssignDeveloper(int.Parse(id));
        return Results.Ok();
    }
    catch (Exception e)
    {
        return Results.Problem();
    }
});

app.MapGet("/Admin/UnassignedDeveloperCheck/AssignProjectManager/{id}", (string id, ApplicationDbContext db) =>
{
    AccountBusinessLogic accountBusinessLogic = new AccountBusinessLogic(new ProjectRepository(db));

    try
    {
        accountBusinessLogic.AssignProjectManager(int.Parse(id));
        return Results.Ok();
    }
    catch (Exception e)
    {
        return Results.Problem();
    }
});

app.Run();
