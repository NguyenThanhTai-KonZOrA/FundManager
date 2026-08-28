using DigitalDocumentPlatform.Implement.ViewModels.Request;
using DigitalDocumentPlatform.Implement.ViewModels.Response;

namespace DigitalDocumentPlatform.Implement.Services.Interface
{
    public interface IRoomService
    {
        Task<RoomDetailResponse> GetRoomDetailAsync(RoomInfoRequest roomInfoRequest);
    }
}