using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.Implement.ViewModels.Request
{
    public class AssignRoleToEmployeeRequest
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public List<int> RoleIds { get; set; } = new();
    }
}