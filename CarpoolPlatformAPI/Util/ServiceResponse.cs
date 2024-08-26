using System.Net;

namespace CarpoolPlatformAPI.Util
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        
        public ServiceResponse() { }

        public ServiceResponse(HttpStatusCode statusCode, T? data)
        {
            IsSuccess = true;
            StatusCode = statusCode;
            Data = data;
        }

        public ServiceResponse(HttpStatusCode statusCode, string errorMessage)
        {
            IsSuccess = false;
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
