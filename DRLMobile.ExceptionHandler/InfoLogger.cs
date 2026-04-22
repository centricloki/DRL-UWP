using System;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace DRLMobile.ExceptionHandler
{
    public class InfoLogger
    {
        private InfoLogger() { }
        private static readonly Lazy<InfoLogger> lazy = new Lazy<InfoLogger>(() => new InfoLogger());
        public static InfoLogger GetInstance => lazy.Value;

        public string ApplicationPath { get; set; }
        public string UserId { get; set; }
        public async Task WriteToLogAsync(string SourceName, object SourceArgument = null, string CustomeMessage = null)
        {
            try
            {
                var line = Environment.NewLine;
                string basePath = ApplicationPath + @"\ErrorLogFile\";

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                string loggerFileName = "Info_Log_File_";
                loggerFileName += $"U{UserId}_";
                loggerFileName += $"{DateTime.Today.ToString("dd-MM-yy")}.txt";

                string loggerFilePath = Path.Combine(basePath, loggerFileName);

                if (!File.Exists(loggerFilePath))
                {
                    File.Create(loggerFilePath).Dispose();
                }

                using (StreamWriter sw = File.AppendText(loggerFilePath))
                {
                    string logMessage = $"Logged On:- {DateTime.Now}" + line;
                    logMessage = $"Source Name:- {SourceName}" + line;
                    if (!string.IsNullOrWhiteSpace(CustomeMessage))
                        logMessage += "Message:- " + CustomeMessage + line;

                    if (SourceArgument != null)
                        logMessage += "Source Argument:- " + JsonConvert.SerializeObject(SourceArgument, Formatting.Indented) + line;

                    logMessage += $"--------------------------------*End {SourceName} Here *------------------------------------------" + line;

                    await sw.WriteLineAsync(logMessage).ConfigureAwait(false);
                    await sw.FlushAsync().ConfigureAwait(false);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}