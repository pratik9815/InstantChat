
using InstantChat.Application;
using InstantChat.Domain.Entities;
using InstantChat.Infrastructure;
using InstantChat.Infrastructure.Hubs;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

//app.MapStaticAssets();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapHub<ChatHub>("/chatHub");

// Seed database
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (!userManager.Users.Any())
    {
        var testUsers = new[]
        {
            new { Email = "alice@test.com", DisplayName = "Alice", Password = "test123" },
            new { Email = "bob@test.com", DisplayName = "Bob", Password = "test123" },
            new { Email = "charlie@test.com", DisplayName = "Charlie", Password = "test123" }
        };

        foreach (var testUser in testUsers)
        {
            var user = new ApplicationUser(testUser.Email, testUser.DisplayName);
            await userManager.CreateAsync(user, testUser.Password);
        }
    }
}

app.Run();
