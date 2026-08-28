using FundManager.Common.Enum;

namespace FundManager.Implement.ViewModels.Request
{
    public class NotificationConfirmRequest
    {
        public int PatronId { get; set; }
        public string Signature { get; set; } = string.Empty;
        public int StaffDeviceId { get; set; }
        public DocumentTypeEnum DocumentType { get; set; } = DocumentTypeEnum.PdpForm;
    }
}