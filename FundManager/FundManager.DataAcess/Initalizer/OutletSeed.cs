using FundManager.Common.Constants;
using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Initalizer
{
    public static class OutletSeed
    {
        public static void Seed(EntityTypeBuilder<Outlet> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            var propertyOutlets = new List<PropertyOutlet>
            {
                new PropertyOutlet
                {
                    Id = 1,
                    PropertyId = 1,
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },
                new PropertyOutlet
                {
                    Id = 2,
                    PropertyId = 2,
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },
                new PropertyOutlet
                {
                    Id = 3,
                    PropertyId = 3,
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                }
            };

            builder.HasData(
                new Outlet
                {
                    Id = 1,
                    Name = "The Grand Spa",
                    Code = "THE_GRAND_SPA",
                    Description = "The Grand Spa",
                    MainColor = "#274549",
                    IconImageUrl = "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png",
                    BackgroundImageUrl = "/ApplicationImages/3cc52c88-5c58-481e-9f5a-533152911041.png",
                    PropertyOutlets = propertyOutlets.FirstOrDefault(p => p.OutletId == 1) != null ? new List<PropertyOutlet> { propertyOutlets.FirstOrDefault(p => p.OutletId == 1) } : null,
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },
                new Outlet
                {
                    Id = 2,
                    Name = "Lotus Spa",
                    Code = "LOTUS_SPA",
                    Description = "Lotus Spa",
                    MainColor = "#384fc2",
                    IconImageUrl = "/ApplicationImages/70de369b-60ee-4aaf-85b0-04b8ad623991.png",
                    BackgroundImageUrl = "/ApplicationImages/ab90af7f-6576-4f63-bce8-bae4d687de0b.png",
                    PropertyOutlets = propertyOutlets.FirstOrDefault(p => p.OutletId == 2) != null ? new List<PropertyOutlet> { propertyOutlets.FirstOrDefault(p => p.OutletId == 2) } : null,
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },
                new Outlet
                {
                    Id = 3,
                    Name = "Maia Spa",
                    Code = "MAIA_SPA",
                    Description = "Maia Spa",
                    MainColor = "#f07ace",
                    IconImageUrl = "/ApplicationImages/ede11902-62a8-4e76-859f-2312b0c24893.png",
                    BackgroundImageUrl = "/ApplicationImages/be451c4b-e9d3-44d3-834c-28b94c352353.png",
                    PropertyOutlets = propertyOutlets.FirstOrDefault(p => p.OutletId == 3) != null ? new List<PropertyOutlet> { propertyOutlets.FirstOrDefault(p => p.OutletId == 3) } : null,
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },
                new Outlet
                {
                    Id = 4,
                    Name = "Global",
                    Code = "GLOBAL",
                    Description = "Global",
                    MainColor = "#274549",
                    BackgroundImageUrl = "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png",
                    IconImageUrl = "/ApplicationImages/ab90af7f-6576-4f63-bce8-bae4d687de0b.png",
                    CreatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                }
            );
        }
    }
}