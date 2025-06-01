using RevelioLog.Core;
using RevelioLog.Core.Enums;
using RevelioLog.Targets.Abstractions;
using System.Text;

namespace RevelioLog.Targets
{
    public class FileTarget : IRevelioLogTarget, IDisposable
    {
        private readonly string _filePath;
        private StreamWriter? _streamWriter;

        public string Name { get; } = "FileTarget";
        public Level MinimumLevel { get; set; } = Level.Information;

        public FileTarget(string filePath)
        {
            _filePath = filePath;
            OpenFile();
        }

        private void OpenFile()
        {
            try
            {
                var directory = Path.GetDirectoryName(_filePath);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                _streamWriter = new StreamWriter(_filePath, append: true, Encoding.UTF8) { AutoFlush = true };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при открытие лог файла {_filePath}: {ex.Message}");
                _streamWriter = null;
            }
        }

        public void Write(LogEntry logEntry)
        {
            if (logEntry.Level >= MinimumLevel && _streamWriter != null)
            {
                try
                {
                    _streamWriter.WriteLine(logEntry.ToString());
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Ошибка записи в файл таргера {_filePath}: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
            _streamWriter = null;
        }
    }
}