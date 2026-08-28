namespace FundManager.Implement.ViewModels.Request
{
    public class RegisterDeviceRequest
    {
        public string DeviceName { get; set; } = string.Empty;
        public string? MacAddress { get; set; }
        public string? IpAddress { get; set; }
    }

    public class SendSignatureRequest
    {
        public int PatronId { get; set; }
        public int StaffDeviceId { get; set; }
        public int? PreferredPatronDeviceId { get; set; }
    }

    public class SignatureSessionResponse
    {
        public int SessionId { get; set; }
        public int PatronId { get; set; }
        public string PatronName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public PatronDataForSignature PatronData { get; set; } = null!;
    }

    public class PatronDataForSignature
    {
        public int Pid { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}