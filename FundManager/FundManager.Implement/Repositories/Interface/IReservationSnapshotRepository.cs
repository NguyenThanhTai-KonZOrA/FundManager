using DigitalDocumentPlatform.DataAccess.ExternalEntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IReservationSnapshotRepository
    {
        /// <summary>Get the main guest snapshot for a room on a business date.</summary>
        Task<ReservationSnapshot?> GetMainGuestSnapshotsByRoomAsync(DateTime businessDate, string roomNumber);
        Task<List<ReservationSnapshot>> GetGuestSnapshotsAsync(DateTime businessDate, string roomNumber);
    }
}