using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundManager.DataAccess.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasIndex(a => a.UserName).HasDatabaseName("IX_AuditLogs_UserName");
            builder.HasIndex(a => a.Action).HasDatabaseName("IX_AuditLogs_Action");
            builder.HasIndex(a => new { a.EntityType, a.EntityId }).HasDatabaseName("IX_AuditLogs_Entity");
            builder.HasIndex(a => a.CreatedAt).HasDatabaseName("IX_AuditLogs_CreatedAt");
            builder.HasIndex(a => new { a.IsSuccess, a.CreatedAt })
                .HasDatabaseName("IX_AuditLogs_Success")
                .HasFilter("[IsSuccess] = 0");
            builder.HasQueryFilter(x => !x.IsDelete);
        }
    }
}
