using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.Management.Automation; // Requires System.Management.Automation.dll
using System.Net.NetworkInformation;
using System.Security.AccessControl;

namespace DRLConsoleApp.CopyNetworkSQLLiteFile
{
    class Program
    {
        //Prod Changes From Manifest
        static string appName = "DRLEnterprises.HoneyforSurface";
        //UAT Changes From Manifest
        //static string appName = "DRLEnterprises.HoneyforTest";

        static string packageFamilyName = null;
        static async Task Main(string[] args)
        {
            try
            {
                // Reset to default color
                Console.ResetColor();
                Console.Clear(); // Clears all text from the console
                Console.WriteLine("Initializing the file download. Please wait...");

                await Task.Run(() => CloseUWPApp());

                //Prod Changes isProd="Y" for Prod
                string isProd = "Y";

                //UAT/Stagging Changes isProd=null for Prod
                //string isProd = null;

                //Copy SQLLite from Network Path on local device at install app location.
                await GetSQLLiteFileAsync((isProd != null && isProd.Equals("Y", StringComparison.OrdinalIgnoreCase)));

                // Launch the app
                await Task.Run(() => LaunchUwpApp());
            }
            catch (Exception ex)
            {
                // Print an error message in Red
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Exception nestedEx = ex?.InnerException;
                while (nestedEx != null)
                {
                    Console.WriteLine(nestedEx.Message);
                    nestedEx = nestedEx?.InnerException;
                }
                // Reset to default color
                Console.ResetColor();

                // Wait for user to press Enter before closing
                Console.WriteLine("Press Enter to close...");
                Console.ReadLine();
            }
        }

