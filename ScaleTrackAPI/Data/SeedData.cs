using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.DTOs.Issue;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Database
{
    public static class SeedData
    {
        public static async Task Initialize(AppDbContext context, UserManager<User> userManager, IConfiguration config)
        {
            context.Database.EnsureCreated();

            if (!userManager.Users.Any())
            {
                var pepper = config["Security:PasswordPepper"] ?? "";

                var admin = new User
                {
                    UserName = "admin@scaletrack.local",
                    Email = "admin@scaletrack.local",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User",
                    Role = UserRole.Admin.ToString()
                };
                await userManager.CreateAsync(admin, "Passw0rd!" + pepper);
                await userManager.AddClaimAsync(admin, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, admin.Role.ToString()));

                var dev = new User
                {
                    UserName = "dev@scaletrack.local",
                    Email = "dev@scaletrack.local",
                    EmailConfirmed = true,
                    FirstName = "Dev",
                    LastName = "User",
                    Role = UserRole.Developer.ToString()
                };
                await userManager.CreateAsync(dev, "Passw0rd!" + pepper);
                await userManager.AddClaimAsync(dev, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, dev.Role.ToString()));

                var viewer = new User
                {
                    UserName = "viewer@scaletrack.local",
                    Email = "viewer@scaletrack.local",
                    EmailConfirmed = true,
                    FirstName = "Viewer",
                    LastName = "User",
                    Role = UserRole.Viewer.ToString()
                };
                await userManager.CreateAsync(viewer, "Passw0rd!" + pepper);
                await userManager.AddClaimAsync(viewer, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, viewer.Role.ToString()));
            }

            if (!context.Issues.Any())
            {
                context.Issues.AddRange(
                    new Issue { Title = "Login button not working", Description = "The login button does nothing", Type = IssueType.Bug, Priority = IssuePriority.High, Status = IssueStatus.Open, CreatedAt = DateTime.UtcNow },
                    new Issue { Title = "Add dark mode", Description = "Implement dark mode feature", Type = IssueType.Feature, Priority = IssuePriority.Low, Status = IssueStatus.Open, CreatedAt = DateTime.UtcNow }
                );
                context.SaveChanges();
            }

            if (!context.Tags.Any())
            {
                context.Tags.AddRange(
                    new Tag { Name = "UI" },
                    new Tag { Name = "Backend" },
                    new Tag { Name = "Feature" }
                );
                context.SaveChanges();
            }

            if (!context.IssueTags.Any())
            {
                var issue1 = context.Issues.First();
                var tag1 = context.Tags.First();
                context.IssueTags.Add(new IssueTag { IssueId = issue1.Id, TagId = tag1.Id });
                context.SaveChanges();
            }

            if (!context.Comments.Any())
            {
                var issue = context.Issues.First();
                var user = await userManager.Users.FirstAsync();

                context.Comments.Add(new Comment { IssueId = issue.Id, UserId = user.Id, Content = "I can reproduce this issue on Chrome.", CreatedAt = DateTime.UtcNow });
                context.SaveChanges();
            }

            if (!context.AuditTrails.Any())
            {
                var user = await userManager.Users.FirstAsync();

                context.AuditTrails.Add(new AuditTrail
                {
                    EntityName = "Issue",
                    EntityId = 1,
                    Action = "Created",
                    ChangedBy = user.Id,
                    Changes = "Initial issue created",
                    ApprovedBy = user.Id,
                    ApprovedAt = DateTime.UtcNow
                });
                context.SaveChanges();
            }

            if (!context.MenuItems.Any())
            {
                context.MenuItems.AddRange(
                    new MenuItem { Name = "Dashboard", Path = "/dashboard", Icon = "home", Roles = "Admin,Developer,Viewer" },
                    new MenuItem { Name = "Manage Users", Path = "/users", Icon = "users", Roles = "Admin" },
                    new MenuItem { Name = "Reports", Path = "/reports", Icon = "bar-chart", Roles = "Admin,Developer" },
                    new MenuItem { Name = "My Tasks", Path = "/tasks", Icon = "check-square", Roles = "Developer,Viewer" }
                );
                context.SaveChanges();
            }

        }
    }
}

