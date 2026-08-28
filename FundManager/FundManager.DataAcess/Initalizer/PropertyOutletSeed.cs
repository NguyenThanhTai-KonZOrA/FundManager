using FundManager.Common.Constants;
using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Initalizer
{
    public static class PropertyOutletSeed
    {
        public static void Seed(EntityTypeBuilder<PropertyOutlet> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            var propertyOutlets = new List<PropertyOutlet>()
            {
                new PropertyOutlet
                {
                    Id = 1,
                    PropertyId = 1,
                    OutletId = 1,
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },
                new PropertyOutlet
                {
                    Id = 2,
                    PropertyId = 2,
                    OutletId = 2,
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },
                new PropertyOutlet
                {
                    Id = 3,
                    PropertyId = 3,
                    OutletId = 3,
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser
                }
            };
            builder.HasData(propertyOutlets);
        }
    }
}