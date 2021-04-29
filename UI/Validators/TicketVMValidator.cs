using Core.DTOs.ServiceNow;
using Core.Resources;
using FluentValidation;

namespace UI.Validators
{
    public class TicketVMValidator : AbstractValidator<TicketVM>
    {
        public TicketVMValidator()
        {
            RuleFor(r => r.FullName).NotEmpty().When(x => !x.IsLoggedIn).WithMessage($"{Resource.FullName} {Resource.Required}");
            RuleFor(r => r.IdentityNumber).NotEmpty().When(x => !x.IsLoggedIn).WithMessage($"{Resource.IdentityNumber} {Resource.Required}");
            RuleFor(r => r.IdentityNumber).Length(10).WithMessage($"{Resource.IdentityNumber} {Resource.Length_10}");
            RuleFor(r => r.MobileNumber).NotEmpty().When(x => !x.IsLoggedIn).WithMessage($"{Resource.MobileNumber} {Resource.Required}");
            //RuleFor(r => r.MobileNumber).Matches(@"\+(9[976]\d|8[987530]\d|6[987]\d|5[90]\d|42\d|3[875]\d|2[98654321]\d|9[8543210]|8[6421]|6[6543210]|5[87654321]|4[987654310]|3[9643210]|2[70]|7|1)\d{1,14}$").When(x => !x.IsLoggedIn && !string.IsNullOrEmpty(x.MobileNumber)).WithMessage($"{Resource.MobileNumber} {Resource.Invalid}");
            //RuleFor(x => x.MobileNumber.Substring(0, 2)).Equal("05").WithMessage($"{Resource.MobileNumber} {Resource.StartWith_05}");
            RuleFor(r => r.EMail).NotEmpty().When(x => !x.IsLoggedIn).WithMessage($"{Resource.Email} {Resource.Required}");
            RuleFor(r => r.EMail).EmailAddress().WithMessage($"{Resource.Email} {Resource.Invalid}");
            RuleFor(r => r.CategoryId).NotNull().WithMessage($"{Resource.Category} {Resource.Required}");
            RuleFor(r => r.SubCategoryId).NotEmpty().WithMessage($"{Resource.SubCategory} {Resource.Required}");
            RuleFor(r => r.Description).NotEmpty().WithMessage($"{Resource.Desc} {Resource.Required}");
            RuleFor(r => r.ShortDescription).NotEmpty().WithMessage($"{Resource.ShortDesc} {Resource.Required}");
        }
    }
}
