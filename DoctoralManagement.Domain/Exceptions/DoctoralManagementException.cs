using System.Net;

namespace DoctoralManagement.Domain.Exceptions
{
    public class DoctoralManagementException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public DoctoralManagementException() { }

        public DoctoralManagementException(string message)
            : base(message)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public DoctoralManagementException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public DoctoralManagementException(string message, int statusCode)
            : base(message)
        {
            StatusCode = (HttpStatusCode)statusCode;
        }
    }
}
