using FluentValidation;
using Core.DTOs;
using Core.Resources;

namespace UI.Validators
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordModel>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage($"{Resource.Email} {Resource.Required}");
            RuleFor(x => x.Email).EmailAddress().WithMessage($"{Resource.Email} {Resource.Invalid}");

            RuleFor(x => x.Email).MaximumLength(50).WithMessage($"{Resource.Email} {Resource.Max_50}");
        }
    }
}
