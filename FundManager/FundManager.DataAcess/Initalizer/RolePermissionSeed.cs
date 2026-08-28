using DigitalDocumentPlatform.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalDocumentPlatform.DataAccess.Initalizer
{
    public static class RolePermissionSeed
    {
        public static void Seed(EntityTypeBuilder<RolePermission> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            var rolePermissions = new List<RolePermission>();

            // Administrator - all permissions
            var adminPermissions = new List<RolePermission>()
            {
                new() { Id = 1, RoleId = 1, PermissionId = 1, CreatedAt = seedAt, UpdatedAt = seedAt },
                new() { Id = 2, RoleId = 1, PermissionId = 2, CreatedAt = seedAt, UpdatedAt = seedAt },
                new() { Id = 3, RoleId = 1, PermissionId = 3, CreatedAt = seedAt, UpdatedAt = seedAt },
                new() { Id = 4, RoleId = 1, PermissionId = 4, CreatedAt = seedAt, UpdatedAt = seedAt },
                new() { Id = 5, RoleId = 1, PermissionId = 5, CreatedAt = seedAt, UpdatedAt = seedAt },
                new() { Id = 6, RoleId = 1, PermissionId = 6, CreatedAt = seedAt, UpdatedAt = seedAt },
            };
            rolePermissions.AddRange(adminPermissions);
            builder.HasData(rolePermissions);
        }
    }
}