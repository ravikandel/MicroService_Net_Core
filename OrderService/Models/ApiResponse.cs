namespace OrderService.Models
{
    public class ApiResponse<T>
    {
        public EResult StatusCode { get; set; }
        public string StatusText => StatusCode.ToString();
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}

