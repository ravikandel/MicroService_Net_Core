namespace ProductService.Common
{

    public enum EResult
    {
        Success = 0,
        Error = 1,
        Warning = 2,
        Exception = 3
    }
    public class Response
    {
        public EResult StatusCode { get; set; }
        public string StatusText => StatusCode.ToString();
        public string? Message { get; set; }
    }

    public class Response<T> : Response
    {
        public T? Data { get; set; }
    }


}
