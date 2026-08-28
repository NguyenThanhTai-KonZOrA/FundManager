using System.ComponentModel.DataAnnotations;

namespace FundManager.Implement.ViewModels.Request
{
    public class AssignStaffDeviceToOutletRequest
    {
        [Required]
        public int StaffDeviceId { get; set; }
    }
}
