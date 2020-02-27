using System;
using System.Threading.Tasks;
using AspNetCore.Identity.ArangoDbCore.Test.Shared;
using AspNetCore.Identity.ArangoDbCore.Test.Specification;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace AspNetCore.Identity.ArangoDbCore.Test.AspNetCore.Identity.Test
{
    public class UserValidatorTest
    {
        [Fact]
        public async Task ValidateThrowsWithNull()
        {
            // Setup
            var manager = MockHelpers.TestUserManager(new NoopUserStore());
            var validator = new UserValidator<TestUser>();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>("manager", () => validator.ValidateAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => validator.ValidateAsync(manager, null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ValidateFailsWithTooShortUserNames(string input)
        {
            // Setup
            var manager = MockHelpers.TestUserManager(new NoopUserStore());
            var validator = new UserValidator<TestUser>();
            var user = new TestUser {UserName = input};

            // Act
            var result = await validator.ValidateAsync(manager, user);

            // Assert
            IdentityResultAssert.IsFailure(result, new IdentityErrorDescriber().InvalidUserName(input));
        }

        [Theory]
        [InlineData("test_email@foo.com", true)]
        [InlineData("hao", true)]
        [InlineData("test123", true)]
        [InlineData("hyphen-yes@foo-bar.com", true)]
        [InlineData("+plus+yes+@foo-bar.com", true)]
        [InlineData("!noway", false)]
        [InlineData("foo@boz#.com", false)]
        public async Task DefaultAlphaNumericOnlyUserNameValidation(string userName, bool expectSuccess)
        {
            // Setup
            var manager = MockHelpers.TestUserManager(new NoopUserStore());
            var validator = new UserValidator<TestUser>();
            var user = new TestUser {UserName = userName};

            // Act
            var result = await validator.ValidateAsync(manager, user);

            // Assert
            if (expectSuccess)
            {
                IdentityResultAssert.IsSuccess(result);
            }
            else
            {
                IdentityResultAssert.IsFailure(result);
            }
        }

        [Theory]
        [InlineData("test_email@foo.com", true)]
        [InlineData("hao", true)]
        [InlineData("test123", true)]
        [InlineData("!noway", true)]
        [InlineData("foo@boz#.com", true)]
        public async Task CanAllowNonAlphaNumericUserName(string userName, bool expectSuccess)
        {
            // Setup
            var manager = MockHelpers.TestUserManager(new NoopUserStore());
            manager.Options.User.AllowedUserNameCharacters = null;
            var validator = new UserValidator<TestUser>();
            var user = new TestUser {UserName = userName};

            // Act
            var result = await validator.ValidateAsync(manager, user);

            // Assert
            if (expectSuccess)
            {
                IdentityResultAssert.IsSuccess(result);
            }
            else
            {
                IdentityResultAssert.IsFailure(result);
            }
        }

    }
}
