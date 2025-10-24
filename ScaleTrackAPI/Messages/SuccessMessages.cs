using Microsoft.Extensions.Configuration;

namespace ScaleTrackAPI.Messages
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
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves a specific success message by key from appsettings.json.
        /// Throws if missing.
        /// </summary>
        public static string Get(string key)
        {
            var message = _configuration?[$"SuccessMessages:{key}"];
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
