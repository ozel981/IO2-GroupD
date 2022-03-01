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
    public class PostCreateValidatorTest
    {
        [Fact(DisplayName = "Correct data - no validation error")]
        public void WhenPostCreateHasCorrectData_ShouldNotHasAnyError()
        {
            PostCreateValidator validator = new PostCreateValidator();
            PostCreate post = new PostCreate
            {
                Title = "title",
                Content = "content"
            };

            var result = validator.TestValidate(post);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Title is null - validation error")]
        public void WhenPostCreateTitleIsNull_ShoulHasError()
        {
            PostCreateValidator validator = new PostCreateValidator();
            PostCreate post = new PostCreate
            {
                Content = "content"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Title);
        }

        [Fact(DisplayName = "Content is null - validation error")]
        public void WhenPostCreateContentIsNull_ShoulHasError()
        {
            PostCreateValidator validator = new PostCreateValidator();
            PostCreate post = new PostCreate
            {
                Title = "title",
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Content);
        }

        [Fact(DisplayName = "Title is empty - validation error")]
        public void WhenPostCreateTitleIsEmpty_ShoulHasError()
        {
            PostCreateValidator validator = new PostCreateValidator();
            PostCreate post = new PostCreate
            {
                Title = "",
                Content = "content"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Title);
        }

        [Fact(DisplayName = "Content is empty - validation error")]
        public void WhenPostCreateContentIsEmpty_ShoulHasError()
        {
            PostCreateValidator validator = new PostCreateValidator();
            PostCreate post = new PostCreate
            {
                Title = "title",
                Content = ""
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Content);
        }
        //new 
        [Fact(DisplayName = "Title is too long - validation error")]
        public void WhenPostCreateTitleIsTooLong_ShoulHasError()
        {
            PostCreateValidator validator = new PostCreateValidator();
            PostCreate post = new PostCreate
            {
                Title = new string('*', validator.MaxPostTitleLength + 1),
                Content = "content"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Title);
        }

        [Fact(DisplayName = "Content is too long - validation error")]
        public void WhenPostCreateContentIsTooLong_ShoulHasError()
        {
            PostCreateValidator validator = new PostCreateValidator();
            PostCreate post = new PostCreate
            {
                Title = "title",
                Content = new string('*', validator.MaxPostContentLength + 1)
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(p => p.Content);
        }
    }
}
