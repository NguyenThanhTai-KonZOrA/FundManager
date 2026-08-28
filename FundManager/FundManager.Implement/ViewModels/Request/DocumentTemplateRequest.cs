using FundManager.DataAccess.EntityModels;
using System.ComponentModel.DataAnnotations;

namespace FundManager.Implement.ViewModels.Request
{
    public class CreateDocumentTemplateRequest
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DocumentType DocumentType { get; set; } = DocumentType.Other;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>Full HTML content of the document.</summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>Optional outlet scope. Null = applies to all outlets.</summary>
        public int? OutletId { get; set; }
    }

    public class UpdateDocumentTemplateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DocumentType DocumentType { get; set; } = DocumentType.Other;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int? OutletId { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>Optional note explaining what changed in this version.</summary>
        [StringLength(1000)]
        public string? ChangeNote { get; set; }
    }
}
