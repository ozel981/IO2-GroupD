using FluentValidation;
using SaleSystem.Models.Comments;

namespace SaleSystem.Validators
{
    public class NewCommentValidator : AbstractValidator<NewComment>
    {
        public readonly int MaxCommentLength = 512;
        public NewCommentValidator()
        {
            RuleFor(nc => nc.Content)
                .NotNull()
                .NotEmpty().WithMessage("Comment content can not be empty.")
                .MaximumLength(MaxCommentLength);
        }
    }
}
