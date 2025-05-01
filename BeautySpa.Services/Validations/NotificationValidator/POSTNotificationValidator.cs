using BeautySpa.ModelViews.NotificationModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.NotificationValidator
{
    public class POSTNotificationValidator : AbstractValidator<POSTNotificationModelView>
    {
        public POSTNotificationValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Message).NotEmpty().MaximumLength(500);
            RuleFor(x => x.NotificationType).NotEmpty().MaximumLength(50);
        }
    }
}
