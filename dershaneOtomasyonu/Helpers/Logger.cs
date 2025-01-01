using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Helpers
{
    public interface ILogger
    {
        Task Info(string message);
        Task Warn(string message);
        Task Error(string message, Exception ex = null);
        Task Debug(string message);
    }

    public class Logger : ILogger
    {
        private readonly IBaseRepository<LogEntry> _logRepository;

        public Logger(IBaseRepository<LogEntry> logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task Info(string message)
        {
            await LogAsync("INFO", message);
        }

        public async Task Warn(string message)
        {
            await LogAsync("WARN", message);
        }

        public async Task Error(string message, Exception ex = null)
        {
            var fullMessage = ex == null ? message : $"{message} | Exception: {ex.Message} | StackTrace: {ex.StackTrace}";
            await LogAsync("ERROR", fullMessage);
        }

        public async Task Debug(string message)
        {
            await LogAsync("DEBUG", message);
        }

        private async Task LogAsync(string level, string message)
        {
            var logEntry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Message = message,
                IpAddress = NetworkHelper.GetLocalIPAddress(),
                KullaniciId = GlobalData.Kullanici?.Id ?? null
            };

            try
            {
                await _logRepository.AddAsync(logEntry);
            }
            catch (Exception ex)
            {
                // Eğer log kaydı yapılmazsa fallback olarak konsola yaz
                Console.WriteLine($"Failed to log to database: {ex.Message}");
                Console.WriteLine($"[{level}] {message}");
            }
        }
    }

}
