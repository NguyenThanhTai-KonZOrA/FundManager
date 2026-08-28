using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.ViewModels.Response
{
    public class DocumentTemplateResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public string DocumentTypeName => DocumentType.ToString();
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public int? OutletId { get; set; }
        public string? OutletName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<DocumentTemplateTranslationResponse> Translations { get; set; } = [];
        public List<DocumentTemplateVersionHistoryResponse> VersionHistories { get; set; } = [];
    }

    public class DocumentTemplateBriefResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public string DocumentTypeName => DocumentType.ToString();
        public string Description { get; set; } = string.Empty;
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public int? OutletId { get; set; }
        public string? OutletName { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public List<DocumentTemplateTranslationResponse> Translations { get; set; } = [];
        public List<DocumentTemplateVersionHistoryResponse> VersionHistories { get; set; } = [];
    }
}
