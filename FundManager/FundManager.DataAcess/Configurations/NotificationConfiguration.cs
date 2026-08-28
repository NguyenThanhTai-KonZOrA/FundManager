using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.PayloadJson).IsRequired();
            builder.Property(n => n.Status).HasMaxLength(32).IsRequired();
            builder.Property(n => n.AttemptCount).HasDefaultValue(0);
            builder.Property(n => n.CreatedBy).HasMaxLength(100).HasDefaultValue("System");

            builder.HasIndex(n => new { n.Status, n.AttemptCount, n.StaffDeviceId })
                .HasDatabaseName("IX_Notifications_Pending")
                .IncludeProperties(n => new { n.Id, n.SessionId, n.PayloadJson, n.CreatedAt })
                .HasFilter("[Status] = 'Pending'");

            builder.HasIndex(n => new { n.SessionId, n.StaffDeviceId })
                .HasDatabaseName("IX_Notifications_SessionStaff");

            builder.HasIndex(n => n.StaffDeviceId)
                .HasDatabaseName("IX_Notifications_StaffDeviceId");

            builder.HasQueryFilter(x => !x.IsDelete);
        }
    }
}