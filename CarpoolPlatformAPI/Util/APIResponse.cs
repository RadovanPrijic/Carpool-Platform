using System.Net;

namespace CarpoolPlatformAPI.Util
{
    public class APIResponse<T> where T : class
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public T? Result { get; set; } = null;

        public APIResponse() { }

        public APIResponse(HttpStatusCode statusCode, bool isSuccess, T? result = null)
        {
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            Result = result;
        }

        public APIResponse(HttpStatusCode statusCode, bool isSuccess, List<string> errorMessages)
        {
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            ErrorMessages = errorMessages;
        }
    }
}
