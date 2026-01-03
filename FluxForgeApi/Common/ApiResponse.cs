namespace FluxForgeApi.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }

        public static ApiResponse<T> Ok(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                Error = null
            };
        }

        public static ApiResponse<T> Fail(string errorMessage)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = null,
                Error = errorMessage
            };
        }
    }
}
