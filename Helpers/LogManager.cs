using System;
using System.IO;

namespace SCustomers.Helpers
{
    public class LogManager
    {
        public static void LogException(Exception ex)
        {
            try
            {
                var logFolder = Path.Combine(CurrentDirectory, LogDirName);

                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }
                var filePath = Path.Combine(logFolder, FileName);

                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filePath))
                {

                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine($"Log Written Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    sw.WriteLine($"Error Type: {ex.GetType()}");
                    sw.WriteLine($"Error Message: {ex.Message}");
                    sw.WriteLine($"Error Line No: {ex.StackTrace}");
                    
                    sw.WriteLine($"InnerError Type: {ex.InnerException?.GetType()}");
                    sw.WriteLine($"InnerError Message: {ex.InnerException?.Message}");
                    sw.WriteLine($"InnerError Line No: {ex.InnerException?.StackTrace}");

                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine("\n\n");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {

            }

        }
        public static string LogDirName
        {
            get { return "logs"; }
        }
        public static string CurrentDirectory
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
        }
        public static string FileName
        {
            get
            {
                return $"log-{DateTime.Now:yyyyMMdd}.txt";
            }
        }
    }
}
