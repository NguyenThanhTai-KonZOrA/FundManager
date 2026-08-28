using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.ExternalEntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.Implement.Repositories
{
    public class ReservationSnapshotRepository : IReservationSnapshotRepository
    {
        private readonly BreakFastCheckInDbContext _context;

        public ReservationSnapshotRepository(BreakFastCheckInDbContext context)
        {
            _context = context;
        }

        public async Task<ReservationSnapshot?> GetMainGuestSnapshotsByRoomAsync(DateTime businessDate, string roomNumber)
        {
            return await _context.ReservationSnapshots
                .AsNoTracking()
                .Where(x => x.BusinessDate == businessDate
                    && x.RoomNumber == roomNumber
                    && x.MainGuest)
                .OrderByDescending(x => x.ReservationSnapshotID)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ReservationSnapshot>> GetGuestSnapshotsAsync(DateTime businessDate, string roomNumber)
        {
            return await _context.ReservationSnapshots
                .AsNoTracking()
                .Where(x => x.BusinessDate == businessDate
                    && x.RoomNumber == roomNumber)
                .OrderByDescending(x => x.MainGuest)
                .ThenBy(x => x.GuestName)
                .ToListAsync();
        }
    }
}