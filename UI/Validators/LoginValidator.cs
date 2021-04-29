using FluentValidation;
using Core.DTOs;
using Core.Resources;

namespace UI.Validators
{
    public class LoginValidator : AbstractValidator<LoginModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage($"{Resource.Email} ({Resource.UserName}) {Resource.Required}");
            RuleFor(x => x.UserName).EmailAddress().WithMessage($"{Resource.Email} ({Resource.UserName}) {Resource.Invalid}");
            RuleFor(x => x.Password).NotEmpty().WithMessage($"{Resource.Password} {Resource.Required}");

            RuleFor(x => x.UserName).MaximumLength(50).WithMessage($"{Resource.Email} {Resource.Max_50}");
            RuleFor(x => x.Password).MaximumLength(15).WithMessage($"{Resource.Password} {Resource.Max_15}");
        }
    }
}
