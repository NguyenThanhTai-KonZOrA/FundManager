namespace FundManager.DataAccess.EntityModels
{
    /// <summary>
    /// Defines what kind of action a WorkflowStep requires the patron to perform.
    /// </summary>
    public enum StepType
    {
        /// <summary>Patron must fill in and submit a FormTemplate.</summary>
        FillForm = 1,
        /// <summary>Patron must sign a document.</summary>
        DocumentAndSignature = 2,
    }
}