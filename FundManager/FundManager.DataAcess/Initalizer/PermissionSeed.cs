using DigitalDocumentPlatform.Common.Constants;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalDocumentPlatform.DataAccess.Initalizer
{
    public static class PermissionSeed
    {
        public static void Seed(EntityTypeBuilder<Permission> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                // Employee Management
                new Permission { Id = 1, PermissionName = "View Dashboard", PermissionCode = CommonConstants.CanViewDashboard, Category = "Employee", Description = "View dashboard", CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser, IsActive = true, IsDelete = false },
                new Permission { Id = 2, PermissionName = "Room Check", PermissionCode = CommonConstants.CanCheckRoom, Category = "Employee", Description = "View room check information", CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser, IsActive = true, IsDelete = false },
                new Permission { Id = 3, PermissionName = "Un-check", PermissionCode = CommonConstants.CanUncheck, Category = "Employee", Description = "Update employee information", CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser, IsActive = true, IsDelete = false },
                new Permission { Id = 4, PermissionName = "History", PermissionCode = CommonConstants.CanViewHistory, Category = "Employee", Description = "View history information", CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser, IsActive = true, IsDelete = false },
                new Permission { Id = 5, PermissionName = "Reports", PermissionCode = CommonConstants.CanViewReports, Category = "Employee", Description = "View reports", CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser, IsActive = true, IsDelete = false },
                new Permission { Id = 6, PermissionName = "ViewAllReports", PermissionCode = CommonConstants.CanViewAllReports, Category = "Employee", Description = "View all reports", CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser, IsActive = true, IsDelete = false }
            );
        }
    }
}