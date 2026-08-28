using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Configurations
{
    public class ApplicationSettingsConfiguration : IEntityTypeConfiguration<ApplicationSettings>
    {
        public void Configure(EntityTypeBuilder<ApplicationSettings> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
            builder.HasIndex(e => e.Key).IsUnique();
        }
    }

    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
        }
    }

    public class OutletConfiguration : IEntityTypeConfiguration<Outlet>
    {
        public void Configure(EntityTypeBuilder<Outlet> builder)
        {
            builder.HasQueryFilter(x => !x.IsDelete);
        }
    }

    public class PropertyOutletConfiguration : IEntityTypeConfiguration<PropertyOutlet>
    {
        public void Configure(EntityTypeBuilder<PropertyOutlet> builder)
        {
            builder.HasKey(po => new { po.PropertyId, po.OutletId });

            builder.HasOne(po => po.Property)
                .WithMany(p => p.PropertyOutlets)
                .HasForeignKey(po => po.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(po => po.Outlet)
                .WithMany(o => o.PropertyOutlets)
                .HasForeignKey(po => po.OutletId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
