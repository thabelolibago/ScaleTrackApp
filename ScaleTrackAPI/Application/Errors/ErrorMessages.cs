using Microsoft.Extensions.Configuration;
using System;

namespace ScaleTrackAPI.Application.Errors.ErrorMessages
{
    /// <summary>
    /// Provides centralized access to error message templates from configuration (appsettings.json).
    /// Supports nested keys like "General:UnexpectedError".
    /// </summary>
    public static class ErrorMessages
    {
        private static IConfiguration? _configuration;

        /// <summary>
        /// Initialize ErrorMessages with app configuration.
        /// Must be called once during startup (Program.cs).
        /// </summary>
        public static void Init(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Retrieves a specific error message by key.
        /// Supports nested keys separated by colon, e.g., "General:UnexpectedError".
        /// </summary>
        public static string Get(string key)
        {
            if (_configuration == null)
                throw new InvalidOperationException("ErrorMessages is not initialized. Call Init() during startup.");

            // Try to get the value directly
            var message = _configuration[$"ErrorMessages:{key}"];

            if (!string.IsNullOrEmpty(message))
                return message;

            // Attempt to traverse nested sections
            var sections = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
            IConfigurationSection? section = _configuration.GetSection("ErrorMessages");
            foreach (var s in sections)
            {
                section = section.GetSection(s);
                if (!section.Exists())
                    throw new Exception($"Missing error message for key '{key}' in configuration.");
            }

            message = section.Value;
            if (string.IsNullOrEmpty(message))
                throw new Exception($"Missing error message for key '{key}' in configuration.");

            return message;
        }

        /// <summary>
        /// Retrieves a formatted message with arguments.
        /// Example: ErrorMessages.Get("Audit:AuditNotFound", 42)
        /// </summary>
        public static string Get(string key, params object[] args)
        {
            var template = Get(key);
            return string.Format(template, args);
        }
    }
}
