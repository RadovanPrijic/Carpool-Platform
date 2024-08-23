using System.Net;

namespace CarpoolPlatformAPI.Util
{
    public class APIResponse
    {
        private  HttpStatusCode StatusCode { get; set; }
        private bool IsSuccess { get; set; } = true;
        private List<string> ErrorMessages { get; set; } = new List<string>();
        private object? Result { get; set; } = null;
    }
}
