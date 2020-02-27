using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.ArangoDbCore.Test.Shared;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace AspNetCore.Identity.ArangoDbCore.Test.AspNetCore.Identity.Test
{
    public class RoleManagerTest
    {
        [Fact]
        public async Task CreateCallsStore()
        {
            // Setup
            var store = new Mock<IRoleStore<TestRole>>();
            var role = new TestRole { Name = "Foo" };
            store.Setup(s => s.CreateAsync(role, CancellationToken.None)).ReturnsAsync(IdentityResult.Success).Verifiable();
            store.Setup(s => s.GetRoleNameAsync(role, CancellationToken.None)).Returns(Task.FromResult(role.Name)).Verifiable();
            store.Setup(s => s.SetNormalizedRoleNameAsync(role, role.Name.ToUpperInvariant(), CancellationToken.None)).Returns(Task.FromResult(0)).Verifiable();
            var roleManager = MockHelpers.TestRoleManager(store.Object);

            // Act
            var result = await roleManager.CreateAsync(role);

            // Assert
            Assert.True(result.Succeeded);
            store.VerifyAll();
        }

        [Fact]
        public async Task UpdateCallsStore()
        {
            // Setup
            var store = new Mock<IRoleStore<TestRole>>();
            var role = new TestRole { Name = "Foo" };
            store.Setup(s => s.UpdateAsync(role, CancellationToken.None)).ReturnsAsync(IdentityResult.Success).Verifiable();
            store.Setup(s => s.GetRoleNameAsync(role, CancellationToken.None)).Returns(Task.FromResult(role.Name)).Verifiable();
            store.Setup(s => s.SetNormalizedRoleNameAsync(role, role.Name.ToUpperInvariant(), CancellationToken.None)).Returns(Task.FromResult(0)).Verifiable();
            var roleManager = MockHelpers.TestRoleManager(store.Object);

            // Act
            var result = await roleManager.UpdateAsync(role);

            // Assert
            Assert.True(result.Succeeded);
            store.VerifyAll();
        }

        [Fact]
        public void RolesQueryableFailWhenStoreNotImplemented()
        {
            var manager = CreateRoleManager(new NoopRoleStore());
            Assert.False(manager.SupportsQueryableRoles);
            Assert.Throws<NotSupportedException>(() => manager.Roles.Count());
        }

        [Fact]
        public async Task FindByNameCallsStoreWithNormalizedName()
        {
            // Setup
            var store = new Mock<IRoleStore<TestRole>>();
            var role = new TestRole { Name = "Foo" };
            store.Setup(s => s.FindByNameAsync("FOO", CancellationToken.None)).Returns(Task.FromResult(role)).Verifiable();
            var manager = MockHelpers.TestRoleManager(store.Object);

            // Act
            var result = await manager.FindByNameAsync(role.Name);

            // Assert
            Assert.Equal(role, result);
            store.VerifyAll();
        }

        [Fact]
        public async Task CanFindByNameCallsStoreWithoutNormalizedName()
        {
            // Setup
            var store = new Mock<IRoleStore<TestRole>>();
            var role = new TestRole { Name = "Foo" };
            store.Setup(s => s.FindByNameAsync(role.Name, CancellationToken.None)).Returns(Task.FromResult(role)).Verifiable();
            var manager = MockHelpers.TestRoleManager(store.Object);
            manager.KeyNormalizer = null;

            // Act
            var result = await manager.FindByNameAsync(role.Name);

            // Assert
            Assert.Equal(role, result);
            store.VerifyAll();
        }

        [Fact]
        public async Task RoleExistsCallsStoreWithNormalizedName()
        {
            // Setup
            var store = new Mock<IRoleStore<TestRole>>();
            var role = new TestRole { Name = "Foo" };
            store.Setup(s => s.FindByNameAsync("FOO", CancellationToken.None)).Returns(Task.FromResult(role)).Verifiable();
            var manager = MockHelpers.TestRoleManager(store.Object);

            // Act
            var result = await manager.RoleExistsAsync(role.Name);

            // Assert
            Assert.True(result);
            store.VerifyAll();
        }


        [Fact]
        public void DisposeAfterDisposeDoesNotThrow()
        {
            var manager = CreateRoleManager(new NoopRoleStore());
            manager.Dispose();
            manager.Dispose();
        }

        [Fact]
        public async Task RoleManagerPublicNullChecks()
        {
            Assert.Throws<ArgumentNullException>("store",
                () => new RoleManager<TestRole>(null, null, null, null, null));
            var manager = CreateRoleManager(new NoopRoleStore());
            await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await manager.CreateAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await manager.UpdateAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("role", async () => await manager.DeleteAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("roleName", async () => await manager.FindByNameAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>("roleName", async () => await manager.RoleExistsAsync(null));
        }

        [Fact]
        public async Task RoleStoreMethodsThrowWhenDisposed()
        {
            var manager = CreateRoleManager(new NoopRoleStore());
            manager.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.FindByIdAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.FindByNameAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.RoleExistsAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.CreateAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.UpdateAsync(null));
            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.DeleteAsync(null));
        }

        private static RoleManager<TestRole> CreateRoleManager(IRoleStore<TestRole> roleStore)
        {
            return MockHelpers.TestRoleManager(roleStore);
        }



    }
}
