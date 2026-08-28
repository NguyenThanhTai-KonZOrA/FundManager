using FundManager.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;

namespace FundManager.DataAccess.EntityModels
{
    /// <summary>
    /// Defines a configurable form (e.g. Spa Consultation Form).
    /// Supports versioning — each edit increments Version; submissions record which version was active.
    /// </summary>
    public class FormTemplate : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>URL of the logo displayed at the top of the form.</summary>
        [StringLength(500)]
        public string? LogoUrl { get; set; }

        /// <summary>Footer / disclaimer text rendered at the bottom of the form.</summary>
        [StringLength(2000)]
        public string? FooterText { get; set; }

        [StringLength(1000)]
        public string? AgreementText { get; set; }

        /// <summary>
        /// Monotonically increasing version counter.
        /// Incremented every time an admin publishes changes.
        /// </summary>
        public int Version { get; set; } = 1;

        // Navigation
        public ICollection<FormQuestion> Questions { get; set; } = [];
        public ICollection<FormSubmission> Submissions { get; set; } = [];
        public ICollection<WorkflowStep> WorkflowSteps { get; set; } = [];
        public ICollection<FormTemplateVersionHistory> VersionHistories { get; set; } = [];
        public ICollection<FormTemplateTranslation> Translations { get; set; } = [];
    }
}