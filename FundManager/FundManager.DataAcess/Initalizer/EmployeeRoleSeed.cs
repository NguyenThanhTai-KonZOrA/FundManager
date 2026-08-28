using FundManager.Common.Constants;
using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Initalizer
{
    public static class EmployeeRoleSeed
    {
        public static void Seed(EntityTypeBuilder<EmployeeRole> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new EmployeeRole
                {
                    Id = 1,
                    RoleId = 1,
                    EmployeeId = 1,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser,
                    IsActive = true,
                    IsDelete = false
                }
            );
        }
    }
}