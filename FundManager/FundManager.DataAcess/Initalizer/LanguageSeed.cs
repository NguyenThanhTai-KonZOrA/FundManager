
using DigitalDocumentPlatform.Common.Constants;

namespace DigitalDocumentPlatform.DataAccess.Initalizer
{
    public static class LanguageSeed
    {
        public static void Seed(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<EntityModels.Language> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);
            builder.HasData(
                new EntityModels.Language
                {
                    Id = 1,
                    Code = CommonConstants.DefaultLanguage,
                    Name = "English",
                    NativeName = "English",
                    FlagEmoji = "/ApplicationImages/bf6f69ff-9957-4faf-8524-6833eb96e3f9.svg",
                    SortOrder = 1,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                new EntityModels.Language
                {
                    Id = 2,
                    Code = "vi",
                    Name = "Vietnamese",
                    NativeName = "Tiếng Việt",
                    FlagEmoji = "/ApplicationImages/6640af6d-60a0-4cea-98c8-1871e7facb0e.svg",
                    SortOrder = 2,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                new EntityModels.Language
                {
                    Id = 3,
                    Code = "ko",
                    Name = "Korean",
                    NativeName = "한국어",
                    FlagEmoji = "/ApplicationImages/f4bb608f-5a36-4b58-9959-c3361228a7ee.svg",
                    SortOrder = 3,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                new EntityModels.Language
                {
                    Id = 4,
                    Code = "zh",
                    Name = "Chinese",
                    NativeName = "中文",
                    FlagEmoji = "/ApplicationImages/d350d454-b583-4725-b20f-7c1a0e03d51b.svg",
                    SortOrder = 4,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                }
            );
        }
    }
}