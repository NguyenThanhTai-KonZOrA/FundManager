namespace DigitalDocumentPlatform.Implement.ViewModels
{
    public class CurrentEmployeeInfo
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string WindowAccount { get; set; } = string.Empty;
        public bool IsQualityControl { get; set; } = false;
    }
}