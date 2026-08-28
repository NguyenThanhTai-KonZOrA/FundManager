using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.Implement.ViewModels.Request
{
    public class AssignStaffDeviceToOutletRequest
    {
        [Required]
        public int StaffDeviceId { get; set; }
    }
}
