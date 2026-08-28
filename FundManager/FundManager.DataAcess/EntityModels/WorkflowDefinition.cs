using DigitalDocumentPlatform.Common.BaseEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDocumentPlatform.DataAccess.EntityModels
{
    /// <summary>
    /// Defines the ordered sequence of steps a patron must complete for a specific Outlet.
    /// One Outlet has at most one active WorkflowDefinition at a time.
    /// </summary>
    public class WorkflowDefinition : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>The Outlet this workflow is assigned to.</summary>
        [ForeignKey(nameof(Outlet))]
        public int OutletId { get; set; }
        public Outlet Outlet { get; set; } = null!;

        // Navigation
        public ICollection<WorkflowStep> Steps { get; set; } = [];
    }
}