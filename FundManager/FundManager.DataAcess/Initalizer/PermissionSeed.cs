using FundManager.Common.Constants;
using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Initalizer
{
    public static class PermissionSeed
    {
        public static void Seed(EntityTypeBuilder<Permission> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                // Employee Management
                new Permission { Id = 1, PermissionName = "View Dashboard", PermissionCode = CommonConstants.CanViewDashboard, Category = "Employee", Description = "View dashboard", CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser, IsActive = true, IsDelete = false }
            );
        }
    }
}