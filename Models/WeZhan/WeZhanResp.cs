namespace ApiCallerDemo.Models.WeZhan
{
    internal class WeZhanResp<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Msg { get; set; }
        public Guid RequestId { get; set; }
    }
}