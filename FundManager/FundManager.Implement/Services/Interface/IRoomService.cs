using FundManager.Implement.ViewModels.Request;
using FundManager.Implement.ViewModels.Response;

namespace FundManager.Implement.Services.Interface
{
    public interface IRoomService
    {
        Task<RoomDetailResponse> GetRoomDetailAsync(RoomInfoRequest roomInfoRequest);
    }
}