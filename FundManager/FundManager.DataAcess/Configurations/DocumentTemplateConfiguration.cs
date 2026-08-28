using DigitalDocumentPlatform.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalDocumentPlatform.DataAccess.Configurations
{
    public class DocumentTemplateConfiguration : IEntityTypeConfiguration<DocumentTemplate>
    {
        public void Configure(EntityTypeBuilder<DocumentTemplate> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Title).IsRequired().HasMaxLength(200);
            builder.Property(d => d.Version).HasDefaultValue(1);
            builder.Property(d => d.Content).IsRequired(false);

            // DocumentTemplate -> Outlet: Restrict (nullable FK)
            builder.HasOne(d => d.Outlet)
                .WithMany()
                .HasForeignKey(d => d.OutletId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(d => !d.IsDelete);
        }
    }
}
