using FluentValidation;
using Core.Resources;
using Core.DTOs.Identity;

namespace UI.Validators
{
    public class AppRoleValidator : AbstractValidator<ApplicationRole>
    {
        public AppRoleValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage($"{Resource.RoleName} {Resource.Required}");

            RuleFor(x => x.Name).MaximumLength(50).WithMessage($"{Resource.RoleName} {Resource.Max_50}");
        }
    }
}
