using FluentValidation;
using SaleSystem.Models.Users;

namespace SaleSystem.Validators
{
    public class UserUpdateValidator : AbstractValidator<UserUpdate>
    {
        public readonly int MaxUserNameLength = 256;
        public readonly int MaxUserEmailLength = 256;
        public readonly string emailRegex = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
        public UserUpdateValidator()
        {
            RuleFor(nc => nc.UserName)
                .NotNull()
                .NotEmpty().WithMessage("Comment content can not be empty.")
                .MaximumLength(MaxUserNameLength);

            RuleFor(nc => nc.UserEmail)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty().WithMessage("Comment content can not be empty.")
                .Matches(emailRegex).WithMessage("Incorrect email.")
                .Must(email => !email.Contains(' ')).WithMessage("Can't contains white spaces.")
                .Must(email => email.Split('@').Length == 2).WithMessage("Can't contains white spaces.")
                .MaximumLength(MaxUserEmailLength);
        }
    }
}
