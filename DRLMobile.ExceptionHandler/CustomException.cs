using System;

namespace DRLMobile.ExceptionHandler
{
    public class CustomException : ApplicationException
    {
        private string _errorDescription = string.Empty;
        public string ErrorDescription
        {
            get { return _errorDescription; }
        }

        public CustomException() : base() { }

        public CustomException(string pstrErrorDescription) : base(pstrErrorDescription)
        {
            _errorDescription = pstrErrorDescription;

        }
    }
}
