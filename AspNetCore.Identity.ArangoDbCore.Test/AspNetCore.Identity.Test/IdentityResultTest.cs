using Microsoft.AspNetCore.Identity;
using Xunit;

namespace AspNetCore.Identity.ArangoDbCore.Test.AspNetCore.Identity.Test
{
    public class IdentityResultTest
    {
        [Fact]
        public void VerifyDefaultConstructor()
        {
            var result = new IdentityResult();
            Assert.False(result.Succeeded);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void NullFailedUsesEmptyErrors()
        {
            var result = IdentityResult.Failed();
            Assert.False(result.Succeeded);
            Assert.Empty(result.Errors);
        }
    }
}
