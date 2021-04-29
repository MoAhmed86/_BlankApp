using FluentValidation;
using Core.DTOs;
using Core.Resources;

namespace UI.Validators
{
    public class ResetPasswordlValidator : AbstractValidator<ResetPasswordModel>
    {
        public ResetPasswordlValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage($"{Resource.Email} {Resource.Required}");
            RuleFor(x => x.Email).EmailAddress().WithMessage($"{Resource.Email} {Resource.Invalid}");

            RuleFor(x => x.Token).NotEmpty().WithMessage($"{Resource.Token} {Resource.Invalid}");

            RuleFor(x => x.Password).NotEmpty().WithMessage($"{Resource.Password} {Resource.Required}");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage($"{Resource.Confirmation} {Resource.Password} {Resource.Required}");
            RuleFor(x => x.Password).Equal(w => w.ConfirmPassword).WithMessage($"{Resource.Password} {Resource.DoesnotMatch_F}");

            RuleFor(x => x.Email).MaximumLength(50).WithMessage($"{Resource.Email} {Resource.Max_50}");

            RuleFor(x => x.Token).MaximumLength(300).WithMessage($"{Resource.Token} {Resource.Max_300}");

            RuleFor(x => x.Password).MinimumLength(6).WithMessage($"{Resource.Password} {Resource.MinLength_6}");
            RuleFor(x => x.ConfirmPassword).MinimumLength(6).WithMessage($"{Resource.Confirmation} {Resource.Password} {Resource.MinLength_6}");
        }
    }
}
