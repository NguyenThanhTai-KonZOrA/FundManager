using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface ISignatureSessionRepository : IGenericRepository<SignatureSession>
    {
        Task<SignatureSession> GetSignatureSessionAsync(int patronId, int staffDeviceId, int patronDeviceId);
        Task<SignatureSession?> GetPendingSessionByPatronIdAsync(int patronId);
        Task<SignatureSession?> GetCompletedSessionByPatronIdAsync(int patronId);
    }
}