using CarExpenses.DAL;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using CarExpenses.Web.Api.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http.Json;

namespace CarExpenses.Test
{
    public class UsersApiControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private const int AdminUserId = 12001;
        private const int BasicUserId = 12002;
        private readonly HttpClient _adminClient;
        private readonly HttpClient _basicClient;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public UsersApiControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _adminClient = factory.CreateAuthenticatedClient(roles: AppRoles.Admin, userId: AdminUserId);
            _basicClient = factory.CreateAuthenticatedClient(userId: BasicUserId);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfUsers()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var username = CreateUniqueUsername("list");
            var email = CreateUniqueEmail("list");
            await CreateUserAsync(userManager, username, email, "test123");

            // Act
            var response = await _adminClient.GetAsync("/api/users");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<UserSummaryDto>>();
            Assert.NotNull(dtos);
            Assert.NotEmpty(dtos);
            Assert.Contains(dtos, item => item.Username == username && item.Email == email);
        }

        [Fact]
        public async Task GetAll_ShouldFilterBySearchTerm()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var term = CreateUniqueToken();
            var username = $"search-{term}";
            var email = $"search-{term}@example.com";
            await CreateUserAsync(userManager, username, email, "test123");
            await CreateUserAsync(userManager, CreateUniqueUsername("other"), CreateUniqueEmail("other"), "test123");

            // Act
            var response = await _adminClient.GetAsync($"/api/users?search={Uri.EscapeDataString(term)}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<UserSummaryDto>>();
            Assert.NotNull(dtos);
            Assert.NotEmpty(dtos);
            Assert.Contains(dtos, item => item.Username == username && item.Email == email);

            foreach (var dto in dtos)
            {
                Assert.True(
                    dto.Username.Contains(term) || dto.Email.Contains(term),
                    $"Expected all results to contain search term '{term}'.");
            }
        }

        [Fact]
        public async Task GetAll_ShouldReturnForbidden_WhenNotAdmin()
        {
            // Arrange
            // Act
            var response = await _basicClient.GetAsync("/api/users");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Forbidden, $"Expected 403 Forbidden but got {response.StatusCode}");
        }

        [Fact]
        public async Task GetById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var username = CreateUniqueUsername("by-id");
            var email = CreateUniqueEmail("by-id");
            var user = await CreateUserAsync(userManager, username, email, "test123");

            // Act
            var response = await _adminClient.GetAsync($"/api/users/{user.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dto = await response.Content.ReadFromJsonAsync<UserDetailDto>();
            Assert.NotNull(dto);
            Assert.Equal(user.Id, dto.Id);
            Assert.Equal(username, dto.Username);
            Assert.Equal(email, dto.Email);
        }

        [Fact]
        public async Task GetById_ShouldReturn404_WhenUserNotExists()
        {
            // Arrange
            // Act
            var response = await _adminClient.GetAsync("/api/users/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Create_ShouldReturnUser_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            await EnsureRolesAsync(roleManager);

            var username = CreateUniqueUsername("create");
            var email = CreateUniqueEmail("create");
            var dto = new UserCreateDto
            {
                Username = username,
                Email = email,
                Password = "test123"
            };

            // Act
            var response = await _adminClient.PostAsJsonAsync("/api/users", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
            var created = await response.Content.ReadFromJsonAsync<UserDetailDto>();
            Assert.NotNull(created);
            Assert.Equal(username, created.Username);
            Assert.Equal(email, created.Email);

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var dbUser = await userManager.FindByIdAsync(created.Id.ToString());
            Assert.NotNull(dbUser);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            var dto = new UserCreateDto
            {
                Username = string.Empty,
                Email = "not-an-email",
                Password = string.Empty
            };

            // Act
            var response = await _adminClient.PostAsJsonAsync("/api/users", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await CreateUserAsync(userManager, CreateUniqueUsername("update"), CreateUniqueEmail("update"), "test123");

            var newUsername = CreateUniqueUsername("updated");
            var newEmail = CreateUniqueEmail("updated");
            var dto = new UserUpdateDto
            {
                Username = newUsername,
                Email = newEmail,
                Password = "newpass1"
            };

            // Act
            var response = await _adminClient.PutAsJsonAsync($"/api/users/{user.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(user).ReloadAsync();
            var updated = await userManager.FindByIdAsync(user.Id.ToString());
            Assert.NotNull(updated);
            Assert.Equal(newUsername, updated.UserName);
            Assert.Equal(newEmail, updated.Email);
            var passwordOk = await userManager.CheckPasswordAsync(updated, dto.Password ?? string.Empty);
            Assert.True(passwordOk, "Expected password to be updated.");
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await CreateUserAsync(userManager, CreateUniqueUsername("invalid"), CreateUniqueEmail("invalid"), "test123");

            var dto = new UserUpdateDto
            {
                Username = string.Empty,
                Email = "invalid-email",
                Password = "short"
            };

            // Act
            var response = await _adminClient.PutAsJsonAsync($"/api/users/{user.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturn404_WhenUserNotExists()
        {
            // Arrange
            var dto = new UserUpdateDto
            {
                Username = "missing",
                Email = "missing@example.com",
                Password = "test123"
            };

            // Act
            var response = await _adminClient.PutAsJsonAsync("/api/users/9999", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenUserExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var user = await CreateUserAsync(userManager, CreateUniqueUsername("delete"), CreateUniqueEmail("delete"), "test123");

            // Act
            var response = await _adminClient.DeleteAsync($"/api/users/{user.Id}");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(user).ReloadAsync();
            var deleted = await dbContext.Users.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == user.Id);
            Assert.NotNull(deleted);
            Assert.True(deleted.DeleatedAt.HasValue, "Expected user to be soft deleted.");
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenUserNotExists()
        {
            // Arrange
            // Act
            var response = await _adminClient.DeleteAsync("/api/users/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        private static async Task EnsureRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(AppRoles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(AppRoles.BasicUser))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(AppRoles.BasicUser));
            }
        }

        private static async Task<User> CreateUserAsync(UserManager<User> userManager, string username, string email, string password)
        {
            var user = new User
            {
                UserName = username,
                Email = email
            };

            var result = await userManager.CreateAsync(user, password);
            Assert.True(result.Succeeded, "Expected user creation to succeed.");
            return user;
        }

        private static string CreateUniqueToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static string CreateUniqueUsername(string prefix)
        {
            return $"{prefix}-user-{CreateUniqueToken()}";
        }

        private static string CreateUniqueEmail(string prefix)
        {
            return $"{prefix}-{CreateUniqueToken()}@example.com";
        }
    }
}
