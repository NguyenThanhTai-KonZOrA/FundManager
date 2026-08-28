using FundManager.Implement.Repositories.Interface;
using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
{
    public class RoomService : IRoomService
    {
        private readonly IReservationSnapshotRepository _reservationSnapshotRepository;
        public RoomService(IReservationSnapshotRepository reservationSnapshotRepository)
        {
            _reservationSnapshotRepository = reservationSnapshotRepository;
        }

        public async Task<RoomDetailResponse> GetRoomDetailAsync(RoomInfoRequest roomInfoRequest)
        {
            DateTime businessDate = DateTime.Today;
            if (roomInfoRequest.HotelDate.HasValue)
            {
                businessDate = roomInfoRequest.HotelDate.Value;
            }

            var snapshot = await _reservationSnapshotRepository.GetMainGuestSnapshotsByRoomAsync(businessDate, roomInfoRequest.RoomNumber!);

            if (snapshot == null)
                return new RoomDetailResponse();

            var guestSnapshots = await _reservationSnapshotRepository.GetGuestSnapshotsAsync(businessDate, roomInfoRequest.RoomNumber!);
            var guests = guestSnapshots.Select((x, i) => new SharerInfo
            {
                Name = x.GuestName?.ToUpper() ?? x.MainGuestName?.ToUpper() ?? string.Empty,
                GuestLabel = $"Guest {(i + 1)}",
                PlayerId = string.IsNullOrEmpty(x.MembershipNumber) ? (int?)null : int.TryParse(x.MembershipNumber, out var playerId) ? playerId : (int?)null,
            }).ToList();

            return new RoomDetailResponse
            {
                ResvId = snapshot.ReservationNo,
                GuestName = snapshot.GuestName?.ToUpper() ?? string.Empty,
                RoomNumber = snapshot.RoomNumber,
                TotalGuest = snapshot.TotalGuest,
                ArrivalDate = snapshot.ArrivalDate,
                DepartureDate = snapshot.DepartureDate,
                Adults = snapshot.AdultCount,
                Child1 = snapshot.Child1Count,
                Child2 = snapshot.Child2Count,
                SpecialRequests = "",
                Sharers = guests,
            };
        }
    }
}