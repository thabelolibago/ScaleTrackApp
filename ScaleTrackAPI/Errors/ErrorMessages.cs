using Microsoft.Extensions.Configuration;

namespace ScaleTrackAPI.Errors
{
    /// <summary>
    /// Provides centralized access to error message templates from configuration (appsettings.json).
    /// </summary>
    public static class ErrorMessages
    {
        private static IConfiguration? _configuration;

        /// <summary>
        /// Initializes the ErrorMessages class with configuration.
        /// Must be called once during startup (in Program.cs).
        /// </summary>
        public static void Init(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves a specific error message by key from appsettings.json.
        /// Throws if missing.
        /// </summary>
        public static string Get(string key)
        {
            var message = _configuration?[$"ErrorMessages:{key}"];
            if (string.IsNullOrEmpty(message))
                throw new Exception($"Missing error message for key '{key}' in configuration.");
            return message;
        }

        /// <summary>
        /// Retrieves a message and formats it with given arguments.
        /// Example: ErrorMessages.Get("InvalidAssetTransition", oldStatus, newStatus)
        /// </summary>
        public static string Get(string key, params object[] args)
        {
            var template = Get(key);
            return string.Format(template, args);
        }
    }
}
