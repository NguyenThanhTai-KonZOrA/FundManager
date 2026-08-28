using FundManager.Common.Constants;
using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Initalizer
{
    public static class ApplicationSettingsSeed
    {
        public static void Seed(EntityTypeBuilder<ApplicationSettings> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
              new ApplicationSettings
              {
                  Id = 1,
                  Key = "DelayDurationAfterSubmitted",
                  Value = "10",
                  Description = "Delay duration after submission in minutes.",
                  Category = CommonConstants.SystemUser,
                  DataType = CommonConstants.Integer,
                  CreatedAt = seedAt,
                  CreatedBy = CommonConstants.SystemUser,
                  UpdatedAt = seedAt,
                  UpdatedBy = CommonConstants.SystemUser,
              }
            );
        }
    }
}