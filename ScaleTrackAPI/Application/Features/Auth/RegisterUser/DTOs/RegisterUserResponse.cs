namespace ScaleTrackAPI.Application.Features.Auth.RegisterUser.DTOs
{
    /// <summary>
    /// Response returned when a user registers but has not verified email yet.
    /// Only contains a friendly message.
    /// </summary>
    public class RegisterUserResponse
    {
        /// <summary>
        /// Friendly message for the client
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
