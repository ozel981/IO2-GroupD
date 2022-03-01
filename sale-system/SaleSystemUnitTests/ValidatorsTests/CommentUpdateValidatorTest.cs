using FluentValidation.TestHelper;
using SaleSystem.Models.Comments;
using SaleSystem.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SaleSystemUnitTests.ValidatorsTests
{
    public class CommentUpdateValidatorTest
    {
        [Fact(DisplayName = "Correct data - no validation error")]
        public void WhenCommentUpdateHasCorrectData_ShouldNotHasAnyError()
        {
            CommentUpdateValidator validator = new CommentUpdateValidator();
            CommentUpdate comment = new CommentUpdate
            {
                Content = "test"
            };

            var result = validator.TestValidate(comment);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Content is null - validation error")]
        public void WhenCommentUpdateContentIsNull_ShoulHasError()
        {
            CommentUpdateValidator validator = new CommentUpdateValidator();
            CommentUpdate comment = new CommentUpdate { };

            var result = validator.TestValidate(comment);

            result.ShouldHaveValidationErrorFor(c => c.Content);
        }

        [Fact(DisplayName = "Content is empty - validation error")]
        public void WhenCommentUpdateContentIsEmpty_ShoulHasError()
        {
            CommentUpdateValidator validator = new CommentUpdateValidator();
            CommentUpdate comment = new CommentUpdate
            {
                Content = ""
            };

            var result = validator.TestValidate(comment);

            result.ShouldHaveValidationErrorFor(c => c.Content);
        }

        [Fact(DisplayName = "Content is too long - validation error")]
        public void WhenCommentUpdateContentIsTooLong_ShoulHasError()
        {
            CommentUpdateValidator validator = new CommentUpdateValidator();
            CommentUpdate comment = new CommentUpdate
            {
                Content = new string('*', validator.MaxCommentLength + 1)
            };

            var result = validator.TestValidate(comment);

            result.ShouldHaveValidationErrorFor(c => c.Content);
        }
    }
}
