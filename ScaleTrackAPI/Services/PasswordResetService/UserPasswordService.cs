using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Models;

public class UserPasswordService
{
    private readonly UserManager<User> _userManager;
    private readonly PasswordHelper _passwordHelper;

    public UserPasswordService(UserManager<User> userManager, PasswordHelper passwordHelper)
    {
        _userManager = userManager;
        _passwordHelper = passwordHelper;
    }

    public async Task ChangePasswordAsync(User user, string newPassword)
    {
        var pepperedPassword = _passwordHelper.WithPepper(newPassword);

        var removeResult = await _userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            var errors = string.Join("; ", removeResult.Errors.Select(e => e.Description));
            throw new Exception(ErrorMessages.Get("User:FailedToDeleteUser") + $": {errors}");
        }

        var addResult = await _userManager.AddPasswordAsync(user, pepperedPassword);
        if (!addResult.Succeeded)
        {
            var errors = string.Join("; ", addResult.Errors.Select(e => e.Description));
            throw new Exception(ErrorMessages.Get("User:FailedToAddPassword") + $": {errors}");
        }
    }
}
