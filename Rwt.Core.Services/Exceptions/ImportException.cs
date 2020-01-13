using System;

namespace Rwt.Core.Services.Exceptions
{
    public class ImportException : Exception
    {
        public ImportException()
            : base()
        {
        }

        public ImportException(string message)
            : base(message)
        {
        }

        public ImportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
