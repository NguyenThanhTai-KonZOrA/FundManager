using FundManager.Common.Constants;
using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Initalizer
{
    public static class RoleSeed
    {
        public static void Seed(EntityTypeBuilder<Role> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Role { Id = 1, RoleName = CommonConstants.AdminRole, Description = "Full system access", CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser, IsActive = true, IsDelete = false }
            );
        }
    }
}