using FluentValidation;
using SaleSystem.Models.Comments;

namespace SaleSystem.Validators
{
    public class CommentUpdateValidator : AbstractValidator<CommentUpdate>
    {
        public readonly int MaxCommentLength = 512;
        public CommentUpdateValidator()
        {
            RuleFor(nc => nc.Content)
                .NotNull()
                .NotEmpty().WithMessage("Comment content can not be empty.")
                .MaximumLength(MaxCommentLength);
        }
    }
}
