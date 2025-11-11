using Microsoft.Extensions.Configuration;
using System;

namespace ScaleTrackAPI.Application.Messages.SuccessMessages
{
    /// <summary>
    /// Provides centralized access to success message templates from configuration (appsettings.json).
    /// </summary>
    public static class SuccessMessages
    {
        private static IConfiguration? _configuration;

        /// <summary>
        /// Initializes the SuccessMessages class with configuration.
        /// Must be called once during startup (in Program.cs).
        /// </summary>
        public static void Init(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Retrieves a specific success message by key from appsettings.json.
        /// Throws if missing.
        /// </summary>
        public static string Get(string key)
        {
            if (_configuration == null)
                throw new InvalidOperationException("SuccessMessages is not initialized. Call Init() during startup.");

            var message = _configuration[$"SuccessMessages:{key}"];

            if (string.IsNullOrEmpty(message))
                throw new Exception($"Missing success message for key '{key}' in configuration.");

            return message;
        }

        /// <summary>
        /// Retrieves a message and formats it with given arguments.
        /// Example: SuccessMessages.Get("CommentDeleted")
        /// </summary>
        public static string Get(string key, params object[] args)
        {
            var template = Get(key);
            return string.Format(template, args);
        }
    }
}
