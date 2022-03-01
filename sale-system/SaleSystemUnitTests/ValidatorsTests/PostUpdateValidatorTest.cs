using FluentValidation.TestHelper;
using SaleSystem.Models.Posts;
using SaleSystem.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SaleSystemUnitTests.ValidatorsTests
{
    public class PostUpdateValidatorTest
    {
        [Fact(DisplayName = "Correct data - no validation error")]
        public void WhenPostUpdateHasCorrectData_ShouldNotHasAnyError()
        {
            PostUpdateValidator validator = new PostUpdateValidator();
            PostUpdate post = new PostUpdate
            {
                Title = "title",
                Content = "content"
            };

            var result = validator.TestValidate(post);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Title is null - validation error")]
        public void WhenPostUpdateTitleIsNull_ShoulHasError()
        {
            PostUpdateValidator validator = new PostUpdateValidator();
            PostUpdate post = new PostUpdate
            {
                Content = "content"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Title);
        }

        [Fact(DisplayName = "Content is null - validation error")]
        public void WhenPostUpdateContentIsNull_ShoulHasError()
        {
            PostUpdateValidator validator = new PostUpdateValidator();
            PostUpdate post = new PostUpdate
            {
                Title = "title",
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Content);
        }

        [Fact(DisplayName = "Title is empty - validation error")]
        public void WhenPostUpdateTitleIsEmpty_ShoulHasError()
        {
            PostUpdateValidator validator = new PostUpdateValidator();
            PostUpdate post = new PostUpdate
            {
                Title = "",
                Content = "content"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Title);
        }

        [Fact(DisplayName = "Content is empty - validation error")]
        public void WhenPostUpdateContentIsEmpty_ShoulHasError()
        {
            PostUpdateValidator validator = new PostUpdateValidator();
            PostUpdate post = new PostUpdate
            {
                Title = "title",
                Content = ""
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Content);
        }
        //new 
        [Fact(DisplayName = "Title is too long - validation error")]
        public void WhenPostUpdateTitleIsTooLong_ShoulHasError()
        {
            PostUpdateValidator validator = new PostUpdateValidator();
            PostUpdate post = new PostUpdate
            {
                Title = new string('*', validator.MaxPostTitleLength + 1),
                Content = "content"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Title);
        }

        [Fact(DisplayName = "Content is too long - validation error")]
        public void WhenPostUpdateContentIsTooLong_ShoulHasError()
        {
            PostUpdateValidator validator = new PostUpdateValidator();
            PostUpdate post = new PostUpdate
            {
                Title = "title",
                Content = new string('*', validator.MaxPostContentLength + 1)
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Content);
        }
    }
}
