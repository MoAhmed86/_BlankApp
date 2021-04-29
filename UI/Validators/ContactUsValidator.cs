using Core.DTOs.ServiceNow;
using Core.Resources;
using FluentValidation;

namespace UI.Validators
{
    public class ContactUsValidator : AbstractValidator<ContactUsVM>
    {
        public ContactUsValidator()
        {
            RuleFor(r => r.Email).NotEmpty().WithMessage($"{Resource.Email} {Resource.Required}");
            RuleFor(r => r.Email).EmailAddress().WithMessage($"{Resource.Email} {Resource.Invalid}");
            RuleFor(r => r.Email).MaximumLength(100).WithMessage($"{Resource.Email} {Resource.Max_100}");
            RuleFor(r => r.Subject).NotEmpty().WithMessage($"{Resource.Subject} {Resource.Required}");
            RuleFor(r => r.Subject).MaximumLength(500).WithMessage($"{Resource.Subject} {Resource.Max_500}");
        }
    }
}
