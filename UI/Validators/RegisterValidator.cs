using FluentValidation;
using Core.DTOs;
using Core.Resources;
using System.Linq;
using System;
using Core;

namespace UI.Validators
{
    public class RegisterValidator:AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage($"{Resource.FirstName} {Resource.Required}");

            RuleFor(x => x.IdentityNumber).NotEmpty().WithMessage($"{Resource.IdentityNumber} {Resource.Required}");

            RuleFor(x => x.EmailAddress).NotEmpty().WithMessage($"{Resource.Email} {Resource.Required}");
            RuleFor(x => x.EmailAddress).EmailAddress().WithMessage($"{Resource.Email} {Resource.Invalid}");

            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage($"{Resource.MobileNumber} {Resource.Required}");

            RuleFor(x => x.FullName).MaximumLength(100).WithMessage($"{Resource.FirstName} {Resource.Max_100}");
            RuleFor(x => x.Address).MaximumLength(150).WithMessage($"{Resource.Address} {Resource.Max_150}");

            RuleFor(x => x.IdentityNumber).Length(10).WithMessage($"{Resource.IdentityNumber} {Resource.Length_10}");
            RuleFor(x => x.IdentityNumber).Must(m => CommonStaticFunctions.ValidateNID(m)).WithMessage($"{Resource.IdentityNumber} {Resource.Invalid}");

            RuleFor(x => x.EmailAddress).MaximumLength(50).WithMessage($"{Resource.Email} {Resource.Max_50}");

            RuleFor(x => x.PhoneNumber).Length(10).WithMessage($"{Resource.MobileNumber} {Resource.Length_10}");
            RuleFor(x => x.PhoneNumber.Substring(0,2)).Equal("05").WithMessage($"{Resource.MobileNumber} {Resource.StartWith_05}");

            RuleFor(x => x.Password).MinimumLength(6).WithMessage($"{Resource.Password} {Resource.MinLength_6}");
            RuleFor(x => x.ConfirmPassword).MinimumLength(6).WithMessage($"{Resource.Confirmation} {Resource.Password} {Resource.MinLength_6}");
        }
    }
}
