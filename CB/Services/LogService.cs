using System;
using System.IO;
using System.Threading.Tasks;

namespace CB.Services;

    public static class LogService
    {
        private static readonly string LogFilePath = "db_actions.log";
        private static readonly object _lock = new();

        public static async Task LogAsync(string message)
        {
            try
            {
                var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";
                // Лочим чтобы избежать одновременной записи из разных потоков
                lock (_lock)
                {
                    File.AppendAllText(LogFilePath, line + Environment.NewLine);
                }
            }
            catch { /* Игнорировать ошибки логирования */ }
        }

        public static Task LogDbActionAsync(string action, string recipeId, string? title = null)
            => LogAsync($"{action} | Id={recipeId} | Title={title}");

        public static Task LogDbErrorAsync(string method, Exception ex)
            => LogAsync($"ERROR in {method} | {ex.GetType().Name}: {ex.Message}");

        public static Task LogDbConnectionAsync(string connectionString)
            => LogAsync($"DB CONNECTION | {connectionString}");
    }
