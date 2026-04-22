using System;
using System.Diagnostics;
using System.IO;

using Newtonsoft.Json;

namespace DRLMobile.ExceptionHandler
{
    public static class ErrorLogger
    {
        public static string ApplicationPath { get; set; }

        public static void WriteToErrorLog(string fileName, string methodName, string errDescription, object methodArgument = null)
        {
            try
            {
                var line = Environment.NewLine + Environment.NewLine;

                ///string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "~/ExceptionDetailsFile/";
                string basePath = ApplicationPath + @"\ErrorLogFile\";

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                string ExceptionDetailsFileName = "Error_Log_File_" + DateTime.Today.ToString("dd-MM-yy") + ".txt";

                string ExceptionDetailsFilePath = Path.Combine(basePath, ExceptionDetailsFileName);

                if (!File.Exists(ExceptionDetailsFilePath))
                {
                    File.Create(ExceptionDetailsFilePath).Dispose();
                }

                using (StreamWriter sw = File.AppendText(ExceptionDetailsFilePath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "Error File Name : " + fileName + line + "Error Method Name : " + methodName + line;
                    if (methodArgument != null)
                    {
                        error += "Error Method Argument : "+JsonConvert.SerializeObject(methodArgument, Formatting.Indented) + line;
                    }
                    error += "Error Description : " + errDescription + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void WriteToErrorLog(string fileName, string methodName, Exception objException, object methodArgument = null)
        {
            try
            {
                var line = Environment.NewLine;
                string basePath = ApplicationPath + @"\ErrorLogFile\";

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                string ExceptionDetailsFileName = "Error_Log_File_" + DateTime.Today.ToString("dd-MM-yy") + ".txt";

                string ExceptionDetailsFilePath = Path.Combine(basePath, ExceptionDetailsFileName);

                if (!File.Exists(ExceptionDetailsFilePath))
                {
                    File.Create(ExceptionDetailsFilePath).Dispose();
                }

                using (StreamWriter sw = File.AppendText(ExceptionDetailsFilePath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "Error File Name : " + fileName + line + "Error Method Name : " + methodName + line;
                    if (methodArgument != null)
                    {
                        error += "Error Method Argument : " + JsonConvert.SerializeObject(methodArgument, Formatting.Indented) + line;
                    }
                    if (!string.IsNullOrWhiteSpace(objException.Message))
                        error += "Error Message : " + objException.Message + line;
                    if (!string.IsNullOrWhiteSpace(objException.InnerException?.Message))
                        error += "Error InnerException : " + objException.InnerException.Message + line;
                    error += "Error StackTrace : " + objException.StackTrace + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void WriteHttpErrorLog(string fileName, string methodName, string statusCode, string content, string statusDescription, string errorMessage)
        {
            var line = Environment.NewLine + Environment.NewLine;

            string basePath = ApplicationPath + @"\ErrorLogFile\";

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            string ExceptionDetailsFileName = "Http_Log_File_" + DateTime.Today.ToString("dd-MM-yy") + ".txt";

            string ExceptionDetailsFilePath = Path.Combine(basePath, ExceptionDetailsFileName);

            if (!File.Exists(ExceptionDetailsFilePath))
            {
                File.Create(ExceptionDetailsFilePath).Dispose();
            }

            using (StreamWriter sw = File.AppendText(ExceptionDetailsFilePath))
            {
                string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line +
                    "Error File Name : " + fileName + line +
                    "Error Method Name : " + methodName + line +
                    "Http Status Code : " + statusCode + line +
                    "Http Content : " + content + line +
                    "Http Error Description : " + statusDescription + line +
                    "Http Error Message : " + errorMessage + line;

                sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                sw.WriteLine("-------------------------------------------------------------------------------------");
                sw.WriteLine(line);
                sw.WriteLine(error);
                sw.WriteLine("--------------------------------*End*------------------------------------------");
                sw.WriteLine(line);
                sw.Flush();
                sw.Close();
            }
        }
    }
}