using System;
using System.IO;

namespace DRLMobile.ExceptionHandler
{
    public static class ErrorHandler
    {
        #region File Constant 
        private const string FILE_NAME = "ErrorHandler";
        #endregion

        public static void LogAndThrowSystemException(string fileName, string methodName, string errDescription)
        {
            string lErrDesc = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(errDescription.Trim()))
                {
                    // Write the error details to error logger
                    ErrorLogger.WriteToErrorLog(fileName, methodName, errDescription);
                }

                GetLocalErrorDetails(ref lErrDesc);

                if (!string.IsNullOrEmpty(errDescription.Trim()))
                {
                    //If error occurs in error logging, get addon error message
                    string AddOnErrDesc = string.Empty;

                    GetLocalErrorDetails(ref AddOnErrDesc);

                    lErrDesc += Environment.NewLine + "[" + AddOnErrDesc + "]";
                }

                // Throws system error message.
                throw new CustomException(lErrDesc);
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogAndThrowStandardSystemException(FILE_NAME, "LogAndThrowSystemException", "Exception Message:" + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
        }

        // This method will write Error Log and Throw specified Error Exception
        public static void LogAndThrowSpecifiedException(string fileName, string methodName, string errDescription)
        {
            string lErrDesc = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(errDescription.Trim()))
                {
                    // Write the error details to error logger
                    ErrorLogger.WriteToErrorLog(fileName, methodName, errDescription);
                }

                GetLocalErrorDetails(ref lErrDesc);

                if (!string.IsNullOrEmpty(errDescription.Trim()))
                {
                    //If error occurs in error logging, get addon error message
                    string AddOnErrDesc = string.Empty;

                    GetLocalErrorDetails(ref AddOnErrDesc);

                    lErrDesc += Environment.NewLine + "[" + AddOnErrDesc + "]";
                }

                // Throws system error message.
                throw new CustomException(lErrDesc);
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogAndThrowStandardSystemException(FILE_NAME, "LogAndThrowSpecifiedException", "Exception Message:" + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
        }

        // Writes Error Log with error trace and Throws specified Error Exception
        public static void LogAndThrowSpecifiedException(string fileName, string methodName, Exception objException)
        {
            string lErrDesc = string.Empty;

            try
            {
                if (objException != null && !string.IsNullOrEmpty(objException.Message))
                {
                    ErrorLogger.WriteToErrorLog(fileName, methodName, "Exception Message:" + objException.Message + "\r\nStackTrace: " + objException.StackTrace);
                }

                GetLocalErrorDetails(ref lErrDesc);

                if (!string.IsNullOrEmpty(objException.Message))
                {
                    //If error occurs in error logging,get addon error message
                    string AddOnErrDesc = string.Empty;

                    GetLocalErrorDetails(ref AddOnErrDesc);

                    lErrDesc += Environment.NewLine + "[" + AddOnErrDesc + "]";
                }

                //Throws specified Error/informative message
                throw new CustomException(lErrDesc);
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogAndThrowStandardSystemException(FILE_NAME, "LogAndThrowSpecifiedException", "Exception Message:" + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
        }

        // This method will writes Error Log with error trace.
        public static void LogException(string fileName, string methodName, Exception objException)
        {
            try
            {
                if (objException != null && !string.IsNullOrEmpty(objException.Message))
                {
                    // write error details in the Error Log file
                    ErrorLogger.WriteToErrorLog(fileName, methodName, "Exception Message:" + objException.Message + "\r\nStackTrace: " + objException.StackTrace);
                }

                if (objException.Message != null)
                {
                    //If error occurs in error logging,get addon error message
                    string AddOnErrDesc = string.Empty;

                    GetLocalErrorDetails(ref AddOnErrDesc);

                    ShowErrorMessagePopUp(AddOnErrDesc);
                }
            }
            catch (Exception ex)
            {
                LogAndThrowStandardSystemException(FILE_NAME, "LogException","Exception Message:" + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
        }

        // This method will writes Error Log with error trace.
        public static void LogException(string fileName, string methodName, string errDescription)
        {
            try
            {
                if (!string.IsNullOrEmpty(errDescription.Trim()))
                {
                    // write error details in the Error Log file
                    ErrorLogger.WriteToErrorLog(fileName, methodName, errDescription);
                }

                if (!string.IsNullOrEmpty(errDescription.Trim()))
                {
                    //If error occurs in error logging,get addon error message
                    string AddOnErrDesc = string.Empty;

                    GetLocalErrorDetails(ref AddOnErrDesc);

                    ShowErrorMessagePopUp(AddOnErrDesc);
                }
            }
            catch (Exception ex)
            {
                LogAndThrowStandardSystemException(FILE_NAME, "LogException", "Exception Message:" + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
        }

        #region Private Methods

        private static void GetLocalErrorDetails(ref string errDesc)
        {
            try
            {
                errDesc = ErrorHandlerConstants.SYSTEM_ERROR_MSG;
            }
            catch (FileNotFoundException)
            {
                // If can not read from resource file throw hard coded system error message.
                throw new CustomException(ErrorHandlerConstants.SYSTEM_ERROR_MSG);
            }
            catch (Exception ex)
            {
                LogAndThrowStandardSystemException(FILE_NAME, "GetLocalErrorDetails", "Exception Message: Error in reading error description. Decs:" +ex.Message + "\r\nStackTrace:" + ex.StackTrace);
            }
        }

        private static void LogAndThrowStandardSystemException(string fileName, string methodName, string errDescription)
        {
            try
            {
                if (!string.IsNullOrEmpty(errDescription.Trim()))
                {
                    try
                    {
                        ErrorLogger.WriteToErrorLog(fileName, methodName, errDescription);
                    }
                    catch (Exception)
                    {
                       // ShowFullSystemException(fileName, methodName);
                    }
                }

                if (!string.IsNullOrEmpty(errDescription.Trim()))
                {
                    string addOnErrorDesc = string.Empty;

                    GetLocalErrorDetails(ref addOnErrorDesc);

                    //Show standard system error message 
                    ShowErrorMessagePopUp(ErrorHandlerConstants.SYSTEM_ERROR_MSG + Environment.NewLine + "[" + addOnErrorDesc + "]");
                }
                else
                {
                    //Show standard system Error message
                    ShowErrorMessagePopUp(ErrorHandlerConstants.SYSTEM_ERROR_MSG);
                }
            }
            catch (Exception)
            {
               
            }
        }

        private static void ShowErrorMessagePopUp(string ErrorDescription)
        {
          
        }

        #endregion
    }
}