using DigitalDocumentPlatform.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalDocumentPlatform.DataAccess.Configurations
{
    public class WorkflowDefinitionConfiguration : IEntityTypeConfiguration<WorkflowDefinition>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Name).IsRequired().HasMaxLength(200);

            // WorkflowDefinition -> Outlet: Restrict (outlet deletion won't cascade to workflows)
            builder.HasOne(w => w.Outlet)
                .WithMany(o => o.WorkflowDefinitions)
                .HasForeignKey(w => w.OutletId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(w => !w.IsDelete);
        }
    }

    public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
    {
        public void Configure(EntityTypeBuilder<WorkflowStep> builder)
        {
            builder.HasKey(s => s.Id);

            // WorkflowStep -> WorkflowDefinition: Cascade
            builder.HasOne(s => s.WorkflowDefinition)
                .WithMany(w => w.Steps)
                .HasForeignKey(s => s.WorkflowDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkflowStep -> FormTemplate: Restrict (optional FK, no cascade)
            builder.HasOne(s => s.FormTemplate)
                .WithMany(t => t.WorkflowSteps)
                .HasForeignKey(s => s.FormTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkflowStep -> DocumentTemplate: Restrict (optional FK, no cascade)
            builder.HasOne(s => s.DocumentTemplate)
                .WithMany(d => d.WorkflowSteps)
                .HasForeignKey(s => s.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(s => !s.IsDelete);
        }
    }
}
