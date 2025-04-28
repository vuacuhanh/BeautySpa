using BeautySpa.Core.Base;
using BeautySpa.ModelViews.AuthModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IAuthService
    {
        Task<BaseResponseModel<string>> SignUpAsync(SignUpAuthModelView model);
        Task<BaseResponseModel<string>> SignUpWithOtpAsync(SignUpAuthModelView model, string otp);
        Task<BaseResponseModel<TokenResponseModelView>> SignInAsync(SignInAuthModelView model);
        Task<BaseResponseModel<TokenResponseModelView>> SignInWithGoogleAsync(SignInWithGoogleModelView model);
        Task<BaseResponseModel<TokenResponseModelView>> SignInWithFacebookAsync(SignInWithFacebookModelView model);
        Task<BaseResponseModel<string>> ForgotPasswordAsync(ForgotPasswordAuthModelView model);
        Task<BaseResponseModel<string>> ResetPasswordAsync(ResetPasswordAuthModelView model);
        Task<BaseResponseModel<string>> ChangePasswordAsync(ChangePasswordAuthModelView model);
        Task<BaseResponseModel<string>> RequestOtpAsync(string email);
        Task<BaseResponseModel<string>> ResendConfirmEmailAsync(string email);
        Task<BaseResponseModel<string>> ConfirmEmailAsync(string userId, string token);
        Task<BaseResponseModel<TokenResponseModelView>> RefreshTokenAsync(RefreshTokenRequestModelView model);
    }
}
