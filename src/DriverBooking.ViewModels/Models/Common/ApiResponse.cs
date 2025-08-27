namespace DriverBooking.Core.Models.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public List<string>? Errors { get; set; }

        public static ApiResponse<T> CreateSuccessResponse(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> CreateFailureResponseWithoutError(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
            };
        }

        public static ApiResponse<T> CreateFailureResponseWithErrors(List<string> errors, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
