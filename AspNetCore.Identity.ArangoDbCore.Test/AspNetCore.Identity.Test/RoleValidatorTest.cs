using System;
using System.Threading.Tasks;
using AspNetCore.Identity.ArangoDbCore.Test.Shared;
using AspNetCore.Identity.ArangoDbCore.Test.Specification;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace AspNetCore.Identity.ArangoDbCore.Test.AspNetCore.Identity.Test
{
    public class RoleValidatorTest
    {
        [Fact]
        public async Task ValidateThrowsWithNull()
        {
            // Setup
            var validator = new RoleValidator<TestRole>();
            var manager = MockHelpers.TestRoleManager<TestRole>();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>("manager", async () => await validator.ValidateAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await validator.ValidateAsync(manager, null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ValidateFailsWithTooShortRoleName(string input)
        {
            // Setup
            var validator = new RoleValidator<TestRole>();
            var manager = MockHelpers.TestRoleManager<TestRole>();
            var user = new TestRole {Name = input};

            // Act
            var result = await validator.ValidateAsync(manager, user);

            // Assert
            IdentityResultAssert.IsFailure(result, new IdentityErrorDescriber().InvalidRoleName(input));
        }

    }
}
