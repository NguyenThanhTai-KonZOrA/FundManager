using System.ComponentModel.DataAnnotations;

namespace DigitalDocumentPlatform.Implement.ViewModels.Request
{
    public class MappingDeviceToPropertyRequest
    {
        [Required]
        public int DeviceId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdatePropertyDeviceMappingRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int DeviceId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class GetPropertyDeviceMappedRequest
    {
        /// <summary>
        /// Computer hostname of the client machine
        /// </summary>
        public string? DeviceName { get; set; }

        /// <summary>
        /// MAC address of the client machine (optional, for more precise matching)
        /// </summary>
        public string? MacAddress { get; set; }
    }
}