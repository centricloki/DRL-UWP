using System.IO;
using System.Reflection;

namespace DRLMobile.ExceptionHandler
{
    /// <summary>
    /// This class contains all the constants that are required for error handling
    /// </summary>
    public class ErrorHandlerConstants
    {
        public static string APP_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

        public const string APP_NAME = "Honey";

        // define the file - a resource file
        public const string ERROR_MSG_FILE_NAME = "ErrorMsg.resources";

        // file path of above resource file
        public static string ERROR_MSG_FILE_PATH = APP_PATH + "\\Common\\Resource\\ErrorDictionary\\";

        // The message shown on screen to user in case of any exception
        public const string SYSTEM_ERROR_MSG = "Something went wrong. Please try again.";
    }
}
