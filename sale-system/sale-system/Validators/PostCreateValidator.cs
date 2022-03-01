using FluentValidation;
using SaleSystem.Models.Posts;

namespace SaleSystem.Validators
{
    public class PostCreateValidator : AbstractValidator<PostCreate>
    {
        public readonly int MaxPostTitleLength = 256;
        public readonly int MaxPostContentLength = 512;
        public PostCreateValidator()
        {
            RuleFor(nc => nc.Title)
                .NotNull()
                .NotEmpty().WithMessage("Post title can not be empty.")
                .MaximumLength(MaxPostTitleLength);

            RuleFor(nc => nc.Content)
                .NotNull()
                .NotEmpty().WithMessage("Post conteny can not be empty.")
                .MaximumLength(MaxPostContentLength);
        }
    }
}
