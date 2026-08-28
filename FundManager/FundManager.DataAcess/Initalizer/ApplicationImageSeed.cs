using FundManager.Common.Enum;
using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Initalizer
{
    public static class ApplicationImageSeed
    {
        public static void Seed(EntityTypeBuilder<ApplicationImage> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                // For Application Logo
                new ApplicationImage
                {
                    Id = 1,
                    Name = "The Grand Ho Tram",
                    Description = "The Grand Ho Tram Logo",
                    FileExtension = ".png",
                    FilePath = "/ApplicationImages/TheGrandHoTramLogo.png",
                    FileSize = 3100, // Example file size in bytes
                    FileUrl = "/ApplicationImages/16831151-1e2c-4f27-bc56-1a26c3afef0f.jpg",
                    OutletId = null,
                    PropertyId = null,
                    Type = ImageTypeEnum.Logo,
                    CreatedAt = seedAt,
                    CreatedBy = Common.Constants.CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = Common.Constants.CommonConstants.SystemUser,
                },
                // For Outlet Logos
                new ApplicationImage
                {
                    Id = 2,
                    Name = "The Grand Spa Icon",
                    Description = "The Grand Spa",
                    FileExtension = ".png",
                    FilePath = "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png",
                    FileSize = 13050, // Example file size in bytes
                    FileUrl = "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png",
                    OutletId = 1,
                    PropertyId = 2,
                    Type = ImageTypeEnum.Outlet,
                    CreatedAt = seedAt,
                    CreatedBy = Common.Constants.CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = Common.Constants.CommonConstants.SystemUser,
                },
                new ApplicationImage
                {
                    Id = 3,
                    Name = "Lotus Spa Icon",
                    Description = "Lotus Spa Logo",
                    FileExtension = ".png",
                    FilePath = "/ApplicationImages/70de369b-60ee-4aaf-85b0-04b8ad623991.png",
                    FileSize = 11465, // Example file size in bytes
                    FileUrl = "/ApplicationImages/70de369b-60ee-4aaf-85b0-04b8ad623991.png",
                    OutletId = 2,
                    PropertyId = 1,
                    Type = ImageTypeEnum.Outlet,
                    CreatedAt = seedAt,
                    CreatedBy = Common.Constants.CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = Common.Constants.CommonConstants.SystemUser,
                },
                new ApplicationImage
                {
                    Id = 4,
                    Name = "MAIA SPA Icon",
                    Description = "MAIA SPA Icon",
                    FileExtension = ".png",
                    FilePath = "/ApplicationImages/ede11902-62a8-4e76-859f-2312b0c24893.png",
                    FileSize = 12176, // Example file size in bytes
                    FileUrl = "/ApplicationImages/ede11902-62a8-4e76-859f-2312b0c24893.png",
                    OutletId = 3,
                    PropertyId = 3,
                    Type = ImageTypeEnum.Outlet,
                    CreatedAt = seedAt,
                    CreatedBy = Common.Constants.CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = Common.Constants.CommonConstants.SystemUser,
                },
                new ApplicationImage
                {
                    Id = 5,
                    Name = "The Grand Spa Image",
                    Description = "The Grand Spa Image",
                    FileExtension = ".png",
                    FilePath = "/ApplicationImages/3cc52c88-5c58-481e-9f5a-533152911041.png",
                    FileSize = 1003615, // Example file size in bytes
                    FileUrl = "/ApplicationImages/3cc52c88-5c58-481e-9f5a-533152911041.png",
                    OutletId = 1,
                    PropertyId = 1,
                    Type = ImageTypeEnum.Outlet,
                    CreatedAt = seedAt,
                    CreatedBy = Common.Constants.CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = Common.Constants.CommonConstants.SystemUser,
                },
                new ApplicationImage
                {
                    Id = 6,
                    Name = "Lotus Spa Image",
                    Description = "Lotus Spa Image",
                    FileExtension = ".png",
                    FilePath = "/ApplicationImages/ab90af7f-6576-4f63-bce8-bae4d687de0b.png",
                    FileSize = 551515, // Example file size in bytes
                    FileUrl = "/ApplicationImages/ab90af7f-6576-4f63-bce8-bae4d687de0b.png",
                    OutletId = 1,
                    PropertyId = 1,
                    Type = ImageTypeEnum.Outlet,
                    CreatedAt = seedAt,
                    CreatedBy = Common.Constants.CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = Common.Constants.CommonConstants.SystemUser,
                },
                new ApplicationImage
                {
                    Id = 7,
                    Name = "Maia Spa Image",
                    Description = "Maia Spa Image",
                    FileExtension = ".png",
                    FilePath = "/ApplicationImages/be451c4b-e9d3-44d3-834c-28b94c352353.png",
                    FileSize = 844391, // Example file size in bytes
                    FileUrl = "/ApplicationImages/be451c4b-e9d3-44d3-834c-28b94c352353.png",
                    OutletId = 1,
                    PropertyId = 1,
                    Type = ImageTypeEnum.Outlet,
                    CreatedAt = seedAt,
                    CreatedBy = Common.Constants.CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = Common.Constants.CommonConstants.SystemUser,
                },
                new ApplicationImage
                {
                    Id = 8,
                    Name = "English Flag",
                    Description = "English Flag",
                    FileExtension = ".svg",
                    FilePath = "D:\\IIS Publish\\DigitalDocumentPlatform\\FundManager.API\\ApplicationImages\\bf6f69ff-9957-4faf-8524-6833eb96e3f9.svg",
                    FileSize = 1274, // Example file size in bytes
                    FileUrl = "/ApplicationImages/bf6f69ff-9957-4faf-8524-6833eb96e3f9.svg",
                    OutletId = null,
                    PropertyId = null,
                    Type = ImageTypeEnum.Icon,
                    CreatedAt = seedAt,
                    CreatedBy = Common.Constants.CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = Common.Constants.CommonConstants.SystemUser,
                },
                 new ApplicationImage
                 {
                     Id = 9,
                     Name = "Vietnam Flag",
                     Description = "Vietnam Flag",
                     FileExtension = ".svg",
                     FilePath = "D:\\IIS Publish\\DigitalDocumentPlatform\\FundManager.API\\ApplicationImages\\6640af6d-60a0-4cea-98c8-1871e7facb0e.svg",
                     FileSize = 1274, // Example file size in bytes
                     FileUrl = "/ApplicationImages/6640af6d-60a0-4cea-98c8-1871e7facb0e.svg",
                     OutletId = null,
                     PropertyId = null,
                     Type = ImageTypeEnum.Icon,
                     CreatedAt = seedAt,
                     CreatedBy = Common.Constants.CommonConstants.SystemUser,
                     UpdatedAt = seedAt,
                     UpdatedBy = Common.Constants.CommonConstants.SystemUser,
                 },
                  new ApplicationImage
                  {
                      Id = 10,
                      Name = "Korean Flag",
                      Description = "Korean Flag",
                      FileExtension = ".svg",
                      FilePath = "D:\\IIS Publish\\DigitalDocumentPlatform\\FundManager.API\\ApplicationImages\\f4bb608f-5a36-4b58-9959-c3361228a7ee.svg",
                      FileSize = 1274, // Example file size in bytes
                      FileUrl = "/ApplicationImages/f4bb608f-5a36-4b58-9959-c3361228a7ee.svg",
                      OutletId = null,
                      PropertyId = null,
                      Type = ImageTypeEnum.Icon,
                      CreatedAt = seedAt,
                      CreatedBy = Common.Constants.CommonConstants.SystemUser,
                      UpdatedAt = seedAt,
                      UpdatedBy = Common.Constants.CommonConstants.SystemUser,
                  },
                  new ApplicationImage
                  {
                      Id = 11,
                      Name = "China Flag",
                      Description = "China Flag",
                      FileExtension = ".svg",
                      FilePath = "D:\\IIS Publish\\DigitalDocumentPlatform\\FundManager.API\\ApplicationImages\\d350d454-b583-4725-b20f-7c1a0e03d51b.svg",
                      FileSize = 1274, // Example file size in bytes
                      FileUrl = "/ApplicationImages/d350d454-b583-4725-b20f-7c1a0e03d51b.svg",
                      OutletId = null,
                      PropertyId = null,
                      Type = ImageTypeEnum.Icon,
                      CreatedAt = seedAt,
                      CreatedBy = Common.Constants.CommonConstants.SystemUser,
                      UpdatedAt = seedAt,
                      UpdatedBy = Common.Constants.CommonConstants.SystemUser,
                  }
            );
        }
    }
}