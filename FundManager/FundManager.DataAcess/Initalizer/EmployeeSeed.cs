using FundManager.Common.Constants;
using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Initalizer
{
    public static class EmployeeSeed
    {
        public static void Seed(EntityTypeBuilder<Employee> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Employee
                {
                    Id = 1,
                    FullName = "System Administrator",
                    Email = "adminTemp@thegrandhotram.com",
                    PhoneNumber = CommonConstants.SystemUser,
                    Department = "IT",
                    Position = "Administrator",
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser,
                    IsActive = true,
                    IsDelete = false,
                    WindowAccount = CommonConstants.AdminUserName,
                    EmployeeCode = CommonConstants.AdminUserName
                }
            );
        }
    }
}