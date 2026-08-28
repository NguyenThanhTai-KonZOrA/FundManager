using DigitalDocumentPlatform.Common.Enum;

namespace DigitalDocumentPlatform.Implement.ViewModels.Request
{
    public class SignatureConfirmRequest
    {
        public int PatronId { get; set; }
        public string Signature { get; set; } = string.Empty;
        public int SessionId { get; set; }
        public int StaffDeviceId { get; set; }
        public DocumentTypeEnum DocumentType { get; set; } = DocumentTypeEnum.PdpForm;
    }
}