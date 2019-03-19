using System;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace BuildTrackerApi.Services
{
    [Serializable]
    internal class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; internal set; }
        public AppException()
        {
            this.StatusCode = HttpStatusCode.InternalServerError;
        }

        public AppException(string message, HttpStatusCode _statusCode = HttpStatusCode.InternalServerError) : base(message)
        {
            this.StatusCode = _statusCode;
        }

        public AppException(string message, Exception innerException) : base(message, innerException)
        {
            this.StatusCode = HttpStatusCode.InternalServerError;
        }

        protected AppException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.StatusCode = HttpStatusCode.InternalServerError;
        }
        public AppException(string message, params object[] args)
          : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
            this.StatusCode = HttpStatusCode.InternalServerError;
        }
    }
}