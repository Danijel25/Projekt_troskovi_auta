using CarExpenses.DAL;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using CarExpenses.Web.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace CarExpenses.Test
{
    public class ExpenseCategoriesApiControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly HttpClient _adminClient;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public ExpenseCategoriesApiControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateAuthenticatedClient();
            _adminClient = factory.CreateAuthenticatedClient(roles: AppRoles.Admin);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfCategories()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            await CreateExpenseCategory(dbContext, "Fuel");
            await CreateExpenseCategory(dbContext, "Service");

            // Act
            var response = await _client.GetAsync("/api/expense-categories");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<ExpenseCategoryDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<ExpenseCategoryDto>>(dtos);
            Assert.NotEmpty(dtos);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyListOfCategories()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            dbContext.ExpenseCategories.RemoveRange(dbContext.ExpenseCategories.IgnoreQueryFilters());
            await dbContext.SaveChangesAsync();
            // Act
            var response = await _client.GetAsync("/api/expense-categories");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dtos = await response.Content.ReadFromJsonAsync<List<ExpenseCategoryDto>>();
            Assert.NotNull(dtos);
            Assert.IsType<List<ExpenseCategoryDto>>(dtos);
            Assert.Empty(dtos);
        }

        [Fact]
        public async Task GetById_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var category = await CreateExpenseCategory(dbContext, "Insurance");

            // Act
            var response = await _client.GetAsync($"/api/expense-categories/{category.Id}");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected a successful status code but got {response.StatusCode}");
            var dto = await response.Content.ReadFromJsonAsync<ExpenseCategoryDetailDto>();
            Assert.NotNull(dto);
            Assert.IsType<ExpenseCategoryDetailDto>(dto);
            Assert.True(dto.Id == category.Id, $"Expected ID {category.Id} but got {dto.Id}");
        }

        [Fact]
        public async Task GetById_ShouldReturn404_WhenCategoryNotExists()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/api/expense-categories/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Create_ShouldReturnCategory_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var dto = CreateValidExpenseCategoryCreateDto();

            // Act
            var response = await _adminClient.PostAsJsonAsync("/api/expense-categories", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected 201 Created but got {response.StatusCode}");
            var created = await response.Content.ReadFromJsonAsync<ExpenseCategoryDetailDto>();
            Assert.NotNull(created);
            Assert.Equal(dto.Name, created.Name);

            var dbCategory = await dbContext.ExpenseCategories.FirstOrDefaultAsync(item => item.Id == created.Id);
            Assert.NotNull(dbCategory);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            var dto = CreateInvalidExpenseCategoryCreateDto();

            // Act
            var response = await _adminClient.PostAsJsonAsync("/api/expense-categories", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Create_ShouldReturnForbidden_WhenWrongRole()
        {
            // Arrange
            var dto = CreateInvalidExpenseCategoryCreateDto();

            // Act
            var response = await _client.PostAsJsonAsync("/api/expense-categories", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Forbidden, $"Expected 403 Forbidden but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var category = await CreateExpenseCategory(dbContext, "Parking");
            var dto = CreateValidExpenseCategoryUpdateDto();

            // Act
            var response = await _adminClient.PutAsJsonAsync($"/api/expense-categories/{category.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");
            await dbContext.Entry(category).ReloadAsync();
            Assert.Equal(dto.Name, category.Name);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var category = await CreateExpenseCategory(dbContext, "Maintenance");
            var dto = CreateInvalidExpenseCategoryUpdateDto();

            // Act
            var response = await _adminClient.PutAsJsonAsync($"/api/expense-categories/{category.Id}", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Expected 400 Bad Request but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturn404_WhenCategoryNotExists()
        {
            // Arrange
            var dto = CreateValidExpenseCategoryUpdateDto();

            // Act
            var response = await _adminClient.PutAsJsonAsync("/api/expense-categories/9999", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Update_ShouldReturn403_WhenCategoryNotExists()
        {
            // Arrange
            var dto = CreateValidExpenseCategoryUpdateDto();

            // Act
            var response = await _client.PutAsJsonAsync("/api/expense-categories/9999", dto);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Forbidden, $"Expected 403 Forbidden but got {response.StatusCode}");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenCategoryExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarExpesesDbContext>();
            var category = await CreateExpenseCategory(dbContext, "Registration");

            // Act
            var response = await _adminClient.DeleteAsync($"/api/expense-categories/{category.Id}");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NoContent, $"Expected 204 No Content but got {response.StatusCode}");

            await dbContext.Entry(category).ReloadAsync();
            var dbCategory = await dbContext.ExpenseCategories
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(item => item.Id == category.Id);
            Assert.NotNull(dbCategory);
            Assert.True(dbCategory.DeleatedAt.HasValue, "Expected category to be soft deleted.");
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenCategoryNotExists()
        {
            // Arrange
            // Act
            var response = await _adminClient.DeleteAsync("/api/expense-categories/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected 404 Not Found but got {response.StatusCode}");
        }

        [Fact]
        public async Task Delete_ShouldReturn403_WhenCategoryNotExists()
        {
            // Arrange
            // Act
            var response = await _client.DeleteAsync("/api/expense-categories/9999");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Forbidden, $"Expected 403 Forbidden but got {response.StatusCode}");
        }

        private static ExpenseCategoryCreateDto CreateValidExpenseCategoryCreateDto()
        {
            return new ExpenseCategoryCreateDto
            {
                Name = "Tolls"
            };
        }

        private static ExpenseCategoryUpdateDto CreateValidExpenseCategoryUpdateDto()
        {
            return new ExpenseCategoryUpdateDto
            {
                Name = "Updated Category"
            };
        }

        private static ExpenseCategoryCreateDto CreateInvalidExpenseCategoryCreateDto()
        {
            return new ExpenseCategoryCreateDto
            {
                Name = string.Empty
            };
        }

        private static ExpenseCategoryUpdateDto CreateInvalidExpenseCategoryUpdateDto()
        {
            return new ExpenseCategoryUpdateDto
            {
                Name = string.Empty
            };
        }

        private static async Task<ExpenseCategory> CreateExpenseCategory(CarExpesesDbContext dbContext, string name)
        {
            var category = new ExpenseCategory
            {
                Name = name
            };

            dbContext.ExpenseCategories.Add(category);
            await dbContext.SaveChangesAsync();

            return category;
        }
    }
}
