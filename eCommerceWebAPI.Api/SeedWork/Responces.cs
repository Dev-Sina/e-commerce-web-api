namespace eCommerceWebAPI.Api.SeedWork
{
    public interface IResponse<T>
    {
        string Status { get; set; }
        string Message { get; set; }
        T Data { get; set; }
    }

    public interface IResponseList<T> where T : class
    {
        string Status { get; set; }
        string Message { get; set; }
        int Count { get; set; }
        List<T> Data { get; set; }
    }

    public class Response : IResponse<object>
    {
        public List<string> Errors { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class CustomResponse<T> : IResponse<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public long ResultCount { get; set; }
    }

    public class CustomResponseList<T> : IResponseList<T> where T : class
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public int Count { get; set; }
        public List<T> Data { get; set; }
    }
}
