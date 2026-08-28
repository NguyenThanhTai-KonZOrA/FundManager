using DigitalDocumentPlatform.API.Filters;
using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;

namespace DigitalDocumentPlatform.API.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;
        private readonly IRoomService _roomService;
        public RoomController(ILogger<RoomController> logger, IRoomService roomService)
        {
            _logger = logger;
            _roomService = roomService;
        }

        [HttpGet("detail/{roomNumber}")]
        [AuditLog("Room", "GetDetail")]
        public async Task<IActionResult> GetRoomDetail(string roomNumber, [FromQuery] DateTime? hotelDate)
        {
            if (string.IsNullOrEmpty(roomNumber))
                throw new BadHttpRequestException("Room number is required.");

            var roomInfoRequest = new RoomInfoRequest
            {
                RoomNumber = roomNumber,
                HotelDate = hotelDate
            };

            try
            {
                var roomDetail = await _roomService.GetRoomDetailAsync(roomInfoRequest);
                return Ok(roomDetail);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching room detail.");
                throw new BadHttpRequestException("Error occurred while fetching room detail.", ex);
            }
        }
    }
}