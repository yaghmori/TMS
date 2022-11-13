using System.Net;
using System.Reflection;

namespace TMS.Shared.Responses
{
    public class MainResponse
    {
        public MainResponse()
        {

        }

        public MainResponse(bool isSuccess, object content, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
            Content = content;
        }
        public MainResponse(bool isSuccess, object content)
        {
            IsSuccess = isSuccess;
            Content = content;
        }
        public MainResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; set; } = false;
        public string? Message { get; set; } = null;
        public object? Content { get; set; } = null;
        public string? ContentType { get { return Content?.GetType().FullName; } } //Replace($"{nameof(TMS)}.{nameof(Shared)}.{nameof(DTO)}.","").Replace($"{nameof(TMS)}.{nameof(Shared)}.", "")
    }
}
