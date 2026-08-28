namespace FundManager.Implement.ViewModels.Response
{
    public class TheGrandEmployeeResponse
    {
        public string employeeID { get; set; } = string.Empty;
        public string fullName { get; set; } = string.Empty;
        public string departmentName { get; set; } = string.Empty;
        public string position { get; set; } = string.Empty;
        public string adUserName { get; set; } = string.Empty;
        public bool adStatus { get; set; }
    }

    public class TheGrandEmployeeBaseResponse
    {
        public string result { get; set; } = string.Empty;
        public List<TheGrandEmployeeResponse> data { get; set; } = new List<TheGrandEmployeeResponse>();
    }
}