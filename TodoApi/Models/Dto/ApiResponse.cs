namespace TodoApi.Models.Dto
{
    public class ApiResponse<T>
    {
        public bool Status { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public ApiResponse(bool status, T data, string message)
        {
            Status = status;
            Data = data;
            Message = message;
        }
    }
}
