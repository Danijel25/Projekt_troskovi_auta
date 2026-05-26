using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using CarExpenses.Web.Api.Dtos;
using CarExpenses.Web.Api.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers.Api;

[Authorize(Roles = AppRoles.Admin)]
[ApiController]
[Route("api/users")]
public sealed class UsersApiController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly IUserRepository userRepository;

    public UsersApiController(UserManager<User> userManager, IUserRepository userRepository)
    {
        this.userManager = userManager;
        this.userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserSummaryDto>>> GetAll([FromQuery] string? search)
    {
        var users = await userRepository
            .Query(new UserFilter { Search = search })
            .ToListAsync();
        var result = users.Select(DtoMapping.ToSummaryDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDetailDto>> GetById(int id)
    {
        var user = await userManager.Users
            .Include(item => item.Cars)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(DtoMapping.ToDetailDto(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserDetailDto>> Create(UserCreateDto dto)
    {
        var user = new User
        {
            UserName = dto.Username,
            Email = dto.Email
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        var roleResult = await userManager.AddToRoleAsync(user, AppRoles.BasicUser);
        if (!roleResult.Succeeded)
        {
            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        var created = await userManager.Users
            .Include(item => item.Cars)
            .AsNoTracking()
            .FirstAsync(item => item.Id == user.Id);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, DtoMapping.ToDetailDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UserUpdateDto dto)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }

        user.UserName = dto.Username;
        user.Email = dto.Email;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await userManager.ResetPasswordAsync(user, token, dto.Password);
            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return ValidationProblem(ModelState);
            }
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        return NoContent();
    }
}
