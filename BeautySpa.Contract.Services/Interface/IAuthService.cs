
using BeautySpa.ModelViews.AuthModelViews;

  public interface IAuthService
  {
      Task<string?> SignUpAsync(SignUpAuthModelView signup, string roleName);
      Task<string?> SignInAsync(SignInAuthModelView signin);
      Task<bool> ChangePasswordAsync(ChangePasswordAuthModelView changepass, Guid UserId);
      Task<IList<string>> GetUserRolesAsync(string email);
  }

