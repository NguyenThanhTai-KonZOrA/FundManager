namespace FundManager.DataAccess.EntityModels
{
    /// <summary>
    /// Defines the type/category of a document template.
    /// New types can be added here without schema changes.
    /// </summary>
    public enum DocumentType
    {
        /// <summary>Personal Data Processing consent form.</summary>
        PDP = 1,

        /// <summary>Hotel Terms and Policies acknowledgement.</summary>
        HTP = 2,

        /// <summary>General Terms and Conditions.</summary>
        Term = 3,

        /// <summary>Spa-specific liability release / acknowledgement.</summary>
        SpaAcknowledgement = 4,

        /// <summary>Any other document type not covered above.</summary>
        Other = 99
    }
}
