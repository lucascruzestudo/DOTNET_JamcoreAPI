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
                    username: "administrator",
                    password: "123-!@#-123-!@#",
                    email: "admin@system.com",
                    roleId: RoleConstants.Admin
                ),
                new User
                (
                    username: "lucascruzestudo",
                    password: "123-!@#-123-!@#",
                    email: "lucascruzestudo@gmail.com",
                    roleId: RoleConstants.Admin
                ),
                new User
                (
                    username: "lucascruztrabalho",
                    password: "123-!@#-123-!@#",
                    email: "lucascruztrabalho@gmail.com",
                    roleId: RoleConstants.User
                )
            );

            await context.SaveChangesAsync();
        }
    }
}
