using Serilog;
using Serilog.Events;
using System;

namespace CB.Services;

    public static class LogService
    {
        private static bool _initialized = false;
        public static void Init()
        {
            if (_initialized) return;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    new Serilog.Formatting.Compact.CompactJsonFormatter(),
                    "db_actions.serilog.json",
                    rollingInterval: RollingInterval.Day)
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
