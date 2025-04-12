
using BeautySpa.ModelViews.AuthModelViews;

  public interface IAuthService
  {
      Task<string?> SignUpAsync(SignUpAuthModelView signup, Guid roleId);
      Task<string?> SignInAsync(SignInAuthModelView signin);
      Task<bool> ChangePasswordAsync(ChangePasswordAuthModelView changepass, Guid UserId);
      Task<IList<string>> GetUserRolesAsync(string email);
  }

