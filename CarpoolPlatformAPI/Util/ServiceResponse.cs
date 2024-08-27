using System.Net;

namespace CarpoolPlatformAPI.Util
{
    public class ServiceResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        
        public ServiceResponse() { }

        public ServiceResponse(HttpStatusCode statusCode, T? data)
        {
            StatusCode = statusCode;
            Data = data;
        }

        // For resource deletion (no content response)
        public ServiceResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public ServiceResponse(HttpStatusCode statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