        //UAT Changes default is uat.
        static async Task GetSQLLiteFileAsync(bool isProd = false)
        {
            string userFileName = null; string networkPath = null;
            string connectionString = null;
            if (isProd)
            {
                connectionString = @"Data Source=DRLMSS112;Initial Catalog=DRLNew;User ID=honeydbuser;Password=h0n3y1645!;";
                networkPath = @"\\srv-honeysurf\HoneyReplatform\DRLServiceAPI\Content\UserFiles\";
            }
            else
            {
                //Stagging Sever
                //connectionString = @"Data Source=IOSSQLDEV01;Initial Catalog=DRLNew_08272025;User ID=AcornDevDBUser;Password=Corn9875!;";
                //networkPath = @"\\devhoneysurf\HoneyReplatformDev\DRLServiceAPI\Content\UserFiles\";

                //UAT Sever
                connectionString = @"Data Source=IOSSQLDEV01;Initial Catalog=DRLNew_08272025;User ID=AcornDevDBUser;Password=Corn9875!;";
                networkPath = @"\\devhoneysurf\HoneyReplatform\DRLServiceAPI\Content\UserFiles\";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT TOP(1) * FROM dbo.UserMaster WHERE RoleID=6 and len(pin)=4 and IsInActive=0 and IsDeleted=0 ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                userFileName = await reader.GetFieldValueAsync<string>("UserFileName");
                                break;
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(userFileName)) await ExtractSQLliteZipFileAsync(networkPath, userFileName);
                else throw new Exception("No user exists. Please check entered username and pin. ");
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetSQLLiteFileAsync", ex);
            }
        }
        static async Task ExtractSQLliteZipFileAsync(string networkPath, string userFileName)
        {
            // await Task.Run(() => GetInstalledAppPackageName());
            GetInstalledAppPackageName();

            string localAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Packages", packageFamilyName, "LocalState");
            try
            {
                if (Directory.Exists(localAppDataPath))
                {
                    string localZipPath = $"{localAppDataPath}\\{userFileName}";
                    if (File.Exists(localZipPath))
                    {
                        File.Delete(localZipPath);
                    }

                    // Start the file copy with a loading spinner
                    Console.WriteLine("Downloading file, please wait...");

                    await Task.Run(() => CopyFileWithProgress($"{networkPath}\\{userFileName}", localZipPath));

                    Console.WriteLine("ZIP file copied successfully from network to local system.");
                    string localSQLLite = $"{localAppDataPath}\\DRL.sqlite";
                    if (File.Exists(localSQLLite))
                    {
                        File.Delete(localSQLLite);
                    }
                    // Extract the ZIP file to the specified directory
                    ZipFile.ExtractToDirectory(localZipPath, localAppDataPath, true);
                    Console.WriteLine("ZIP file extracted successfully to " + localAppDataPath);

                    File.Delete(localZipPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in ExtractSQLliteZipFileAsync", ex);
            }
        }

        static void CopyFileWithProgress(string sourcePath, string destinationPath)
        {
            const int bufferSize = 1024 * 1024; // 1MB buffer
            byte[] buffer = new byte[bufferSize];
            try
            {
                using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
                using (FileStream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                {
                    long totalBytes = sourceStream.Length;
                    long totalCopied = 0;
                    int bytesRead;

                    while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        destinationStream.Write(buffer, 0, bytesRead);
                        totalCopied += bytesRead;

                        // Calculate progress percentage
                        double percentage = (double)totalCopied / totalBytes * 100;

                        // Display progress
                        Console.Write($"\rDownloading.... {percentage:F2}%");
                    }
                }
                Console.WriteLine("\nCopy completed!");
            }
            catch (Exception ex)
            {
                // Rethrow to be caught in CopyFileWithProgress
                throw new Exception("Error in CopyFileWithProgress", ex);
            }
        }

        //static void GetInstalledAppPackageName()
        //{
        //    try
        //    {
        //        // Start PowerShell in a new process
        //        ProcessStartInfo psi = new ProcessStartInfo
        //        {
        //            FileName = "powershell.exe", // Use Windows PowerShell
        //            Arguments = $"-Command \"Get-AppxPackage -Name '{appName}'\"", // Replace with your app's name
        //            UseShellExecute = false,
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //            CreateNoWindow = true,
        //            Verb = "runas" // Run as administrator
        //        };

        //        using (Process process = Process.Start(psi))
        //        {
        //            // Parse the output to get the PackageFamilyName
        //            string output = process.StandardOutput.ReadToEnd();
        //            if (!string.IsNullOrWhiteSpace(output))
        //            {
        //                packageFamilyName = output.Split('\n').FirstOrDefault(line => line.Contains("PackageFamilyName"));
        //                if (packageFamilyName != null)
        //                {
        //                    packageFamilyName = packageFamilyName.Split(':')[1].Trim();
        //                    Console.WriteLine("Package Family Name: " + packageFamilyName);
        //                }
        //                else
        //                {
        //                    Console.WriteLine("Failed to retrieve PackageFamilyName");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Rethrow to be caught in GetInstalledAppPackageName
        //        throw new Exception("Error in GetInstalledAppPackageName", ex);
        //    }
        //}

        static void GetInstalledAppPackageName()
        {
            try
            {
                string packagesPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Packages");

                if (Directory.Exists(packagesPath))
                {
                    var matchingDirs = Directory.GetDirectories(packagesPath, $"{appName}*");
                    if (matchingDirs.Length > 0)
                    {
                        packageFamilyName = Path.GetFileName(matchingDirs[0]);
                        Console.WriteLine($"Found Package Family Name: {packageFamilyName}");
                        return;
                    }
                }

                throw new Exception($"App package folder for '{appName}' not found in {packagesPath}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error locating app package. Ensure '{appName}' is installed.", ex);
            }
        }

        static void LaunchUwpApp()
        {
            // replace with actual AppUserModelID
            string startPackageFamilyName = $"{packageFamilyName}!App";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"shell:appsFolder\\{startPackageFamilyName}",
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
            Console.WriteLine($"Launch app {appName}");
        }
        static void CloseUWPApp()
        {
            try
            {
                // Get all running processes by name (filter for UWP processes with matching name)
                var processes = Process.GetProcesses().Where(p => p.ProcessName.Contains("DRLMobile.Uwp", StringComparison.OrdinalIgnoreCase));

                foreach (var process in processes)
                {
                    process.Kill();
                    Console.WriteLine($"Closed UWP App Process: {process.ProcessName} (ID: {process.Id})");
                }
            }
            catch (Exception ex)
            {
                // Rethrow to be caught in CloseUWPApp
                throw new Exception("Error in CloseUWPApp", ex);
            }
        }
    }
}
