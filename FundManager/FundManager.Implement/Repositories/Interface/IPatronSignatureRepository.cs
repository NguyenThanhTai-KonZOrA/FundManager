using FundManager.Common.Enum;
using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Repositories.Interface
{
    public interface IPatronSignatureRepository : IGenericRepository<PatronSignature>
    {
        /// <summary>
        /// Get the latest signature of a patron for a specific document type
        /// </summary>
        Task<PatronSignature?> GetLatestSignatureAsync(int patronId, DocumentTypeEnum documentType);

        /// <summary>
        /// Get all signatures of a patron by document type
        /// </summary>
        Task<IEnumerable<PatronSignature>> GetSignaturesByDocumentTypeAsync(int patronId, DocumentTypeEnum documentType);

        /// <summary>
        /// Get all signatures of a patron regardless of document type
        /// </summary>
        Task<IEnumerable<PatronSignature>> GetAllSignaturesByPatronIdAsync(int patronId);

        /// <summary>
        /// Check if patron has signed a document of a specific type
        /// </summary>
        Task<bool> HasSignedDocumentAsync(int patronId, DocumentTypeEnum documentType);

        Task<IEnumerable<PatronSignature>> GetAllPatronSignature();
    }
}