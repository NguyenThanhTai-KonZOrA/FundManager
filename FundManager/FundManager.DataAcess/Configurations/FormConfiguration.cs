using DigitalDocumentPlatform.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalDocumentPlatform.DataAccess.Configurations
{
    public class FormTemplateEntityConfiguration : IEntityTypeConfiguration<FormTemplate>
    {
        public void Configure(EntityTypeBuilder<FormTemplate> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Version).HasDefaultValue(1);
            builder.HasQueryFilter(t => !t.IsDelete);
        }
    }

    public class FormQuestionConfiguration : IEntityTypeConfiguration<FormQuestion>
    {
        public void Configure(EntityTypeBuilder<FormQuestion> builder)
        {
            builder.HasKey(q => q.Id);

            // FormQuestion -> FormTemplate: cascade (deleting a template deletes its questions)
            builder.HasOne(q => q.FormTemplate)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.FormTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(q => !q.IsDelete);
        }
    }

    public class FormQuestionOptionConfiguration : IEntityTypeConfiguration<FormQuestionOption>
    {
        public void Configure(EntityTypeBuilder<FormQuestionOption> builder)
        {
            builder.HasKey(o => o.Id);

            // FormQuestionOption -> FormQuestion: cascade
            builder.HasOne(o => o.FormQuestion)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.FormQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(o => !o.IsDelete);
        }
    }

    public class FormSubmissionConfiguration : IEntityTypeConfiguration<FormSubmission>
    {
        public void Configure(EntityTypeBuilder<FormSubmission> builder)
        {
            builder.HasKey(s => s.Id);

            // FormSubmission -> FormTemplate: Restrict (keep historical submissions when template is deleted)
            builder.HasOne(s => s.FormTemplate)
                .WithMany(t => t.Submissions)
                .HasForeignKey(s => s.FormTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // FormSubmission -> PatronDevice: Restrict (optional FK)
            builder.HasOne(s => s.PatronDevice)
                .WithMany()
                .HasForeignKey(s => s.PatronDeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(s => !s.IsDelete);
        }
    }

    public class FormSubmissionAnswerConfiguration : IEntityTypeConfiguration<FormSubmissionAnswer>
    {
        public void Configure(EntityTypeBuilder<FormSubmissionAnswer> builder)
        {
            builder.HasKey(a => a.Id);

            // FormSubmissionAnswer -> FormSubmission: Cascade (delete submission → delete answers)
            builder.HasOne(a => a.FormSubmission)
                .WithMany(s => s.Answers)
                .HasForeignKey(a => a.FormSubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // FormSubmissionAnswer -> FormQuestion: Restrict (prevents cascade cycle)
            // Deleting a question does NOT auto-delete answer history records
            builder.HasOne(a => a.FormQuestion)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.FormQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(a => !a.IsDelete);
        }
    }
}
