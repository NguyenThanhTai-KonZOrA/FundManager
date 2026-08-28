using DigitalDocumentPlatform.Common.Constants;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.DataAccess.Initalizer
{
    /// <summary>
    /// Seeds default workflow definitions and steps for The Spa outlet.
    ///
    /// Flow 1 – Full Spa Journey (OutletId = 4 / The Spa):
    ///   Step 1: FillForm         → Spa Consultation Form  (FormTemplateId = 1)
    ///   Step 2: Signature        → Spa Liability Release   (DocumentTemplateId = 3)
    ///   Step 3: Acknowledgement  → PDP Consent             (DocumentTemplateId = 1)
    ///
    /// Flow 2 – Document-Only (OutletId = 4 / The Spa, inactive by default):
    ///   Step 1: Signature        → Spa Liability Release   (DocumentTemplateId = 3)
    ///   Step 2: Acknowledgement  → PDP Consent             (DocumentTemplateId = 1)
    /// </summary>
    public static class WorkflowSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            // ── WorkflowDefinitions ───────────────────────────────────────────
            modelBuilder.Entity<WorkflowDefinition>().HasData(
                new WorkflowDefinition
                {
                    Id = 1,
                    Name = CommonConstants.DefaultWorkflowName,
                    Description = "Complete spa intake flow: consultation form → liability signature → PDP consent.",
                    OutletId = 4,
                    IsActive = true,
                    IsDelete = false,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                new WorkflowDefinition
                {
                    Id = 2,
                    Name = "Spa Full Journey",
                    Description = "Complete spa intake flow: consultation form → liability signature → PDP consent.",
                    OutletId = 4,
                    IsActive = true,
                    IsDelete = false,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                new WorkflowDefinition
                {
                    Id = 3,
                    Name = "Spa Document-Only",
                    Description = "Abbreviated flow for returning guests: liability signature → PDP consent only.",
                    OutletId = 4,
                    IsActive = false,
                    IsDelete = false,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                }
            );

            // ── WorkflowSteps ─────────────────────────────────────────────────
            modelBuilder.Entity<WorkflowStep>().HasData(
                // Flow 1 – Step 1: Fill Default Consultation Form
                new WorkflowStep
                {
                    Id = 1,
                    WorkflowDefinitionId = 1,
                    StepOrder = 1,
                    StepType = StepType.FillForm,
                    StepLabel = "Default Consultation Form",
                    FormTemplateId = 1,
                    DocumentTemplateId = null,
                    IsActive = true,
                    IsDelete = false,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Flow 1 – Step 2: DocumentAndSignature PDP Consent
                new WorkflowStep
                {
                    Id = 3,
                    WorkflowDefinitionId = 1,
                    StepOrder = 2,
                    StepType = StepType.DocumentAndSignature,
                    StepLabel = "Personal Data Processing Consent",
                    FormTemplateId = null,
                    DocumentTemplateId = 1,
                    IsActive = true,
                    IsDelete = false,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },

                // Flow 2 – Step 1: Fill Spa Consultation Form
                new WorkflowStep
                {
                    Id = 4,
                    WorkflowDefinitionId = 2,
                    StepOrder = 1,
                    StepType = StepType.FillForm,
                    StepLabel = "Spa Consultation Form",
                    FormTemplateId = 1,
                    DocumentTemplateId = null,
                    IsActive = true,
                    IsDelete = false,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Flow 2 – Step 2: Sign Spa Liability Release
                new WorkflowStep
                {
                    Id = 6,
                    WorkflowDefinitionId = 2,
                    StepOrder = 2,
                    StepType = StepType.DocumentAndSignature,
                    StepLabel = "Personal Data Processing Consent",
                    FormTemplateId = null,
                    DocumentTemplateId = 1,
                    IsActive = true,
                    IsDelete = false,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },

                // Flow 3 – Step 1: Sign Spa Liability Release
                new WorkflowStep
                {
                    Id = 7,
                    WorkflowDefinitionId = 3,
                    StepOrder = 1,
                    StepType = StepType.DocumentAndSignature,
                    StepLabel = "Spa Liability Release",
                    FormTemplateId = null,
                    DocumentTemplateId = 3,
                    IsActive = true,
                    IsDelete = false,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                }
            );
        }
    }
}