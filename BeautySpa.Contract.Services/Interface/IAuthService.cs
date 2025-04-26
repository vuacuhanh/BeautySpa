
using BeautySpa.ModelViews.AuthModelViews;

  public interface IAuthService
  {
      Task<string?> SignUpAsync(SignUpAuthModelView signup, Guid roleId);
      Task<string?> SignInAsync(SignInAuthModelView signin);
      Task<bool> ChangePasswordAsync(ChangePasswordAuthModelView changepass, Guid UserId);
      Task<IList<string>> GetUserRolesAsync(string email);

      Task<bool> ConfirmEmailAsync(string userId, string token);

    // Gửi lại Email xác nhận (Resend email xác nhận)
      Task<bool> ResendConfirmationEmailAsync(string email);
    // Gửi mail Quên mật khẩu (Forgot password)
      Task<bool> ForgotPasswordAsync(string email);
    // Reset mật khẩu bằng token từ mail (Reset password)
      Task<bool> ResetPasswordAsync(ResetPasswordAuthModelView model);
}

