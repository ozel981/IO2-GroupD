using FluentValidation.TestHelper;
using SaleSystem.Models.Users;
using SaleSystem.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SaleSystemUnitTests.ValidatorsTests
{
    public class UserUpdateValidatorTest
    {
        [Fact(DisplayName = "Correct data - no validation error")]
        public void WhenUserUpdateHasCorrectData_ShouldNotHasAnyError()
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            UserUpdate post = new UserUpdate
            {
                UserName = "name",
                UserEmail = "test@email.mailbox.pl"
            };

            var result = validator.TestValidate(post);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Name is null - validation error")]
        public void WhenUserUpdateNameIsNull_ShoulHasError()
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            UserUpdate post = new UserUpdate
            {
                UserEmail = "test@email.mailbox.pl"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(u => u.UserName);
        }

        [Fact(DisplayName = "Name is empty - validation error")]
        public void WhenUserUpdateNameIsEmpty_ShoulHasError()
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            UserUpdate post = new UserUpdate
            {
                UserName = "",
                UserEmail = "test@email.mailbox.pl"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(u => u.UserName);
        }

        [Fact(DisplayName = "Name is too long - validation error")]
        public void WhenUserUpdateNameTooLong_ShoulHasError()
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            UserUpdate post = new UserUpdate
            {
                UserName = new string('*', validator.MaxUserNameLength + 1),
                UserEmail = "test@email.mailbox.pl"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(u => u.UserName);
        }

        [Fact(DisplayName = "Email is null - validation error")]
        public void WhenUserUpdateEmailIsNull_ShoulHasError()
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            UserUpdate post = new UserUpdate
            {
                UserName = "name"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(u => u.UserEmail);
        }

        [Fact(DisplayName = "Email has 2 @ - validation error")]
        public void WhenUserUpdateEmailIsInvalid1_ShoulHasError()
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            UserUpdate post = new UserUpdate
            {
                UserName = "name",
                UserEmail = "wrongmail@wrong@mail.com"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(u => u.UserEmail);
        }

        [Fact(DisplayName = "Email has white spaces - validation error")]
        public void WhenUserUpdateEmailIsInvalid2_ShoulHasError()
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            UserUpdate post = new UserUpdate
            {
                UserName = "name",
                UserEmail = "wr ongmail@wrongmail.com"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(u => u.UserEmail);
        }

        [Fact(DisplayName = "Email is empty - validation error")]
        public void WhenUserUpdateEmailIsEmpty_ShoulHasError()
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            UserUpdate post = new UserUpdate
            {
                UserName = "name",
                UserEmail = ""
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(u => u.UserEmail);
        }

        [Fact(DisplayName = "Email is too long - validation error")]
        public void WhenUserUpdateEmailIsTooLong_ShoulHasError()
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            StringBuilder email = new StringBuilder();
            email.Append('*', validator.MaxUserEmailLength / 2);
            email.Append('*');
            email.Append('*', validator.MaxUserEmailLength / 2);
            email.Append(".pl");
            UserUpdate post = new UserUpdate
            {
                UserName = "name",
                UserEmail = email.ToString()
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(u => u.UserEmail);
        }

        [Fact(DisplayName = "Email has no nick - validation error")]
        public void WhenUserUpdateEmailHasNoNick_ShoulHasError()
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            UserUpdate post = new UserUpdate
            {
                UserName = "name",
                UserEmail = "@email.mailbox.pl"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(u => u.UserEmail);
        }

        [Theory]
        [InlineData("test@")]
        [InlineData("test@mail")]
        public void WhenUserUpdateEmailHasNoDomain_ShoulHasError(string email)
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            UserUpdate post = new UserUpdate
            {
                UserName = "name",
                UserEmail = email
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(u => u.UserEmail);
        }

        [Fact(DisplayName = "Email has no @ sign - validation error")]
        public void WhenUserUpdateEmailHasNoAtSign_ShoulHasError()
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            UserUpdate post = new UserUpdate
            {
                UserName = "name",
                UserEmail = "testemail.pl"
            };

            var result = validator.TestValidate(post);

            result.ShouldHaveValidationErrorFor(u => u.UserEmail);
        }
    }
}
