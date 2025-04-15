using Microsoft.EntityFrameworkCore;
using Project.Domain.Constants;
using Project.Domain.Entities;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Data.Seeds;

public static class SeedData
{
    public static async Task InsertSeedsAsync(ApplicationDbContext context)
    {
        if (!await context.Role.AnyAsync())
        {
            context.Role.AddRange(
                new Role
                (
                    id: RoleConstants.Admin,
                    name: "Admin"
                ),
                new Role
                (
                    id: RoleConstants.User,
                    name: "User"
                )
            );

            await context.SaveChangesAsync();
        }

        if (!await context.User.AnyAsync())
        {
            context.User.AddRange(
                new User
                (
                    id: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    username: "jamcoreadmin",
                    password: "123",
                    email: "admin@jamcore.com",
                    roleId: RoleConstants.Admin
                ),
                new User
                (
                    id: Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    username: "jamcoreuser",
                    password: "123",
                    email: "user@jamcore.com",
                    roleId: RoleConstants.User
                )
            );

            await context.SaveChangesAsync();
        }
    }
}
