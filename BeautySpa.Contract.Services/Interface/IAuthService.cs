using BeautySpa.ModelViews.AuthModelViews;

public interface IAuthService
{
    Task<string?> SignUpAsync(SignUpAuthModelView signup, Guid roleId);
    Task<string?> SignInAsync(SignInAuthModelView signin);
    Task<bool> ChangePasswordAsync(ChangePasswordAuthModelView changepass, Guid UserId);
    Task<IList<string>> GetUserRolesAsync(string email);
    Task<bool> ConfirmEmailAsync(string userId, string token);
    Task<bool> ResendConfirmationEmailAsync(string email);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(ResetPasswordAuthModelView model);
    Task<bool> RequestOtpAsync(string email);
    Task<string> SignUpWithOtpAsync(SignUpAuthModelView signup, string otp);
}