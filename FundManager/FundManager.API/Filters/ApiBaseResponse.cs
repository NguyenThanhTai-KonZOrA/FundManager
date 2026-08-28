namespace ProjectManagement.API.Filters
{
    public class ApiBaseResponse<T>
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}