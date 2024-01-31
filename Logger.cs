using System;
using System.IO;

namespace PruebaWebhook
{
    public static class Logger
    {
        private static readonly string logFilePath = "Logs/app.log";

        public static void Log(string message)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir en el archivo de log: {ex.Message}");
            }
        }
    }
}
