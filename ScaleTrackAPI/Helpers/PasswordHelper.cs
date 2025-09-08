namespace ScaleTrackAPI.Helpers
{
    public class PasswordHelper
    {
        private readonly string _pepper;

        public PasswordHelper(IConfiguration config)
        {
            _pepper = config["Security:PasswordPepper"] ?? string.Empty;
        }

        public string WithPepper(string password) => password + _pepper;
    }
}
