using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Configurations
{
    public class StaffDeviceConfiguration : IEntityTypeConfiguration<StaffDevice>
    {
        public void Configure(EntityTypeBuilder<StaffDevice> builder)
        {
            builder.HasIndex(s => s.MacAddress).IsUnique();
            builder.HasQueryFilter(x => !x.IsDelete);
        }
    }

    public class PatronDeviceConfiguration : IEntityTypeConfiguration<PatronDevice>
    {
        public void Configure(EntityTypeBuilder<PatronDevice> builder)
        {
            builder.HasIndex(p => p.DeviceName).IsUnique();
            builder.HasIndex(p => p.ConnectionId);
            builder.HasQueryFilter(x => !x.IsDelete);
        }
    }

    public class SignatureSessionConfiguration : IEntityTypeConfiguration<SignatureSession>
    {
        public void Configure(EntityTypeBuilder<SignatureSession> builder)
        {
            builder.HasOne(s => s.Patron)
                .WithMany()
                .HasForeignKey(s => s.PatronId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.StaffDevice)
                .WithMany()
                .HasForeignKey(s => s.StaffDeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PatronDevice)
                .WithMany()
                .HasForeignKey(s => s.PatronDeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDelete);
        }
    }

    public class DeviceMappingConfiguration : IEntityTypeConfiguration<DeviceMapping>
    {
        public void Configure(EntityTypeBuilder<DeviceMapping> builder)
        {
            builder.HasOne(dm => dm.StaffDevice)
                .WithMany()
                .HasForeignKey(dm => dm.StaffDeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dm => dm.PatronDevice)
                .WithMany()
                .HasForeignKey(dm => dm.PatronDeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(dm => new { dm.StaffDeviceId, dm.IsActive })
                .IsUnique()
                .HasFilter("[IsActive] = 1");

            builder.HasIndex(dm => new { dm.PatronDeviceId, dm.IsActive })
                .IsUnique()
                .HasFilter("[IsActive] = 1");

            builder.HasQueryFilter(x => !x.IsDelete);
        }
    }
}
