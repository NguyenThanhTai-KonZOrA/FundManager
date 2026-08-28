using DigitalDocumentPlatform.Common.Constants;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalDocumentPlatform.DataAccess.Initalizer
{
    public static class PropertySeed
    {
        public static void Seed(EntityTypeBuilder<Property> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Property
                {
                    Id = 1,
                    Name = "InterContinental",
                    Code = "IC",
                    Description = "InterContinental Ho Tram",
                    Color = "#1976d2",
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },
                new Property
                {
                    Id = 2,
                    Name = "Holiday Inn",
                    Code = "HI",
                    Description = "Holiday Inn Ho Tram",
                    Color = "#388e3c",
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },
                new Property
                {
                    Id = 3,
                    Name = "Ixora",
                    Code = "IX",
                    Description = "Ixora Ho Tram",
                    Color = "#d32f2f",
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                }
            );
        }
    }
}