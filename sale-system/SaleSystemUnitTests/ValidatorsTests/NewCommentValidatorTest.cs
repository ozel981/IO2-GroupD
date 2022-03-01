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
    public class NewCommentValidatorTest
    {
        [Fact(DisplayName = "Correct data - no validation error")]
        public void WhenNewCommentHasCorrectData_ShouldNotHasAnyError()
        {
            NewCommentValidator validator = new NewCommentValidator();
            NewComment comment = new NewComment
            {
                PostID = 0,
                Content = "test"
            };

            var result = validator.TestValidate(comment);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Content is null - validation error")]
        public void WhenNewCommentContentIsNull_ShoulHasError()
        {
            NewCommentValidator validator = new NewCommentValidator();
            NewComment comment = new NewComment
            {
                PostID = 0,
            };

            var result = validator.TestValidate(comment);

            result.ShouldHaveValidationErrorFor(c => c.Content);
        }

        [Fact(DisplayName = "Content is empty - validation error")]
        public void WhenNewCommentContentIsEmpty_ShoulHasError()
        {
            NewCommentValidator validator = new NewCommentValidator();
            NewComment comment = new NewComment
            {
                PostID = 0,
                Content = ""
            };

            var result = validator.TestValidate(comment);

            result.ShouldHaveValidationErrorFor(c => c.Content);
        }

        [Fact(DisplayName = "Content is too long - validation error")]
        public void WhenNewCommentContentIsTooLong_ShoulHasError()
        {
            NewCommentValidator validator = new NewCommentValidator();
            NewComment comment = new NewComment
            {
                PostID = 0,
                Content = new string('*', validator.MaxCommentLength + 1)
            };

            var result = validator.TestValidate(comment);

            result.ShouldHaveValidationErrorFor(c => c.Content);
        }
    }
}
