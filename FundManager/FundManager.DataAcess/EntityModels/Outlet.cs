
using FundManager.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;

namespace FundManager.DataAccess.EntityModels
{
    public class Outlet : BaseEntity
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(100)]
        public string Code { get; set; } = string.Empty;
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        [StringLength(100)]
        public string MainColor { get; set; } = string.Empty;
        [StringLength(200)]
        public string IconImageUrl { get; set; } = string.Empty;
        [StringLength(200)]
        public string BackgroundImageUrl { get; set; } = string.Empty;

        // Navigation for many-to-many with Property
        public ICollection<PropertyOutlet> PropertyOutlets { get; set; } = [];

        // Navigation for one-to-many with StaffDevice
        public ICollection<StaffDevice> StaffDevices { get; set; } = [];

        // Navigation for one-to-many with WorkflowDefinition
        public ICollection<WorkflowDefinition> WorkflowDefinitions { get; set; } = [];
    }
}