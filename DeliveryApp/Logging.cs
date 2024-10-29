using System;
using System.IO;
using System.Windows.Forms;

namespace DeliveryApp
{
    public partial class Logging
    {
        public static void LoggingInfo(string filePath, string message)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Путь к файлу логов не указан");
                return;
            }

            try
            {
                File.AppendAllText(filePath, $"{DateTime.Now} INFO: {message}{Environment.NewLine}");
            }
            catch (IOException ex)
            {
                MessageBox.Show("Ошибка записи в лог: " + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Ошибка записи в лог: " + ex.Message);
            }
        }

        public static void LoggingError(string filePath, string message)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Путь к файлу логов не указан");
                return;
            }

            try
            {
                File.AppendAllText(filePath, $"{DateTime.Now} ERROR: {message}{Environment.NewLine}");
            }
            catch (IOException ex)
            {
                MessageBox.Show("Ошибка записи в лог: " + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Ошибка записи в лог: " + ex.Message);
            }
        }
    }
}