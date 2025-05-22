using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CB.Services;

    public static class LogService
    {
        private static bool _initialized;

        public static void Init()
        {
            if (_initialized) return;

            // Загрузка конфигурации из appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  // устанавливаем рабочий каталог
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)  // считываем файл
                .Build();

            // Применение конфигурации Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            _initialized = true;
        }

        public static void LogDbAction(string action, string? recipeId = null, string? title = null)
        {
            Log.Information("DB Action {@Action}", new { action, recipeId, title, timestamp = DateTime.UtcNow });
        }

        public static void LogDbError(string method, Exception ex)
        {
            Log.Error(ex, "DB Error in {Method}", method);
        }

        public static void LogDbConnection(string connectionString)
        {
            Log.Information("DB Connection: {ConnectionString}", connectionString);
        }
    }
